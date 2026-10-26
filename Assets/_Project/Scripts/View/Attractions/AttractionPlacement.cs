using Fairground.Model;
using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Places booth-space content using the plane remembered in <see cref="FairgroundSession"/>.
    /// Booth props are authored at identity; this maps them into the selected surface pose.
    /// XR rig stays outside — only attraction content is transformed.
    /// </summary>
    public static class AttractionPlacement
    {
        /// <summary>
        /// Applies <see cref="FairgroundSession.SelectedPlane"/> when present.
        /// No-op (returns false) when missing — Editor / PlayMode keep identity booth.
        /// </summary>
        public static bool TryApplyFromSession(params Transform[] boothSpaceRoots)
        {
            if (!FairgroundSession.SelectedPlane.HasValue)
                return false;

            Apply(FairgroundSession.SelectedPlane.Value, boothSpaceRoots);
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
        /// Position from plane pose; yaw from horizontal projection of forward (stable booth on tables).
        /// </summary>
        public static void ResolvePose(DetectedPlane plane, out Vector3 position, out Quaternion rotation)
        {
            var pose = plane.Pose;
            var planePosition = new Vector3(pose.PositionX, pose.PositionY, pose.PositionZ);
            var raw = new Quaternion(pose.RotationX, pose.RotationY, pose.RotationZ, pose.RotationW);
            if (raw == default)
                raw = Quaternion.identity;

            var planeRotation = FlatYaw(raw);
            if (TryKeepOffsetFromHead(planePosition, planeRotation, out position, out rotation))
                return;

            position = planePosition;
            rotation = planeRotation;
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

        static bool TryKeepOffsetFromHead(Vector3 planePosition, Quaternion planeRotation, out Vector3 position, out Quaternion rotation)
        {
            position = default;
            rotation = default;
            if (!AttractionViewpoint.HasValue)
                return false;

            var camera = Camera.main;
            if (camera == null)
                return false;

            var storedYaw = Quaternion.Euler(0f, AttractionViewpoint.Yaw, 0f);
            var nowYaw = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f);
            var delta = nowYaw * Quaternion.Inverse(storedYaw);
            position = camera.transform.position + delta * (planePosition - AttractionViewpoint.Position);
            rotation = delta * planeRotation;
            return true;
        }
    }
}
