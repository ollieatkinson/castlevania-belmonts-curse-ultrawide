using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime;

namespace BelmontsCurseUltrawide;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class GameSignatureAttribute : Attribute
{
    internal readonly Type ReturnType;
    internal readonly Type[] Parameters;

    public GameSignatureAttribute(Type returnType, params Type[] parameters)
    {
        ReturnType = returnType;
        Parameters = parameters;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class GameFieldAttribute : Attribute
{
    internal readonly Type DeclaringType;
    internal readonly string Name;
    internal readonly Type FieldType;
    public bool Static { get; set; }

    public GameFieldAttribute(Type declaringType, string name, Type fieldType)
    {
        DeclaringType = declaringType;
        Name = name;
        FieldType = fieldType;
    }
}

internal static class Compatibility
{
    internal static void CheckLoaders()
    {
        foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            if (string.Equals(module.ModuleName, "SUWSF.asi", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Remove the SUWSF version of this mod before using the BepInEx version.");
    }

    internal static void CheckPatch(Type patchType)
    {
        var patch = patchType.GetCustomAttributes<HarmonyPatch>().Single().info;
        var signature = patchType.GetCustomAttribute<GameSignatureAttribute>()
            ?? throw new InvalidOperationException($"No expected signature for {patchType.Name}.");
        Method(patch.declaringType, patch.methodName, signature.ReturnType, signature.Parameters);
        foreach (var field in patchType.GetCustomAttributes<GameFieldAttribute>())
            Field(field.DeclaringType, field.Name, field.FieldType, field.Static);
    }

    internal static IntPtr Method(Type type, string name, Type returnType, params Type[] parameters)
    {
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        var method = AccessTools.DeclaredMethod(type, name, parameters);
        if (method is null || method.IsStatic || method.ReturnType != returnType)
            throw new MissingMethodException($"Expected instance method {type.FullName}.{name}({string.Join(", ", parameters.Select(TypeName))}) returning {TypeName(returnType)}.");
        var field = Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method)
            ?? throw new MissingMemberException($"No IL2CPP method for {type.FullName}.{name}.");
        var info = (IntPtr)field.GetValue(null)!;
        var klass = Il2CppClassPointerStore.GetNativeClassPointer(type);

        // Reject missing-method stubs and stale interop tokens before inspecting
        // the pointer. A token can otherwise resolve to an unrelated method.
        var iterator = IntPtr.Zero;
        var found = false;
        if (klass != IntPtr.Zero && info != IntPtr.Zero)
        {
            IntPtr candidate;
            while ((candidate = IL2CPP.il2cpp_class_get_methods(klass, ref iterator)) != IntPtr.Zero)
                if (candidate == info) { found = true; break; }
        }
        if (!found) throw new MissingMethodException($"No live IL2CPP method for {type.FullName}.{name}.");

        if (IL2CPP.il2cpp_method_get_name_(info) != name
            || !IL2CPP.il2cpp_method_is_instance(info)
            || IL2CPP.il2cpp_method_is_generic(info)
            || IL2CPP.il2cpp_method_get_param_count(info) != parameters.Length)
            throw new InvalidOperationException($"IL2CPP signature changed: {type.FullName}.{name}.");
        CheckType(IL2CPP.il2cpp_method_get_return_type(info), returnType, $"{type.FullName}.{name} return type");
        for (var i = 0; i < parameters.Length; i++)
            CheckType(IL2CPP.il2cpp_method_get_param(info, (uint)i), parameters[i], $"{type.FullName}.{name} parameter {i + 1}");
        return info;
    }

    internal static void Field(Type type, string name, Type expected, bool isStatic = false)
    {
        var klass = Il2CppClassPointerStore.GetNativeClassPointer(type);
        var field = klass == IntPtr.Zero ? IntPtr.Zero : IL2CPP.il2cpp_class_get_field_from_name(klass, name);
        if (field == IntPtr.Zero) throw new MissingFieldException(type.FullName, name);
        if (((IL2CPP.il2cpp_field_get_flags(field) & (uint)FieldAttributes.Static) != 0) != isStatic)
            throw new InvalidOperationException($"Static/instance status changed: {type.FullName}.{name}.");
        CheckType(IL2CPP.il2cpp_field_get_type(field), expected, $"{type.FullName}.{name}");
    }

    internal static unsafe void CheckStage<T>(string name, T value) where T : struct, Enum
    {
        var klass = Il2CppClassPointerStore.GetNativeClassPointer(typeof(T));
        if (klass == IntPtr.Zero || !IL2CPP.il2cpp_class_is_enum(klass) || Enum.GetUnderlyingType(typeof(T)) != typeof(int))
            throw new InvalidOperationException($"Expected an Int32 enum for {typeof(T).FullName}.");
        CheckType(IL2CPP.il2cpp_class_enum_basetype(klass), typeof(int), typeof(T).FullName!);
        var field = IL2CPP.il2cpp_class_get_field_from_name(klass, name);
        if (field == IntPtr.Zero) throw new MissingFieldException(typeof(T).FullName, name);
        CheckType(IL2CPP.il2cpp_field_get_type(field), typeof(T), $"{typeof(T).FullName}.{name}");
        var actual = 0;
        IL2CPP.il2cpp_field_static_get_value(field, &actual);
        if (actual != Convert.ToInt32(value))
            throw new InvalidOperationException($"Enum value changed: {typeof(T).FullName}.{name}.");
    }

    private static void CheckType(IntPtr nativeType, Type expected, string location)
    {
        if (nativeType == IntPtr.Zero) throw new InvalidOperationException($"Missing type: {location}.");
        var pointer = IL2CPP.il2cpp_type_get_name(nativeType);
        string actual;
        try { actual = Normalize(Marshal.PtrToStringUTF8(pointer) ?? ""); }
        finally { if (pointer != IntPtr.Zero) IL2CPP.il2cpp_free(pointer); }
        if (actual.TrimEnd('&') != TypeName(expected.IsByRef ? expected.GetElementType()! : expected)
            || IL2CPP.il2cpp_type_is_byref(nativeType) != expected.IsByRef)
            throw new InvalidOperationException($"Signature changed at {location}: expected {TypeName(expected)}, found {actual}.");
    }

    private static string TypeName(Type type)
    {
        if (type.IsByRef) return TypeName(type.GetElementType()!) + "&";
        if (type.IsArray) return TypeName(type.GetElementType()!) + "[" + new string(',', type.GetArrayRank() - 1) + "]";
        if (type.IsGenericType)
            return Normalize(type.GetGenericTypeDefinition().FullName!) + "<" + string.Join(",", type.GetGenericArguments().Select(TypeName)) + ">";
        return Normalize(type.FullName!);
    }

    private static string Normalize(string name) => Regex.Replace(name, @"`\d+", "").Replace("Il2CppSystem.", "System.").Replace('/', '.').Replace('+', '.');

    internal static void Warn(string reason)
    {
        var message = $"{reason}\n\nThe game will continue without the widescreen fixes.\n\nMore details are in BepInEx/LogOutput.log.";
        try { MessageBoxW(IntPtr.Zero, message, "Ultrawide patch not applied", 0x30); }
        catch (Exception e) { Plugin.Logger.LogWarning($"Could not show compatibility warning: {e.Message}"); }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int MessageBoxW(IntPtr window, string text, string caption, uint type);
}
