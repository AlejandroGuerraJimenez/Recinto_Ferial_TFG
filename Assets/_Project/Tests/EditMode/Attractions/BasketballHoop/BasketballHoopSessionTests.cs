using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions.BasketballHoop;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.BasketballHoop
{
    public sealed class BasketballHoopSessionTests
    {
        [Test]
        public void AttractionScenes_MapsBasketballHoopId()
        {
            Assert.AreEqual(5, (int)AttractionId.BasketballHoop);
            Assert.AreEqual("BasketballHoop", AttractionScenes.GetSceneName(AttractionId.BasketballHoop));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/BasketballHoop.unity",
                AttractionScenes.GetScenePath(AttractionId.BasketballHoop));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("BasketballHoop", out var id));
            Assert.AreEqual(AttractionId.BasketballHoop, id);
        }

        [Test]
        public void Session_Start_InitializesCounters()
        {
            var session = CreateSession(2, 10, 5);
            session.Start();
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(10, session.AttemptsRemaining);
            Assert.AreEqual(0, session.InFlight);
        }

        [Test]
        public void Session_Start_DoesNotReset_RestartDoes()
        {
            var session = CreateSession(2, 2, 5);
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
        public void Session_Basket_AddsScoreAndCanWin()
        {
            var session = CreateSession(2, 10, 4);
            session.Start();
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(2, session.Score);
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(4, session.Score);
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.IsFalse(session.TryRegisterSuccess());
        }

        [Test]
        public void Session_BallsExhausted_LosesWhenScoreBelowThresholdAfterBallsResolve()
        {
            var session = CreateSession(1, 2, 5);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
            Assert.IsFalse(session.TrySpendAttempt());
            Assert.IsFalse(session.TryRegisterSuccess());
        }

        [Test]
        public void Session_LastBall_CanWinBeforeBallResolves()
        {
            var session = CreateSession(5, 1, 5);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
        }

        [Test]
        public void Session_LastBall_LosesAfterBallResolvesWithoutBasket()
        {
            var session = CreateSession(2, 1, 5);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(0, session.AttemptsRemaining);
            Assert.AreEqual(1, session.InFlight);
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsFalse(session.TrySpendAttempt());
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new BasketballHoopViewModel(new BasketballHoopRules(2, 10, 5));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.TryRegisterThrow());
            Assert.IsTrue(vm.TryRegisterBasket());
            Assert.AreEqual(3, events);
            Assert.AreEqual(2, vm.Score);
            Assert.AreEqual(9, vm.BallsRemaining);
            Assert.AreEqual("¡A la canasta!", BasketballHoopHud.Status(vm));
            Assert.AreEqual(AttractionId.BasketballHoop, vm.AttractionId);
            Assert.AreEqual("BasketballHoop", AttractionCatalog.GetSceneName(vm.AttractionId));
        }

        [Test]
        public void ViewModel_FromCounts_OwnsThrowAndRestart()
        {
            var vm = new BasketballHoopViewModel(2, 1, 5);
            vm.StartGame();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
            Assert.IsTrue(vm.TryRegisterThrow());
            Assert.IsFalse(vm.CanThrow);
            Assert.AreEqual(0, vm.BallsRemaining);
            vm.TryResolveThrow();
            Assert.AreEqual(MatchPhase.Lost, vm.Phase);
            Assert.AreEqual("¡Sin balones!", BasketballHoopHud.Status(vm));
            vm.Restart();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(1, vm.BallsRemaining);
            Assert.AreEqual(0, vm.Score);
            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
        }

        [Test]
        public void ViewModel_ReachingThreshold_WinsAndBlocksFurtherThrows()
        {
            var vm = new BasketballHoopViewModel(5, 3, 5);
            vm.StartGame();
            Assert.IsTrue(vm.TryRegisterBasket());
            Assert.AreEqual(MatchPhase.Won, vm.Phase);
            Assert.AreEqual("¡Ganaste!", BasketballHoopHud.Status(vm));
            Assert.IsFalse(vm.CanThrow);
            Assert.IsFalse(vm.TryRegisterThrow());
            Assert.IsFalse(vm.TryRegisterBasket());
        }

        static AttemptMatch CreateSession(int points, int balls, int winScore)
        {
            return new AttemptMatch(new BasketballHoopRules(points, balls, winScore).Attempt);
        }
    }
}
