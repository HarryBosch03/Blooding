using UnityEngine;
using UnityEngine.Rendering;

namespace Blooding.Runtime.Rendering
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRenderer cameraRenderer = new();

        public CustomRenderPipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (var camera in cameras)
            {
                cameraRenderer.Render(context, camera);
            }
        }
    }
}