using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Forces opaque VR camera clear (skybox or solid). No passthrough.
    /// </summary>
    public sealed class AttractionVrCameraSetup : MonoBehaviour
    {
        [SerializeField] Camera targetCamera;
        [SerializeField] bool useSkybox = true;
        [SerializeField] Color solidBackground = new Color(0.15f, 0.35f, 0.55f, 1f);

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
    }
}
