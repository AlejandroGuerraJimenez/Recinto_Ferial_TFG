using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions.DuckFishing;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.DuckFishing
{
    public sealed class DuckFishingMatchTests
    {
        [Test]
        public void AttractionScenes_MapsDuckFishingId()
        {
            Assert.AreEqual("DuckFishing", AttractionScenes.GetSceneName(AttractionId.DuckFishing));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/DuckFishing.unity",
                AttractionScenes.GetScenePath(AttractionId.DuckFishing));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("DuckFishing", out var id));
            Assert.AreEqual(AttractionId.DuckFishing, id);
        }

        [Test]
        public void Match_Start_OpensTheClock()
        {
            var session = new DuckFishingMatch(new DuckFishingRules(30, 6));
            session.Start();
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(30f, session.SecondsRemaining);
            Assert.AreEqual(6, session.DuckCount);
        }

        [Test]
        public void Match_Catch_ScoresOnePerDuck()
        {
            var session = new DuckFishingMatch(new DuckFishingRules(30, 6));
            session.Start();
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.AreEqual(2, session.Score);
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
        }

        [Test]
        public void Match_TimeUp_LocksTheDuckCount()
        {
            var session = new DuckFishingMatch(new DuckFishingRules(10, 6));
            session.Start();
            Assert.IsTrue(session.TryRegisterCatch());
            session.Tick(10f);
            Assert.AreEqual(0f, session.SecondsRemaining);
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.AreEqual(1, session.Score);
            Assert.IsFalse(session.TryRegisterCatch());
        }

        [Test]
        public void Match_TimeUpWithNone_EndsWithoutPoints()
        {
            var session = new DuckFishingMatch(new DuckFishingRules(5, 6));
            session.Start();
            session.Tick(5f);
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
            Assert.AreEqual(0, session.Score);
        }

        [Test]
        public void Match_AllDucks_EndsBeforeTheClock()
        {
            var session = new DuckFishingMatch(new DuckFishingRules(30, 2));
            session.Start();
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.AreEqual(2, session.Score);
            Assert.Greater(session.SecondsRemaining, 0f);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new DuckFishingViewModel(new DuckFishingRules(20, 6));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartMatch();
            Assert.IsTrue(vm.TryRegisterCatch());
            Assert.AreEqual(2, events);
            Assert.AreEqual("¡Engancha el aro!", DuckFishingHud.Status(vm));
            Assert.AreEqual(1, vm.Score);
            Assert.AreEqual(AttractionId.DuckFishing, vm.AttractionId);
        }

        [Test]
        public void ViewModel_Tick_RefreshesWhenTheSecondChanges()
        {
            var vm = new DuckFishingViewModel(10, 6);
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartMatch();
            vm.Tick(0.2f);
            Assert.AreEqual(1, events);
            vm.Tick(0.9f);
            Assert.AreEqual(2, events);
            Assert.AreEqual(9, vm.SecondsLeft);
        }

        [Test]
        public void ViewModel_Restart_ResetsTheRound()
        {
            var vm = new DuckFishingViewModel(12, 6);
            vm.StartMatch();
            Assert.IsTrue(vm.CanFish);
            Assert.IsTrue(vm.TryRegisterCatch());
            vm.Tick(12f);
            Assert.IsFalse(vm.CanFish);
            vm.Restart();
            Assert.IsTrue(vm.CanFish);
            Assert.AreEqual(12, vm.SecondsLeft);
            Assert.AreEqual(0, vm.Score);
            Assert.AreEqual(0, vm.DucksCaught);
        }
    }
}
