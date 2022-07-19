using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class BlitPass : ScriptableRenderPass
    {
        Material m_BlitMaterial;

        private int m_Width;
        private int m_Height;
        private BlitColorTransform m_BlitColorTransform;
        public enum BlitColorTransform
        {
            None,
            Gamma2Line,
            Line2Gamma
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPass(RenderPassEvent evt, Material blitMaterial)
        {
            base.profilingSampler = new ProfilingSampler(nameof(BlitPass));

            m_BlitMaterial = blitMaterial;
            renderPassEvent = evt;
            base.useNativeRenderPass = false;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(int width,int height, BlitColorTransform blitColorTransform)
        {
            m_Width = width;
            m_Height = height;
            m_BlitColorTransform = blitColorTransform;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd,ref renderingData);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_BlitMaterial == null)
            {
                Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", m_BlitMaterial, GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();

            //It is possible that the given color target is now the frontbuffer
            // if (source == renderingData.cameraData.renderer.GetCameraColorFrontBuffer(cmd))
            // {
            //     source = renderingData.cameraData.renderer.cameraColorTarget;
            // }
            var renderer = renderingData.cameraData.renderer as UniversalRenderer;
            var colorBuffer = renderer.m_ColorBufferSystem;
            bool needChangeSize = RenderTargetBufferSystem.m_Desc.width != m_Width|| RenderTargetBufferSystem.m_Desc.height!= m_Height;
            if (needChangeSize)
            {
                colorBuffer.ReSizeFrontBuffer(cmd, m_Width, m_Height);
               
            }
            bool useDrawProceduleBlit = renderingData.cameraData.xr.enabled;
            if (m_BlitColorTransform == BlitColorTransform.Gamma2Line)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, true);
            }
            else if (m_BlitColorTransform == BlitColorTransform.Line2Gamma)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion,true);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion,false);
            }
            RenderTargetIdentifier source;
            source = renderingData.cameraData.renderer.cameraColorTarget;
            RenderingUtils.Blit(cmd, source, colorBuffer.GetFrontBuffer().id, m_BlitMaterial, 0, useDrawProceduleBlit,
                RenderBufferLoadAction.DontCare,
                RenderBufferStoreAction.Store,
                RenderBufferLoadAction.DontCare,
                RenderBufferStoreAction.DontCare);
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion,false);
            if (needChangeSize)
            {
                colorBuffer.ReSizeBackBufferAndSave(cmd, m_Width, m_Height);
                renderer.ResizeDepth(cmd, ref RenderTargetBufferSystem.m_Desc, m_Width, m_Height);
            }
            renderer.SwapColorBuffer(cmd);
            //using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.Bilt)))
            //{
            //    RenderTargetIdentifier opaqueColorRT = destination.Identifier();
            //
            //    ScriptableRenderer.SetRenderTarget(cmd, opaqueColorRT, BuiltinRenderTextureType.CameraTarget, clearFlag,
            //        clearColor);
            //
            //    bool useDrawProceduleBlit = renderingData.cameraData.xr.enabled;
            //    switch (m_DownsamplingMethod)
            //    {
            //        case Downsampling.None:
            //            RenderingUtils.Blit(cmd, source, opaqueColorRT, m_BlitMaterial, 0, useDrawProceduleBlit);
            //            break;
            //        case Downsampling._2xBilinear:
            //            RenderingUtils.Blit(cmd, source, opaqueColorRT, m_BlitMaterial, 0, useDrawProceduleBlit);
            //            break;
            //        case Downsampling._4xBox:
            //            m_SamplingMaterial.SetFloat(m_SampleOffsetShaderHandle, 2);
            //            RenderingUtils.Blit(cmd, source, opaqueColorRT, m_SamplingMaterial, 0, useDrawProceduleBlit);
            //            break;
            //        case Downsampling._4xBilinear:
            //            RenderingUtils.Blit(cmd, source, opaqueColorRT, m_BlitMaterial, 0, useDrawProceduleBlit);
            //            break;
            //    }
            //}

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //if (cmd == null)
            //    throw new ArgumentNullException("cmd");
            //
            //if (destination != RenderTargetHandle.CameraTarget)
            //{
            //    cmd.ReleaseTemporaryRT(destination.id);
            //    destination = RenderTargetHandle.CameraTarget;
            //}
        }
    }
}
