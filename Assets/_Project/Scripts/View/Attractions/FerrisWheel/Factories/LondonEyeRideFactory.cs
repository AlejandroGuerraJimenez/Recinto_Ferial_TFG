using CesiumForUnity;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel.Factories
{
    /// <summary>
    /// Builds the London Eye ride: georeference at the hub, Google tiles when a token exists,
    /// and one counter-rotating capsule with the XR Origin seated inside, looking outward.
    /// </summary>
    public sealed class LondonEyeRideFactory
    {
        public const string RootName = "CesiumGeoreference";

        public FerrisWheelRigView Create(GameObject xrOrigin)
        {
            CesiumGeoreference georeference = CreateGeoreference();
            CreateTileset(georeference.transform);
            FerrisWheelRigView rig = CreateRide(georeference.transform);
            SeatXrOrigin(xrOrigin, rig.GetCabin(0));
            return rig;
        }

        static CesiumGeoreference CreateGeoreference()
        {
            var root = new GameObject(RootName);
            var georeference = root.AddComponent<CesiumGeoreference>();
            georeference.SetOriginLongitudeLatitudeHeight(
                LondonEyeLayout.Longitude,
                LondonEyeLayout.Latitude,
                LondonEyeLayout.HubHeightMeters);
            return georeference;
        }

        static void CreateTileset(Transform parent)
        {
            if (!CesiumIonToken.TryRead(out string token))
            {
                Debug.LogError(
                    "[FerrisWheel] No Cesium ion token. The capsule still spins, but Google Photorealistic 3D Tiles were not created. " +
                    "Add the token to " + CesiumIonToken.ProjectAssetPath +
                    " or set the " + CesiumIonToken.EnvironmentVariable + " environment variable.");
                return;
            }

            var tilesetObject = new GameObject("GooglePhotorealisticTiles");
            tilesetObject.transform.SetParent(parent, false);
            var tileset = tilesetObject.AddComponent<Cesium3DTileset>();
            bool quest = Application.platform == RuntimePlatform.Android;
            tileset.tilesetSource = CesiumDataSource.FromCesiumIon;
            tileset.ionAssetID = LondonEyeLayout.GooglePhotorealisticAssetId;
            tileset.ionAccessToken = token;
            tileset.showCreditsOnScreen = true;
            tileset.createPhysicsMeshes = false;
            tileset.maximumScreenSpaceError = quest
                ? LondonEyeLayout.QuestMaximumScreenSpaceError
                : LondonEyeLayout.EditorMaximumScreenSpaceError;
            tileset.maximumCachedBytes = quest
                ? LondonEyeLayout.QuestMaximumCachedBytes
                : LondonEyeLayout.EditorMaximumCachedBytes;
        }

        static FerrisWheelRigView CreateRide(Transform parent)
        {
            var axis = new GameObject("WheelAxis");
            axis.transform.SetParent(parent, false);
            axis.transform.localRotation = Quaternion.Euler(0f, LondonEyeLayout.AxisYawDegrees, 0f);

            var wheel = new GameObject("Wheel");
            wheel.transform.SetParent(axis.transform, false);

            var capsule = new GameObject("Capsule");
            capsule.transform.SetParent(wheel.transform, false);
            capsule.transform.localPosition = new Vector3(
                0f,
                -LondonEyeLayout.RadiusMeters,
                LondonEyeLayout.OutwardClearanceMeters);

            var rig = axis.AddComponent<FerrisWheelRigView>();
            rig.Configure(wheel.transform, new[] { capsule.transform });
            return rig;
        }

        static void SeatXrOrigin(GameObject xrOrigin, Transform capsule)
        {
            if (xrOrigin == null)
            {
                Debug.LogError("[FerrisWheel] XR Origin is missing, so the capsule has no viewpoint.");
                return;
            }

            Transform origin = xrOrigin.transform;
            origin.SetParent(capsule, false);
            origin.localPosition = Vector3.zero;
            origin.localRotation = Quaternion.identity;
            origin.localScale = Vector3.one;

            // The shared rig wants the player on the room floor, free to walk.
            // This ride keeps the origin in the capsule; head tracking still looks around.
            Disable<AttractionSpawnAlign>(xrOrigin);
            Disable<AttractionStickMove>(xrOrigin);
        }

        static void Disable<T>(GameObject host) where T : Behaviour
        {
            T behaviour = host.GetComponent<T>();
            if (behaviour != null)
                behaviour.enabled = false;
        }
    }
}
