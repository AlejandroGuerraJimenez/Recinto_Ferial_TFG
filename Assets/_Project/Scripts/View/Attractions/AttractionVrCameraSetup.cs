using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Forces opaque VR camera clear (skybox or solid). No passthrough.
    /// </summary>
    public sealed class AttractionVrCameraSetup : MonoBehaviour
    {
        [SerializeField] Camera targetCamera;
        [SerializeField] bool useSkybox;
        [SerializeField] Color solidBackground = new Color(0.35f, 0.55f, 0.75f, 1f);

        void Awake() => Apply();

        public void Configure(Camera camera, bool skybox)
        {
            targetCamera = camera;
            useSkybox = skybox;
            Apply();
        }

        public void Apply()
        {
            if (!ResolveCamera())
                return;

            if (useSkybox)
                ApplySkybox();
            else
                ApplySolidColor();

            ForceOpaqueRender();
        }

        bool ResolveCamera()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            return targetCamera != null;
        }

        void ApplySkybox()
        {
            targetCamera.clearFlags = CameraClearFlags.Skybox;
        }

        void ApplySolidColor()
        {
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
            var color = solidBackground;
            color.a = 1f;
            targetCamera.backgroundColor = color;
        }

        void ForceOpaqueRender()
        {
            var color = targetCamera.backgroundColor;
            color.a = 1f;
            targetCamera.backgroundColor = color;
            targetCamera.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.Default;
            targetCamera.allowHDR = true;

            if (targetCamera.GetUniversalAdditionalCameraData() is { } urp)
            {
                urp.renderType = CameraRenderType.Base;
                urp.requiresColorOption = CameraOverrideOption.Off;
                urp.requiresDepthOption = CameraOverrideOption.Off;
            }
        }
    }
}
