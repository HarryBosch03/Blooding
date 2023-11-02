using UnityEngine;
using UnityEngine.Rendering;

namespace Blooding.Runtime.Rendering
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext context;
        private CullingResults cullingResults;
        private Camera camera;

#if UNITY_EDITOR
        public string SampleName { get; set; }
#else
        public const string SampleName;
#endif

        private CommandBuffer cmd = new();

        private static ShaderTagId unlitShaderTagId = new ("SRPDefaultUnlit");

        private static ShaderTagId[] legacyShaderTagIds =
        {
            new ("Always"),
            new ("ForwardBase"),
            new ("PrepassBase"),
            new ("Vertex"),
            new ("VertexLMRGBM"),
            new ("VertexLM")
        };

        private static Material errorMaterial;

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            this.context = context;
            this.camera = camera;

            PrepareBuffer();
            PrepareForSceneWindow();
            if (!Cull()) return;

            Setup();
            DrawVisibleGeometry();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();
        }

        partial void PrepareBuffer ();
        partial void DrawGizmos();
        partial void PrepareForSceneWindow();

        private void DrawUnsupportedShaders()
        {
            if (!errorMaterial) errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));

            var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
            {
                overrideMaterial = errorMaterial,
            };
            var filteringSettings = FilteringSettings.defaultValue;

            for (var i = 1; i < legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }

            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        private void Setup()
        {
            context.SetupCameraProperties(camera);
            var flags = camera.clearFlags;
            cmd.ClearRenderTarget
            (
                flags <= CameraClearFlags.Depth,
                flags == CameraClearFlags.Color,
                flags == CameraClearFlags.Color ? 
                    camera.backgroundColor.linear : Color.clear
            );
            cmd.BeginSample(SampleName);
            ExecuteBuffer();
        }

        private bool Cull()
        {
            if (camera.TryGetCullingParameters(out var cullingParameters))
            {
                cullingResults = context.Cull(ref cullingParameters);
                return true;
            }

            return false;
        }

        private void DrawVisibleGeometry()
        {
            var sortingSettings = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

            context.DrawSkybox(camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;

            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        private void Submit()
        {
            cmd.EndSample(SampleName);
            ExecuteBuffer();
            context.Submit();
        }

        private void ExecuteBuffer()
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}