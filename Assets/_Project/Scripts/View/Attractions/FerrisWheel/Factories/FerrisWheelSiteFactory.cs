using System.Collections.Generic;
using CesiumForUnity;
using Fairground.View.Attractions.Rendering;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Fairground.View.Attractions.FerrisWheel.Factories
{
    /// <summary>
    /// Builds the London Eye ride: georeference at the hub, Google tiles with the real wheel cut out,
    /// and the project ferris wheel scaled to a 60 m radius. The player stands on a platform under the wheel.
    /// </summary>
    public sealed class FerrisWheelSiteFactory
    {
        public const string RootName = "CesiumGeoreference";

        public FerrisWheelRigView Create(GameObject xrOrigin)
        {
            CesiumGeoreference georeference = CreateGeoreference();
            CreateTileset(georeference.transform);
            FerrisWheelRigView rig = CreateRide(georeference.transform);
            new FerrisWheelFactory().EnsureLighting();
            SeatOnPlatform(xrOrigin, rig.transform.parent);
            FerrisWheelTicketBooth.Place(rig.transform.parent, rig, xrOrigin);
            RegisterHeadsetCamera(georeference, xrOrigin);
            return rig;
        }

        static CesiumGeoreference CreateGeoreference()
        {
            var root = new GameObject(RootName);
            var georeference = root.AddComponent<CesiumGeoreference>();
            georeference.SetOriginLongitudeLatitudeHeight(
                FerrisWheelSiteLayout.Longitude,
                FerrisWheelSiteLayout.Latitude,
                FerrisWheelSiteLayout.HubHeightMeters);
            return georeference;
        }

        static void CreateTileset(Transform parent)
        {
            if (!CesiumIonToken.TryRead(out string token))
            {
                Debug.LogError(
                    "[FerrisWheel] No Cesium ion token. The wheel still spins, but Google Photorealistic 3D Tiles were not created. " +
                    "Add the token to " + CesiumIonToken.ProjectAssetPath +
                    " or set the " + CesiumIonToken.EnvironmentVariable + " environment variable.");
                return;
            }

            var tilesetObject = new GameObject("GooglePhotorealisticTiles");
            tilesetObject.SetActive(false);
            tilesetObject.transform.SetParent(parent, false);
            var tileset = tilesetObject.AddComponent<Cesium3DTileset>();
            bool quest = Application.platform == RuntimePlatform.Android;
            tileset.tilesetSource = CesiumDataSource.FromCesiumIon;
            tileset.ionAssetID = FerrisWheelSiteLayout.GooglePhotorealisticAssetId;
            tileset.ionAccessToken = token;
            tileset.showCreditsOnScreen = true;
            tileset.createPhysicsMeshes = false;
            tileset.maximumScreenSpaceError = quest
                ? FerrisWheelSiteLayout.QuestMaximumScreenSpaceError
                : FerrisWheelSiteLayout.EditorMaximumScreenSpaceError;
            tileset.maximumCachedBytes = quest
                ? FerrisWheelSiteLayout.QuestMaximumCachedBytes
                : FerrisWheelSiteLayout.EditorMaximumCachedBytes;

            CutOutLondonEye(tilesetObject, parent);
            tilesetObject.SetActive(true);
        }

        /// <summary>
        /// Hides only the photorealistic London Eye. The rectangle follows the wheel plane
        /// and stays narrow along the axle, so the park and the river stay visible.
        /// </summary>
        static void CutOutLondonEye(GameObject tilesetObject, Transform georeference)
        {
            var cutout = new GameObject("LondonEyeCutout");
            cutout.transform.SetParent(georeference, false);
            var polygon = cutout.AddComponent<CesiumCartographicPolygon>();
            WriteCutout(cutout.GetComponent<SplineContainer>());

            var overlay = tilesetObject.AddComponent<CesiumPolygonRasterOverlay>();
            overlay.materialKey = "Clipping";
            overlay.invertSelection = false;
            overlay.excludeSelectedTiles = true;
            overlay.polygons = new List<CesiumCartographicPolygon> { polygon };
        }

        static void WriteCutout(SplineContainer container)
        {
            if (container == null)
            {
                Debug.LogError("[FerrisWheel] The London Eye cutout has no spline, so the photorealistic wheel stays.");
                return;
            }

            var leftover = new List<Spline>();
            foreach (Spline existing in container.Splines)
                leftover.Add(existing);
            for (int i = 0; i < leftover.Count; i++)
                container.RemoveSpline(leftover[i]);

            Vector3 along = WheelDirection();
            Vector3 axle = AxleDirection();
            float halfLength = FerrisWheelSiteLayout.CutoutHalfLengthMeters;
            float halfWidth = FerrisWheelSiteLayout.CutoutHalfWidthMeters;
            Vector3[] corners =
            {
                along * halfLength + axle * halfWidth,
                -along * halfLength + axle * halfWidth,
                -along * halfLength - axle * halfWidth,
                along * halfLength - axle * halfWidth,
            };

            var spline = new Spline { Closed = true };
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 corner = corners[i];
                spline.Add(new BezierKnot(new float3(corner.x, 0f, corner.z)), TangentMode.Linear);
            }

            container.AddSpline(spline);
        }

        static FerrisWheelRigView CreateRide(Transform parent)
        {
            var axis = new GameObject("WheelAxis");
            axis.transform.SetParent(parent, false);
            axis.transform.localRotation = Quaternion.Euler(0f, FerrisWheelSiteLayout.AxisYawDegrees, 0f);

            var factory = new FerrisWheelFactory();
            FerrisWheelRigView rig = factory.Create(FerrisWheelSiteLayout.CabinCount);
            Transform wheelRoot = rig.transform;
            float scale = FerrisWheelSiteLayout.RadiusMeters / FerrisWheelLayout.Radius;
            wheelRoot.SetParent(axis.transform, false);
            wheelRoot.localRotation = Quaternion.identity;
            wheelRoot.localScale = Vector3.one * scale;
            // The factory hub sits at WheelCenter. Shift the root so that hub lands on the georeference origin.
            wheelRoot.localPosition = -scale * FerrisWheelLayout.WheelCenter;
            // Beam centre sits on the slab, so the sill does not poke through it.
            const float footRestMeters = 1.4f;
            float footWorldY = -FerrisWheelSiteLayout.FootBelowHubMeters + footRestMeters;
            float footY = FerrisWheelLayout.HubHeight + footWorldY / scale;
            factory.PlaceFeet(wheelRoot, footY);
            LiftCabinsAbovePlatform(rig, scale);
            CreateWalkFloor(axis.transform);

            RemoveChild(wheelRoot, "Ground");
            RemoveChild(wheelRoot, "Platform");
            RemoveChild(wheelRoot, "Sign");
            RemoveChild(wheelRoot, "Title");
            return rig;
        }

        static void CreateWalkFloor(Transform axis)
        {
            const float thickness = FerrisWheelSiteLayout.FloorThicknessMeters;
            float top = -FerrisWheelSiteLayout.FootBelowHubMeters;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "WalkFloor";
            floor.transform.SetParent(axis, false);
            floor.transform.localPosition = new Vector3(0f, top - thickness * 0.5f, 0f);
            floor.transform.localScale = new Vector3(
                FerrisWheelSiteLayout.PlatformHalfAlongMeters * 2f,
                thickness,
                FerrisWheelSiteLayout.PlatformHalfAxleMeters * 2f);
            PrimitiveMaterialApplier.Apply(floor, new Color(0.55f, 0.52f, 0.47f, 1f));
        }

        /// <summary>
        /// The scaled cabin bodies hang well below the rim. Raise them so the lowest one clears the platform.
        /// </summary>
        static void LiftCabinsAbovePlatform(FerrisWheelRigView rig, float scale)
        {
            Transform pivot = LowestCabin(rig);
            Transform body = pivot.Find("Body");
            if (body == null)
                return;

            float bottomLocal = body.localPosition.y - body.localScale.y * 0.5f;
            float bottomY = pivot.position.y + bottomLocal * pivot.lossyScale.y;
            float floorY = -FerrisWheelSiteLayout.FootBelowHubMeters;
            float liftWorld = floorY + FerrisWheelSiteLayout.CabinClearanceMeters - bottomY;
            if (liftWorld <= 0.01f)
                return;

            float liftLocal = liftWorld / scale;
            for (int i = 0; i < rig.CabinCount; i++)
                RaiseCabin(rig.GetCabin(i), liftLocal);
        }

        static void RaiseCabin(Transform pivot, float liftLocal)
        {
            RaiseChild(pivot, "Hanger", liftLocal);
            RaiseChild(pivot, "Body", liftLocal);
            RaiseChild(pivot, "Roof", liftLocal);
        }

        static void RaiseChild(Transform pivot, string childName, float liftLocal)
        {
            Transform child = pivot.Find(childName);
            if (child == null)
                return;

            Vector3 local = child.localPosition;
            local.y += liftLocal;
            child.localPosition = local;
        }

        static void RemoveChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
                Object.Destroy(child.gameObject);
        }

        static Transform LowestCabin(FerrisWheelRigView rig)
        {
            Transform lowest = rig.GetCabin(0);
            for (int i = 1; i < rig.CabinCount; i++)
            {
                Transform cabin = rig.GetCabin(i);
                if (cabin.localPosition.y < lowest.localPosition.y)
                    lowest = cabin;
            }

            return lowest;
        }

        static void SeatOnPlatform(GameObject xrOrigin, Transform axis)
        {
            if (xrOrigin == null || axis == null)
            {
                Debug.LogError("[FerrisWheel] XR Origin is missing, so the platform has no viewpoint.");
                return;
            }

            float floorY = -FerrisWheelSiteLayout.FootBelowHubMeters;
            Transform origin = xrOrigin.transform;
            origin.SetParent(axis, false);
            origin.localPosition = new Vector3(0f, floorY, FerrisWheelSiteLayout.PlayerLocalZ);
            origin.localRotation = Quaternion.identity;
            origin.localScale = Vector3.one;

            Camera camera = origin.GetComponentInChildren<Camera>();
            if (camera != null)
            {
                Vector3 local = camera.transform.localPosition;
                if (local.y < 0.5f)
                    local.y = FerrisWheelSiteLayout.EyeHeightMeters;
                camera.transform.localPosition = local;
            }

            // Floor tracking would pull the head back to the hub. Walking stays on this slab.
            Disable<AttractionSpawnAlign>(xrOrigin);
            Enable<AttractionStickMove>(xrOrigin);
        }

        static void RegisterHeadsetCamera(CesiumGeoreference georeference, GameObject xrOrigin)
        {
            Camera camera = xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>(true) : null;
            if (camera == null || georeference == null)
                return;

            CesiumCameraManager manager = CesiumCameraManager.GetOrCreate(georeference.gameObject);
            if (!manager.additionalCameras.Contains(camera))
                manager.additionalCameras.Add(camera);
        }

        static void Enable<T>(GameObject host) where T : Behaviour
        {
            T behaviour = host.GetComponent<T>();
            if (behaviour != null)
                behaviour.enabled = true;
        }

        static void Disable<T>(GameObject host) where T : Behaviour
        {
            T behaviour = host.GetComponent<T>();
            if (behaviour != null)
                behaviour.enabled = false;
        }

        /// <summary>Horizontal direction of the wheel, local +X of the yawed axle, in east/up/north.</summary>
        static Vector3 WheelDirection()
        {
            float yaw = FerrisWheelSiteLayout.AxisYawDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(yaw), 0f, -Mathf.Sin(yaw));
        }

        /// <summary>Axle direction, local +Z of the yawed axle, in east/up/north.</summary>
        static Vector3 AxleDirection()
        {
            float yaw = FerrisWheelSiteLayout.AxisYawDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(yaw), 0f, Mathf.Cos(yaw));
        }
    }
}
