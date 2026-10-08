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
        Transform _pond;
        DuckTargetView[] _ducks;
        FishingRodView _rod;
        DuckFishingHudView _hud;

        public DuckFishingBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public Transform Pond => _pond;
        public DuckTargetView[] Ducks => _ducks;
        public FishingRodView Rod => _rod;
        public DuckFishingHudView Hud => _hud;

        public DuckFishingBoothBuilder BuildEnvironment()
        {
            _boothFactory.EnsureLighting();
            _boothFactory.CreateGround();
            _boothFactory.CreateBoothBack();
            _pond = _boothFactory.CreatePond();
            return this;
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
