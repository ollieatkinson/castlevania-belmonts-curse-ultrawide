using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using Cinemachine;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using MotherBase.CameraConfiner;

namespace BelmontsCurseUltrawide;

internal sealed class ConfinerHooks : IDisposable
{
    internal readonly Feature Fix = new("camera boundaries");
    [ThreadStatic] private static int boundaryCalculation;
    private readonly List<INativeDetour> hooks = new();
    private AspectGetter aspect = null!;
    private bool observedAspect;

    // BepInEx 6.0.788's Harmony bridge cannot marshal CameraState by reference:
    // it is a non-blittable IL2CPP value type. Forward the pointer unchanged.
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void Pipeline(IntPtr self, IntPtr camera, CinemachineCore.Stage stage,
        IntPtr state, float deltaTime, IntPtr method);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float AspectGetter(IntPtr lens, IntPtr method);

    internal void Install()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            throw new PlatformNotSupportedException("The native camera hooks require an x64 process.");
        var aspectAddress = Address(Compatibility.Method(typeof(LensSettings), "get_Aspect", typeof(float)));
        var simplified = PipelineAddress(typeof(CinemachineConfiner2DSimplified));
        var standard = PipelineAddress(typeof(CinemachineConfiner2D));
        Compatibility.CheckStage(nameof(CinemachineCore.Stage.Body), CinemachineCore.Stage.Body);
        if (!IL2CPP.il2cpp_class_is_valuetype(Il2CppClassPointerStore<LensSettings>.NativeClassPtr))
            throw new InvalidOperationException("LensSettings is no longer a value type.");
        // Check all three native signatures before applying the first detour.
        hooks.Add(INativeDetour.CreateAndApply<AspectGetter>(aspectAddress, GetAspect, out aspect));
        AddPipeline(typeof(CinemachineConfiner2DSimplified), simplified);
        AddPipeline(typeof(CinemachineConfiner2D), standard);
    }

    private float GetAspect(IntPtr lens, IntPtr method)
    {
        var value = aspect(lens, method);
        if (!Fix.Enabled || boundaryCalculation == 0) return value;
        try
        {
            var bounded = Math.Min(value, Plugin.NormalAspect);
            if (!observedAspect)
            {
                observedAspect = true;
                Plugin.Logger.LogInfo($"Camera boundary aspect: {value:F5} -> {bounded:F5}");
            }
            return bounded;
        }
        catch (Exception e) { Fix.Disable(e); return value; }
    }

    private void AddPipeline(Type type, IntPtr address)
    {
        Pipeline original = null!;
        var observed = false;
        void Callback(IntPtr self, IntPtr camera, CinemachineCore.Stage stage, IntPtr state, float deltaTime, IntPtr method)
        {
            var confining = Fix.Enabled && stage == CinemachineCore.Stage.Body;
            if (confining)
            {
                boundaryCalculation++;
                try
                {
                    if (!observed)
                    {
                        observed = true;
                        Plugin.Logger.LogInfo($"Camera boundary hook active: {type.Name}");
                    }
                }
                catch (Exception e) { Fix.Disable(e); }
            }
            try { original(self, camera, stage, state, deltaTime, method); }
            finally { if (confining) boundaryCalculation--; }
        }
        hooks.Add(INativeDetour.CreateAndApply<Pipeline>(address, Callback, out original));
    }

    private static IntPtr PipelineAddress(Type type) => Address(Compatibility.Method(type, "PostPipelineStageCallback", typeof(void),
        typeof(CinemachineVirtualCameraBase), typeof(CinemachineCore.Stage), typeof(CameraState).MakeByRefType(), typeof(float)));

    private static unsafe IntPtr Address(IntPtr info)
    {
        var address = UnityVersionHandler.Wrap((Il2CppMethodInfo*)info).MethodPointer;
        if (address == IntPtr.Zero) throw new MissingMethodException("No native camera method address.");
        return address;
    }

    public void Dispose()
    {
        for (var i = hooks.Count - 1; i >= 0; i--)
        {
            try { hooks[i].Dispose(); }
            catch (Exception e) { Fix.Disable(e); }
        }
        hooks.Clear();
    }
}
