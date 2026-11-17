using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRenderFeature : ScriptableRendererFeature
{
    class CRTPass : ScriptableRenderPass
    {
        public Material crtMaterial;
        private RTHandle tempRT;

        public CRTPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref tempRT, desc, name: "_CRTTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //Debug.Log("CRT Execute firing");
            if (crtMaterial == null)
            {
                Debug.Log("CRT material is null!");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("CRT Effect");
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blitter.BlitCameraTexture(cmd, source, tempRT, crtMaterial, 0);
            Blitter.BlitCameraTexture(cmd, tempRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) { }
        public void Dispose() { tempRT?.Release(); }
    }

    [System.Serializable]
    public class CRTSettings { public Material crtMaterial; }
    public CRTSettings settings = new CRTSettings();
    private CRTPass crtPass;

    public static bool IsEnabled = true;

    public override void Create()
    {
        crtPass = new CRTPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!IsEnabled) return;

#if UNITY_EDITOR
        if (renderingData.cameraData.camera.cameraType == CameraType.SceneView) return;
#endif

        if (settings.crtMaterial == null) return;
        crtPass.crtMaterial = settings.crtMaterial;
        renderer.EnqueuePass(crtPass);
    }

    protected override void Dispose(bool disposing) { crtPass?.Dispose(); }
}