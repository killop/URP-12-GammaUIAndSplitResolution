using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Draw  objects into the given color and depth target
    ///
    /// You can use this pass to render objects that have a material and/or shader
    /// with the pass names UniversalForward or SRPDefaultUnlit.
    /// </summary>
    public class DrawObjectsPass : ScriptableRenderPass
    {
        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;
        bool m_IsOpaque;
        bool m_IsUICamera;

        bool m_UseDepthPriming;
       // SphericalHarmonicsTool shData;
        static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

        static readonly int SHAr_ID = Shader.PropertyToID("my_unity_SHAr");
        static readonly int SHAg_ID = Shader.PropertyToID("my_unity_SHAg");
        static readonly int SHAb_ID = Shader.PropertyToID("my_unity_SHAb");
        static readonly int SHBr_ID = Shader.PropertyToID("my_unity_SHBr");
        static readonly int SHBg_ID = Shader.PropertyToID("my_unity_SHBg");
        static readonly int SHBb_ID = Shader.PropertyToID("my_unity_SHBb");
        static readonly int SHC_ID = Shader.PropertyToID ("my_unity_SHC");

        public DrawObjectsPass(string profilerTag, ShaderTagId[] shaderTagIds, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
        {
            base.profilingSampler = new ProfilingSampler(nameof(DrawObjectsPass));

            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            foreach (ShaderTagId sid in shaderTagIds)
                m_ShaderTagIdList.Add(sid);
            renderPassEvent = evt;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_IsOpaque = opaque;

            if (stencilState.enabled)
            {
                m_RenderStateBlock.stencilReference = stencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = stencilState;
            }
            
          //  shData = new SphericalHarmonicsTool();
        }

        public void Setup(bool isUICamera = false)
        {
            this.m_IsUICamera = isUICamera;
        }

        public DrawObjectsPass(string profilerTag, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
            : this(profilerTag,
            new ShaderTagId[] { new ShaderTagId("SRPDefaultUnlit"), new ShaderTagId("UniversalForward"), new ShaderTagId("UniversalForwardOnly") },
            opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference)
        { }

        internal DrawObjectsPass(URPProfileId profileId, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
            : this(profileId.GetType().Name, opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference)
        {
            m_ProfilingSampler = ProfilingSampler.Get(profileId);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.renderer.useDepthPriming && m_IsOpaque && (renderingData.cameraData.renderType == CameraRenderType.Base || renderingData.cameraData.clearDepth))
            {
                m_RenderStateBlock.depthState = new DepthState(false, CompareFunction.Equal);
                m_RenderStateBlock.mask |= RenderStateMask.Depth;
            }
            else if (m_RenderStateBlock.depthState.compareFunction == CompareFunction.Equal)
            {
                m_RenderStateBlock.depthState = new DepthState(true, CompareFunction.LessEqual);
                m_RenderStateBlock.mask |= RenderStateMask.Depth;
            }
            /*
            var stack = VolumeManager.instance.stack;
            if (stack != null)
            {
                var sceneLight = stack.GetComponent<SceneLight>();
                if (sceneLight != null)
                {
                    shData.GetSHParam(
                        sceneLight.myAmbientRight.value,
                       sceneLight.myAmbientLeft.value,
                       sceneLight.myAmbientSky.value,
                       sceneLight.myAmbientGround.value,
                       sceneLight.myAmbientFront.value,
                       sceneLight.myAmbientBack.value,
                       sceneLight.mySixSideExposure.value,
                       sceneLight.mySixSideRotateDegree.value,
                        sceneLight.myAmbientCubemap.value as Cubemap,
                        sceneLight.myAmbientCubemapExposure.value,
                        sceneLight.myAmbientCubemapRotateDegree.value
                    );
                   RenderSettings.ambientMode = AmbientMode.Trilight;
                   if (sceneLight.ambientSky.overrideState)
                       RenderSettings.ambientSkyColor = sceneLight.ambientSky.value;
                   // Debug.LogError("   "+ RenderSettings.ambientSkyColor+" "+ sceneLight.ambientSky.value+"  id "+ sceneLight.GetInstanceID()); 
                   if (sceneLight.ambientGround.overrideState)
                       RenderSettings.ambientGroundColor = sceneLight.ambientGround.value;
                   if (sceneLight.ambientEquator.overrideState)
                       RenderSettings.ambientEquatorColor = sceneLight.ambientEquator.value;
                   RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
                   if (sceneLight.ambientCubemap.overrideState)
                       RenderSettings.customReflection = sceneLight.ambientCubemap.value as Cubemap;
                   if (sceneLight.ambientCubemapExposure.overrideState)
                       RenderSettings.reflectionIntensity = sceneLight.ambientCubemapExposure.value;
                   
                   if (sceneLight.fog.overrideState)
                       RenderSettings.fog = sceneLight.fog.value;
                   if (sceneLight.fogColor.overrideState)
                       RenderSettings.fogColor = sceneLight.fogColor.value;
                   if (sceneLight.startDistance.overrideState)
                       RenderSettings.fogStartDistance = sceneLight.startDistance.value;
                   if (sceneLight.endDistance.overrideState)
                       RenderSettings.fogEndDistance = sceneLight.endDistance.value;
                }
            }*/
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Global render pass data containing various settings.
                // x,y,z are currently unused
                // w is used for knowing whether the object is opaque(1) or alpha blended(0)
                Vector4 drawObjectPassData = new Vector4(0.0f, 0.0f, 0.0f, (m_IsOpaque) ? 1.0f : 0.0f);
                cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                if (m_IsUICamera)
                {
                    cmd.SetGlobalFloat(ShaderPropertyId.isInUICamera, 1);
                }
                else
                {
                    cmd.SetGlobalFloat(ShaderPropertyId.isInUICamera, 0);
                }
              //cmd.SetGlobalVector(SHAr_ID, shData.result[0]);
              //cmd.SetGlobalVector(SHAg_ID, shData.result[1]);
              //cmd.SetGlobalVector(SHAb_ID, shData.result[2]);
              //cmd.SetGlobalVector(SHBr_ID, shData.result[3]);
              //cmd.SetGlobalVector(SHBg_ID, shData.result[4]);
              //cmd.SetGlobalVector(SHBb_ID, shData.result[5]);
              //cmd.SetGlobalVector(SHC_ID, shData.result[6]);

                // scaleBias.x = flipSign
                // scaleBias.y = scale
                // scaleBias.z = bias
                // scaleBias.w = unused
                float flipSign = (renderingData.cameraData.IsCameraProjectionMatrixFlipped()) ? -1.0f : 1.0f;
                Vector4 scaleBias = (flipSign < 0.0f)
                    ? new Vector4(flipSign, 1.0f, -1.0f, 1.0f)
                    : new Vector4(flipSign, 0.0f, 1.0f, 1.0f);
                cmd.SetGlobalVector(ShaderPropertyId.scaleBiasRt, scaleBias);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                Camera camera = renderingData.cameraData.camera;
                var sortFlags = (m_IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                if (renderingData.cameraData.renderer.useDepthPriming && m_IsOpaque && (renderingData.cameraData.renderType == CameraRenderType.Base || renderingData.cameraData.clearDepth))
                    sortFlags = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder;

                var filterSettings = m_FilteringSettings;

#if UNITY_EDITOR
                // When rendering the preview camera, we want the layer mask to be forced to Everything
                if (renderingData.cameraData.isPreviewCamera)
                {
                    filterSettings.layerMask = -1;
                }
#endif

                DrawingSettings drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);

                var activeDebugHandler = GetActiveDebugHandler(renderingData);
                if (activeDebugHandler != null)
                {
                    activeDebugHandler.DrawWithDebugRenderState(context, cmd, ref renderingData, ref drawSettings, ref filterSettings, ref m_RenderStateBlock,
                        (ScriptableRenderContext ctx, ref RenderingData data, ref DrawingSettings ds, ref FilteringSettings fs, ref RenderStateBlock rsb) =>
                        {
                            ctx.DrawRenderers(data.cullResults, ref ds, ref fs, ref rsb);
                        });
                }
                else
                {
                    context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings, ref m_RenderStateBlock);

                    // Render objects that did not match any shader pass with error shader
                    RenderingUtils.RenderObjectsWithError(context, ref renderingData.cullResults, camera, filterSettings, SortingCriteria.None);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
