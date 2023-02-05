using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// this was used on https://gamedevbill.com, but originally taken from https://cyangamedev.wordpress.com/2020/06/22/urp-post-processing/

// Saved in Blit.cs
public class KuwaharaRendererFeature : ScriptableRendererFeature {
 
    public class KuwaharaPass : ScriptableRenderPass {
        public enum RenderTarget {
            Color,
            RenderTexture,
        }
 
        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }
 
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
 
        RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;
         
        public KuwaharaPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag) {
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");
        }
         
        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
            this.source = source;
            this.destination = destination;
        }
         
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
 
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            
            // Can't read and write to same color target, use a TemporaryRT
            if (destination == RenderTargetHandle.CameraTarget) {
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, blitShaderPassIndex);
                Blit(cmd, m_TemporaryColorTexture.Identifier(), source);
            } else {
                Blit(cmd, source, destination.Identifier(), blitMaterial, blitShaderPassIndex);
            }
 
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
         
        public override void FrameCleanup(CommandBuffer cmd) {
            if (destination == RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }
 
    [System.Serializable]
    public class KuwaharaSetings {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
 
        public Material kuwaharaMaterial = null;
        public int kuwaharaMaterialPassIndex = 0;
        public Target destination = Target.Color;
        public string textureId = "_BlitPassTexture";
    }
 
    public enum Target {
        Color,
        Texture
    }
 
    public KuwaharaSetings settings = new KuwaharaSetings();
    RenderTargetHandle m_RenderTextureHandle;
 
    KuwaharaPass blitPass;
 
    public override void Create() {
        var passIndex = settings.kuwaharaMaterial != null ? settings.kuwaharaMaterial.passCount - 1 : 1;
        settings.kuwaharaMaterialPassIndex = Mathf.Clamp(settings.kuwaharaMaterialPassIndex, -1, passIndex);
        blitPass = new KuwaharaPass(settings.Event, settings.kuwaharaMaterial, settings.kuwaharaMaterialPassIndex, name);
        m_RenderTextureHandle.Init(settings.textureId);
    }
 
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        var src = renderer.cameraColorTarget;
        var dest = (settings.destination == Target.Color) ? RenderTargetHandle.CameraTarget : m_RenderTextureHandle;
 
        if (settings.kuwaharaMaterial == null) {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }
 
        blitPass.Setup(src, dest);
        renderer.EnqueuePass(blitPass);
    }
}