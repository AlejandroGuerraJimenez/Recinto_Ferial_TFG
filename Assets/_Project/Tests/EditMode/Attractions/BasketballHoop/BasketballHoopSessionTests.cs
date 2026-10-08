using Fairground.Core.Attractions;
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
            Assert.AreEqual(BasketballHoopPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(10, session.BallsRemaining);
            Assert.AreEqual(0, session.BallsInFlight);
        }

        [Test]
        public void Session_Basket_AddsScoreAndCanWin()
        {
            var session = CreateSession(2, 10, 4);
            session.Start();
            Assert.IsTrue(session.TryRegisterBasket());
            Assert.AreEqual(2, session.Score);
            Assert.AreEqual(BasketballHoopPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterBasket());
            Assert.AreEqual(4, session.Score);
            Assert.AreEqual(BasketballHoopPhase.Won, session.Phase);
            Assert.IsFalse(session.TryRegisterBasket());
        }

        [Test]
        public void Session_BallsExhausted_LosesWhenScoreBelowThresholdAfterBallsResolve()
        {
            var session = CreateSession(1, 2, 5);
            session.Start();
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.AreEqual(BasketballHoopPhase.Playing, session.Phase);
            Assert.IsTrue(session.ResolveThrow());
            Assert.IsTrue(session.ResolveThrow());
            Assert.AreEqual(BasketballHoopPhase.Lost, session.Phase);
            Assert.IsFalse(session.TryRegisterThrow());
            Assert.IsFalse(session.TryRegisterBasket());
        }

        [Test]
        public void Session_LastBall_CanWinBeforeBallResolves()
        {
            var session = CreateSession(5, 1, 5);
            session.Start();
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.AreEqual(BasketballHoopPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterBasket());
            Assert.AreEqual(BasketballHoopPhase.Won, session.Phase);
            Assert.IsTrue(session.ResolveThrow());
            Assert.AreEqual(BasketballHoopPhase.Won, session.Phase);
        }

        [Test]
        public void Session_LastBall_LosesAfterBallResolvesWithoutBasket()
        {
            var session = CreateSession(2, 1, 5);
            session.Start();
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.AreEqual(0, session.BallsRemaining);
            Assert.AreEqual(1, session.BallsInFlight);
            Assert.AreEqual(BasketballHoopPhase.Playing, session.Phase);
            Assert.IsFalse(session.TryRegisterThrow());
            Assert.IsTrue(session.ResolveThrow());
            Assert.AreEqual(BasketballHoopPhase.Lost, session.Phase);
        }

        [Test]
        public void EndCondition_Strategy_CanForceWin()
        {
            var session = new BasketballHoopSession(new BasketballHoopRules(2, 10, 5), new AlwaysWinEndCondition());
            session.Start();
            session.TryRegisterThrow();
            Assert.AreEqual(BasketballHoopPhase.Won, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new BasketballHoopViewModel(new BasketballHoopRules(2, 10, 5));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.NotifyBallThrown());
            Assert.IsTrue(vm.NotifyBasket());
            Assert.AreEqual(3, events);
            Assert.AreEqual(2, vm.Score);
            Assert.AreEqual(9, vm.BallsRemaining);
            Assert.AreEqual("Shoot the hoop!", vm.StatusText);
            Assert.AreEqual(AttractionId.BasketballHoop, vm.AttractionId);
            Assert.AreEqual("BasketballHoop", vm.SceneName);
        }

        [Test]
        public void ViewModel_FromCounts_OwnsThrowAndRestart()
        {
            var vm = new BasketballHoopViewModel(2, 1, 5);
            vm.StartGame();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(BasketballHoopPhase.Playing, vm.Phase);
            Assert.IsTrue(vm.NotifyBallThrown());
            Assert.IsFalse(vm.CanThrow);
            Assert.AreEqual(0, vm.BallsRemaining);
            vm.NotifyThrowResolved();
            Assert.AreEqual(BasketballHoopPhase.Lost, vm.Phase);
            Assert.AreEqual("Out of balls!", vm.StatusText);
            vm.Restart();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(1, vm.BallsRemaining);
            Assert.AreEqual(0, vm.Score);
            Assert.AreEqual(BasketballHoopPhase.Playing, vm.Phase);
        }

        [Test]
        public void ViewModel_ReachingThreshold_WinsAndBlocksFurtherThrows()
        {
            var vm = new BasketballHoopViewModel(5, 3, 5);
            vm.StartGame();
            Assert.IsTrue(vm.NotifyBasket());
            Assert.AreEqual(BasketballHoopPhase.Won, vm.Phase);
            Assert.AreEqual("You win!", vm.StatusText);
            Assert.IsFalse(vm.CanThrow);
            Assert.IsFalse(vm.NotifyBallThrown());
            Assert.IsFalse(vm.NotifyBasket());
        }

        static BasketballHoopSession CreateSession(int points, int balls, int winScore)
        {
            return new BasketballHoopSession(new BasketballHoopRules(points, balls, winScore));
        }

        sealed class AlwaysWinEndCondition : IBasketballHoopEndCondition
        {
            public BasketballHoopPhase Evaluate(
                BasketballHoopRules rules,
                int score,
                int ballsRemaining,
                int ballsInFlight)
            {
                return BasketballHoopPhase.Won;
            }
        }
    }
}
