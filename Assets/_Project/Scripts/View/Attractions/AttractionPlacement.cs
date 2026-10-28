using Fairground.Model;
using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Puts booth content on the floor at the origin, in front of the new scene's XR rig.
    /// The rig stays there and tracks the headset, so the previous scene's coordinates are not reused.
    /// </summary>
    public static class AttractionPlacement
    {
        /// <summary>
        /// Applies <see cref="FairgroundSession.SelectedPlane"/> when the installer received a visit.
        /// No-op (returns false) when missing — Editor / PlayMode keep the authored booth.
        /// </summary>
        public static bool TryApply(FairgroundSession session, params Transform[] boothSpaceRoots)
        {
            if (session == null || !session.SelectedPlane.HasValue)
                return false;

            Apply(session.SelectedPlane.Value, boothSpaceRoots);
            return true;
        }

        public static void Apply(DetectedPlane plane, params Transform[] boothSpaceRoots)
        {
            if (boothSpaceRoots == null || boothSpaceRoots.Length == 0)
                return;

            ResolvePose(plane, out var position, out var rotation);
            for (var i = 0; i < boothSpaceRoots.Length; i++)
                ApplyToRoot(boothSpaceRoots[i], position, rotation);
        }

        static void ApplyToRoot(Transform root, Vector3 planePosition, Quaternion planeRotation)
        {
            if (root == null)
                return;

            var localPosition = root.position;
            var localRotation = root.rotation;
            root.SetPositionAndRotation(
                planePosition + planeRotation * localPosition,
                planeRotation * localRotation);
        }

        /// <summary>
        /// Keeps the authored floor at the origin, turned to face the player.
        /// </summary>
        public static void ResolvePose(DetectedPlane plane, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = FacingYaw(FlatYaw(ReadRotation(plane)));
        }

        static Quaternion ReadRotation(DetectedPlane plane)
        {
            var pose = plane.Pose;
            var raw = new Quaternion(pose.RotationX, pose.RotationY, pose.RotationZ, pose.RotationW);
            return raw == default ? Quaternion.identity : raw;
        }

        static Quaternion FlatYaw(Quaternion rotation)
        {
            var forward = rotation * Vector3.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            else
                forward.Normalize();

            return Quaternion.LookRotation(forward, Vector3.up);
        }

        static Quaternion FacingYaw(Quaternion fallback)
        {
            var camera = Camera.main;
            if (camera == null)
                return fallback;

            var forward = camera.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                return fallback;

            return Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
    }
}
