using Fairground.View.Attractions.DuckFishing.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Builders
{
    /// <summary>
    /// Builder: assembles pond environment + gameplay objects.
    /// </summary>
    public sealed class DuckFishingBoothBuilder
    {
        readonly BoothPropFactory _boothFactory = new BoothPropFactory();
        readonly DuckTargetFactory _duckFactory = new DuckTargetFactory();
        readonly FishingRodFactory _rodFactory = new FishingRodFactory();
        readonly DuckFishingHudFactory _hudFactory = new DuckFishingHudFactory();
        readonly XrAttractionRigFactory _rigFactory;

        GameObject _xrOrigin;
        GameObject _environment;
        Transform _pond;
        DuckTargetView[] _ducks;
        FishingRodView _rod;
        DuckFishingHudView _hud;

        public DuckFishingBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public GameObject Environment => _environment;
        public Transform Pond => _pond;
        public DuckTargetView[] Ducks => _ducks;
        public FishingRodView Rod => _rod;
        public DuckFishingHudView Hud => _hud;

        /// <summary>
        /// Lighting only — booth mesh already present in the baked scene.
        /// </summary>
        public DuckFishingBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _boothFactory.EnsureLighting();
            _environment = bakedRoot != null ? bakedRoot.gameObject : null;
            _pond = ResolvePond(bakedRoot);
            return this;
        }

        /// <summary>
        /// Instantiates a prebuilt booth prefab, or falls back to CreatePrimitive factory.
        /// </summary>
        public DuckFishingBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _boothFactory.EnsureLighting();
            _environment = boothEnvironmentPrefab != null
                ? Object.Instantiate(boothEnvironmentPrefab)
                : _boothFactory.CreateEnvironment();

            if (_environment != null)
                _environment.name = BoothPropFactory.EnvironmentRootName;

            _pond = ResolvePond(_environment != null ? _environment.transform : null);
            return this;
        }

        static Transform ResolvePond(Transform environmentRoot)
        {
            if (environmentRoot == null)
                return null;

            var pond = environmentRoot.Find(BoothPropFactory.PondName);
            return pond != null ? pond : environmentRoot;
        }

        public DuckFishingBoothBuilder BuildXrRig()
        {
            _xrOrigin = _rigFactory.EnsureXrOrigin();
            _rigFactory.EnsureInteractionManager();
            _rigFactory.EnsureEventSystem();
            return this;
        }

        public DuckFishingBoothBuilder BuildGameplay(int duckCount)
        {
            _ducks = _duckFactory.CreateRing(_pond, duckCount);
            _rod = _rodFactory.Create(_xrOrigin);
            _hud = _hudFactory.Create();
            return this;
        }
    }
}
