using Fairground.Core.Attractions;
using Fairground.Model.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions.DuckFishing;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.DuckFishing
{
    public sealed class DuckFishingSessionTests
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
        public void Session_Start_InitializesCounters()
        {
            var session = CreateSession(10, 8, 6, 4);
            session.Start();
            Assert.AreEqual(DuckFishingPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(8, session.AttemptsRemaining);
            Assert.AreEqual(6, session.DucksRemaining);
        }

        [Test]
        public void Session_Catch_AddsScoreAndCanWin()
        {
            var session = CreateSession(10, 8, 6, 2);
            session.Start();
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.AreEqual(10, session.Score);
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.AreEqual(DuckFishingPhase.Won, session.Phase);
        }

        [Test]
        public void Session_AttemptsExhausted_LosesWhenNotEnoughCaught()
        {
            var session = CreateSession(10, 2, 6, 3);
            session.Start();
            Assert.IsTrue(session.TryRegisterDip());
            Assert.IsTrue(session.TryRegisterDip());
            Assert.AreEqual(DuckFishingPhase.Playing, session.Phase);
            Assert.IsTrue(session.ResolveDip());
            Assert.IsTrue(session.ResolveDip());
            Assert.AreEqual(DuckFishingPhase.Lost, session.Phase);
            Assert.IsFalse(session.TryRegisterDip());
        }

        [Test]
        public void Session_LastDip_CanWinBeforeResolve()
        {
            var session = CreateSession(10, 1, 6, 1);
            session.Start();
            Assert.IsTrue(session.TryRegisterDip());
            Assert.AreEqual(DuckFishingPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterCatch());
            Assert.AreEqual(DuckFishingPhase.Won, session.Phase);
            Assert.IsTrue(session.ResolveDip());
            Assert.AreEqual(DuckFishingPhase.Won, session.Phase);
        }

        [Test]
        public void EndCondition_Strategy_CanForceWin()
        {
            var session = new DuckFishingSession(new DuckFishingRules(10, 8, 6, 4), new AlwaysWinEndCondition());
            session.Start();
            session.TryRegisterDip();
            Assert.AreEqual(DuckFishingPhase.Won, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new DuckFishingViewModel(new DuckFishingRules(10, 3, 6, 2));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.NotifyDipStarted());
            Assert.IsTrue(vm.NotifyDuckCaught());
            Assert.AreEqual(3, events);
            Assert.AreEqual("Catch 2 ducks!", vm.StatusText);
            Assert.AreEqual(AttractionId.DuckFishing, vm.AttractionId);
        }

        [Test]
        public void ViewModel_Restart_ResetsMatch()
        {
            var vm = new DuckFishingViewModel(10, 2, 6, 4);
            vm.StartGame();
            Assert.IsTrue(vm.CanDip);
            Assert.IsTrue(vm.NotifyDipStarted());
            Assert.IsTrue(vm.CanDip);
            Assert.AreEqual(1, vm.AttemptsRemaining);
            vm.Restart();
            Assert.IsTrue(vm.CanDip);
            Assert.AreEqual(2, vm.AttemptsRemaining);
            Assert.AreEqual(0, vm.Score);
        }

        static DuckFishingSession CreateSession(int points, int attempts, int ducks, int toWin)
        {
            return new DuckFishingSession(new DuckFishingRules(points, attempts, ducks, toWin));
        }

        sealed class AlwaysWinEndCondition : IDuckFishingEndCondition
        {
            public DuckFishingPhase Evaluate(
                DuckFishingRules rules,
                int ducksCaught,
                int attemptsRemaining,
                int dipsInFlight)
            {
                return DuckFishingPhase.Won;
            }
        }
    }
}
