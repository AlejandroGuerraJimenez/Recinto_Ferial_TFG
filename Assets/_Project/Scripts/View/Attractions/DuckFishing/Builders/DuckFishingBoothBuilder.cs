using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Builders
{
    /// <summary>
    /// Ducks and the rod. The shared shell builds the booth around them.
    /// </summary>
    public sealed class DuckFishingBoothBuilder : BoothBuilder
    {
        readonly DuckTargetFactory _duckFactory = new DuckTargetFactory();
        readonly FishingRodFactory _rodFactory = new FishingRodFactory();
        Transform _pond;

        public Transform Pond => _pond;

        public DuckTargetView[] Ducks { get; private set; }

        public FishingRodView Rod { get; private set; }

        public DuckFishingBoothBuilder(GameObject xrOriginPrefab)
            : base(DuckFishingBoothLayout.Props(), xrOriginPrefab)
        {
        }

        protected override void OnEnvironmentReady(Transform environmentRoot)
        {
            _pond = ResolvePond(environmentRoot);
        }

        protected override void BuildGameplay()
        {
            Ducks = _duckFactory.CreateRing(_pond, DuckFishingBalance.DuckCount);
            Rod = _rodFactory.Create(XrOrigin);
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
