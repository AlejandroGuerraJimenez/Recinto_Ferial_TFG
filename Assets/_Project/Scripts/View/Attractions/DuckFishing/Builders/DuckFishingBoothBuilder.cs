using Fairground.View.Attractions.DuckFishing.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Builders
{
    /// <summary>
    /// Assembles ducks, the rod, and the shared booth shell.
    /// </summary>
    public sealed class DuckFishingBoothBuilder
    {
        readonly DuckTargetFactory _duckFactory = new DuckTargetFactory();
        readonly FishingRodFactory _rodFactory = new FishingRodFactory();
        readonly BoothHudFactory _hudFactory = new BoothHudFactory();
        readonly BoothAssembly _shell;

        Transform _pond;
        DuckTargetView[] _ducks;
        FishingRodView _rod;
        BoothHudView _hud;

        public DuckFishingBoothBuilder(GameObject xrOriginPrefab)
        {
            _shell = new BoothAssembly(
                DuckFishingBoothLayout.Props(),
                xrOriginPrefab,
                DuckFishingBoothLayout.LogTag,
                DuckFishingBoothLayout.Sky);
        }

        public GameObject XrOrigin => _shell.XrOrigin;
        public Transform Pond => _pond;
        public DuckTargetView[] Ducks => _ducks;
        public FishingRodView Rod => _rod;
        public BoothHudView Hud => _hud;

        public DuckFishingBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _shell.UseBakedEnvironment(bakedRoot);
            _pond = ResolvePond(bakedRoot);
            return this;
        }

        public DuckFishingBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _shell.BuildEnvironment(boothEnvironmentPrefab);
            _pond = ResolvePond(_shell.Environment != null ? _shell.Environment.transform : null);
            return this;
        }

        public DuckFishingBoothBuilder BuildXrRig()
        {
            _shell.BuildXrRig();
            return this;
        }

        public DuckFishingBoothBuilder BuildGameplay(int duckCount)
        {
            _ducks = _duckFactory.CreateRing(_pond, duckCount);
            _rod = _rodFactory.Create(_shell.XrOrigin);
            _hud = _hudFactory.Create(DuckFishingBoothLayout.HudPosition, DuckFishingBoothLayout.HudLines);
            return this;
        }

        public Transform[] BoothSpaceRoots()
        {
            return new[]
            {
                _shell.Environment != null ? _shell.Environment.transform : null,
                _hud != null ? _hud.transform : null,
            };
        }

        static Transform ResolvePond(Transform environmentRoot)
        {
            if (environmentRoot == null)
                return null;

            Transform pond = environmentRoot.Find(DuckFishingBoothLayout.PondName);
            return pond != null ? pond : environmentRoot;
        }
    }
}
