using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StaticDistortionFeature : ScriptableRendererFeature
{
    class StaticPass : ScriptableRenderPass
    {
        public Material mat;
        private RTHandle tempRT;

        public StaticPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref tempRT, desc, name: "_StaticTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mat == null) return;
            if (renderingData.cameraData.cameraType != CameraType.Game) return;

            CommandBuffer cmd = CommandBufferPool.Get("Static Distortion");
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blitter.BlitCameraTexture(cmd, source, tempRT, mat, 0);
            Blitter.BlitCameraTexture(cmd, tempRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) { }
        public void Dispose() { tempRT?.Release(); }
    }

    [System.Serializable]
    public class Settings { public Material staticMaterial; }
    public Settings settings = new Settings();
    private StaticPass pass;

    public static bool IsEnabled = false;
    public static float Intensity = 0f;

    public override void Create() { pass = new StaticPass(); }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!IsEnabled || settings.staticMaterial == null) return;
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        pass.mat = settings.staticMaterial;
        pass.mat.SetFloat("_Intensity", Intensity);
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing) { pass?.Dispose(); }
}