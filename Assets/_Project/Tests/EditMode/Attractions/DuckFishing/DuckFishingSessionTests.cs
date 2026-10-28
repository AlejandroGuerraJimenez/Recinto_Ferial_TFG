using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
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
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(8, session.AttemptsRemaining);
            Assert.AreEqual(6, session.TargetsRemaining);
        }

        [Test]
        public void Session_Start_DoesNotReset_RestartDoes()
        {
            var session = CreateSession(10, 2, 6, 4);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            session.Start();
            Assert.AreEqual(1, session.AttemptsRemaining);
            session.Restart();
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(2, session.AttemptsRemaining);
            Assert.AreEqual(0, session.InFlight);
        }

        [Test]
        public void Session_Catch_AddsScoreAndCanWin()
        {
            var session = CreateSession(10, 8, 6, 2);
            session.Start();
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(10, session.Score);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
        }

        [Test]
        public void Session_AttemptsExhausted_LosesWhenNotEnoughCaught()
        {
            var session = CreateSession(10, 2, 6, 3);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
            Assert.IsFalse(session.TrySpendAttempt());
        }

        [Test]
        public void Session_LastDip_CanWinBeforeResolve()
        {
            var session = CreateSession(10, 1, 6, 1);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
        }

        [Test]
        public void Session_FourCatchesOfSix_WinsWithDucksLeft()
        {
            var session = CreateSession(10, 8, 6, 4);
            session.Start();
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(3, session.TargetsRemaining);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.AreEqual(2, session.TargetsRemaining);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new DuckFishingViewModel(new DuckFishingRules(10, 3, 6, 2));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.TryRegisterDip());
            Assert.IsTrue(vm.TryRegisterCatch());
            Assert.AreEqual(3, events);
            Assert.AreEqual("¡Atrapa 2 patos!", DuckFishingHud.Status(vm));
            Assert.AreEqual(AttractionId.DuckFishing, vm.AttractionId);
        }

        [Test]
        public void ViewModel_Restart_ResetsMatch()
        {
            var vm = new DuckFishingViewModel(10, 2, 6, 4);
            vm.StartGame();
            Assert.IsTrue(vm.CanDip);
            Assert.IsTrue(vm.TryRegisterDip());
            Assert.IsTrue(vm.CanDip);
            Assert.AreEqual(1, vm.AttemptsRemaining);
            vm.Restart();
            Assert.IsTrue(vm.CanDip);
            Assert.AreEqual(2, vm.AttemptsRemaining);
            Assert.AreEqual(0, vm.Score);
        }

        static AttemptMatch CreateSession(int points, int attempts, int ducks, int toWin)
        {
            return new AttemptMatch(new DuckFishingRules(points, attempts, ducks, toWin).Attempt);
        }
    }
}
