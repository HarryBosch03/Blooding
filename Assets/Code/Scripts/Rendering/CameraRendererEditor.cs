#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Blooding.Runtime.Rendering
{
    public partial class CameraRenderer
    {
        partial void DrawGizmos()
        {
            if (!Handles.ShouldRenderGizmos()) return;
            
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
        
        partial void PrepareForSceneWindow()
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
        }

        partial void PrepareBuffer ()
        {
            cmd.name = camera.name;
            SampleName = camera.name;
        }
    }
}

#endif