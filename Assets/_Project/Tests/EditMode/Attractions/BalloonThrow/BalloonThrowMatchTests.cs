using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions.BalloonThrow;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.BalloonThrow
{
    public sealed class BalloonThrowMatchTests
    {
        [Test]
        public void AttractionScenes_MapsBalloonThrowId()
        {
            Assert.AreEqual("BalloonThrow", AttractionScenes.GetSceneName(AttractionId.BalloonThrow));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/BalloonThrow.unity",
                AttractionScenes.GetScenePath(AttractionId.BalloonThrow));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("BalloonThrow", out var id));
            Assert.AreEqual(AttractionId.BalloonThrow, id);
        }

        [Test]
        public void Match_Start_InitializesCounters()
        {
            var session = CreateMatch(10, 8, 6);
            session.Start();
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(8, session.AttemptsRemaining);
            Assert.AreEqual(6, session.TargetsRemaining);
        }

        [Test]
        public void Match_Start_DoesNotReset_RestartDoes()
        {
            var session = CreateMatch(10, 2, 3);
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
        public void Match_Hit_AddsScoreAndCanWin()
        {
            var session = CreateMatch(10, 8, 2);
            session.Start();
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(10, session.Score);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
        }

        [Test]
        public void Match_ThrowsExhausted_LosesWhenBalloonsRemainAfterBallsResolve()
        {
            var session = CreateMatch(10, 2, 3);
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
        public void Match_LastThrow_CanWinBeforeBallResolves()
        {
            var session = CreateMatch(10, 1, 1);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterSuccess());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Won, session.Phase);
        }

        [Test]
        public void Match_LastThrow_LosesAfterBallResolvesWithoutHit()
        {
            var session = CreateMatch(10, 1, 2);
            session.Start();
            Assert.IsTrue(session.TrySpendAttempt());
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.IsTrue(session.ResolveAttempt());
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new BalloonThrowViewModel(new BalloonThrowRules(10, 3, 2));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartMatch();
            Assert.IsTrue(vm.TryRegisterThrow());
            Assert.IsTrue(vm.TryRegisterPop());
            Assert.AreEqual(3, events);
            Assert.AreEqual("¡A la diana!", BalloonThrowHud.Status(vm));
            Assert.AreEqual("Puntos: 10", BalloonThrowHud.Lines(vm)[0]);
            Assert.AreEqual("¡A la diana!", BalloonThrowHud.Lines(vm)[3]);
            Assert.AreEqual(AttractionId.BalloonThrow, vm.AttractionId);
        }

        [Test]
        public void ViewModel_FromCounts_OwnsSpawnAndRestart()
        {
            var vm = new BalloonThrowViewModel(10, 1, 2);
            vm.StartMatch();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(2, vm.BalloonCount);
            Assert.IsTrue(vm.TryRegisterThrow());
            Assert.IsFalse(vm.CanThrow);
            vm.Restart();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(1, vm.AttemptsRemaining);
            Assert.AreEqual(0, vm.Score);
        }

        [Test]
        public void ViewModel_Bomb_SubtractsPointsWithoutPoppingATarget()
        {
            var vm = new BalloonThrowViewModel(10, 4, 3);
            vm.StartMatch();
            Assert.IsTrue(vm.TryRegisterPop());
            Assert.IsTrue(vm.TryRegisterBomb());
            Assert.AreEqual(10 - BalloonThrowBalance.BombPenalty, vm.Score);
            Assert.AreEqual(1, vm.BalloonsPopped);
            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
            Assert.IsTrue(vm.TryRegisterBomb());
            Assert.AreEqual(10 - BalloonThrowBalance.BombPenalty * 2, vm.Score);
        }

        static AttemptMatch CreateMatch(int points, int throws, int balloons)
        {
            return new AttemptMatch(new BalloonThrowRules(points, throws, balloons).Attempt);
        }
    }
}
