using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using Cinemachine;
using HarmonyLib;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Runtime;
using MotherBase.CameraConfiner;

namespace BelmontsCurseUltrawide;

internal sealed class ConfinerHooks : IDisposable
{
    [ThreadStatic] private static int boundaryCalculation;
    private readonly List<INativeDetour> hooks = new();
    private AspectGetter aspect = null!;

    // BepInEx 6.0.788's Harmony bridge cannot marshal CameraState by reference:
    // it is a non-blittable IL2CPP value type. Forward the pointer unchanged.
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void Pipeline(IntPtr self, IntPtr camera, CinemachineCore.Stage stage,
        IntPtr state, float deltaTime, IntPtr method);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float AspectGetter(IntPtr lens, IntPtr method);

    internal void Install()
    {
        hooks.Add(INativeDetour.CreateAndApply<AspectGetter>(Address(typeof(LensSettings), "get_Aspect"),
            GetAspect, out aspect));
        AddPipeline(typeof(CinemachineConfiner2DSimplified));
        AddPipeline(typeof(CinemachineConfiner2D));
    }

    private float GetAspect(IntPtr lens, IntPtr method)
    {
        var value = aspect(lens, method);
        return boundaryCalculation > 0 ? Math.Min(value, Plugin.NormalAspect) : value;
    }

    private void AddPipeline(Type type)
    {
        Pipeline original = null!;
        void Callback(IntPtr self, IntPtr camera, CinemachineCore.Stage stage, IntPtr state, float deltaTime, IntPtr method)
        {
            var confining = stage == CinemachineCore.Stage.Body;
            if (confining) boundaryCalculation++;
            try { original(self, camera, stage, state, deltaTime, method); }
            finally { if (confining) boundaryCalculation--; }
        }
        hooks.Add(INativeDetour.CreateAndApply<Pipeline>(Address(type, "PostPipelineStageCallback"), Callback, out original));
    }

    private static unsafe IntPtr Address(Type type, string name)
    {
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        var method = AccessTools.DeclaredMethod(type, name) ?? throw new MissingMethodException(type.FullName, name);
        var field = Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method)
            ?? throw new MissingMemberException($"No IL2CPP method for {type.Name}.{name}.");
        var info = (IntPtr)field.GetValue(null)!;
        if (info == IntPtr.Zero) throw new MissingMethodException(type.FullName, name);
        var address = UnityVersionHandler.Wrap((Il2CppMethodInfo*)info).MethodPointer;
        if (address == IntPtr.Zero) throw new MissingMethodException(type.FullName, name);
        return address;
    }

    public void Dispose()
    {
        for (var i = hooks.Count - 1; i >= 0; i--) hooks[i].Dispose();
        hooks.Clear();
    }
}
