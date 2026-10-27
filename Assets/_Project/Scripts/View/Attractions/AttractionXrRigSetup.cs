using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Matches the fairground rig: floor tracking and a free camera.
    /// The starter locomotion object pins the head to a character controller, so the
    /// attraction stays a fixed picture. The fairground AR Input Manager also stops
    /// the XR input subsystem on unload; a new session turns that pose stream back on.
    /// </summary>
    public static class AttractionXrRigSetup
    {
        public static void Apply(GameObject origin)
        {
            if (origin == null)
                return;

            EnsureTrackingSession();
            ConfigureFloorOrigin(origin);
            DisableBuiltInLocomotion(origin);
            DisableCharacterController(origin);
            EnsureComponent<AttractionStickMove>(origin);
            EnsureComponent<AttractionSpawnAlign>(origin);
            EnsureTrackingSession();
        }

        static void ConfigureFloorOrigin(GameObject origin)
        {
            XROrigin xrOrigin = origin.GetComponent<XROrigin>();
            if (xrOrigin == null)
                return;

            xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
            xrOrigin.CameraYOffset = 0f;

            if (xrOrigin.CameraFloorOffsetObject == null)
                return;

            Transform offset = xrOrigin.CameraFloorOffsetObject.transform;
            Vector3 local = offset.localPosition;
            local.y = 0f;
            offset.localPosition = local;
        }

        static void DisableBuiltInLocomotion(GameObject origin)
        {
            Transform locomotion = origin.transform.Find("Locomotion");
            if (locomotion != null)
                locomotion.gameObject.SetActive(false);
        }

        static void DisableCharacterController(GameObject origin)
        {
            CharacterController body = origin.GetComponent<CharacterController>();
            if (body != null)
                body.enabled = false;
        }

        static void EnsureComponent<T>(GameObject origin) where T : Component
        {
            if (origin.GetComponent<T>() == null)
                origin.AddComponent<T>();
        }

        static void EnsureTrackingSession()
        {
#if UNITY_EDITOR
            if (!AttractionDesktopPlay.IsLiveHeadset())
                return;
#endif
            if (Object.FindAnyObjectByType<ARSession>() != null)
                return;

            var sessionObject = new GameObject("AR Session");
            sessionObject.AddComponent<ARInputManager>();
            ARSession session = sessionObject.AddComponent<ARSession>();
            session.requestedTrackingMode = TrackingMode.PositionAndRotation;
        }
    }
}
