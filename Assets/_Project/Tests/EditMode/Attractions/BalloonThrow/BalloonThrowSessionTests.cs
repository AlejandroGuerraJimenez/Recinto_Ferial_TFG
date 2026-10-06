using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions.BalloonThrow;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.BalloonThrow
{
    public sealed class BalloonThrowSessionTests
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
        public void Session_Start_InitializesCounters()
        {
            var session = CreateSession(10, 8, 6);
            session.Start();
            Assert.AreEqual(BalloonThrowPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(8, session.ThrowsRemaining);
            Assert.AreEqual(6, session.BalloonsRemaining);
        }

        [Test]
        public void Session_Hit_AddsScoreAndCanWin()
        {
            var session = CreateSession(10, 8, 2);
            session.Start();
            Assert.IsTrue(session.TryRegisterBalloonHit());
            Assert.AreEqual(10, session.Score);
            Assert.IsTrue(session.TryRegisterBalloonHit());
            Assert.AreEqual(BalloonThrowPhase.Won, session.Phase);
        }

        [Test]
        public void Session_ThrowsExhausted_LosesWhenBalloonsRemain()
        {
            var session = CreateSession(10, 2, 3);
            session.Start();
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.IsTrue(session.TryRegisterThrow());
            Assert.AreEqual(BalloonThrowPhase.Lost, session.Phase);
            Assert.IsFalse(session.TryRegisterThrow());
        }

        [Test]
        public void EndCondition_Strategy_CanForceWin()
        {
            var session = new BalloonThrowSession(new BalloonThrowRules(10, 8, 6), new AlwaysWinEndCondition());
            session.Start();
            session.TryRegisterThrow();
            Assert.AreEqual(BalloonThrowPhase.Won, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new BalloonThrowViewModel(new BalloonThrowRules(10, 3, 2));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.NotifyBallThrown());
            Assert.IsTrue(vm.NotifyBalloonHit());
            Assert.AreEqual(3, events);
            Assert.AreEqual("Pop the balloons!", vm.StatusText);
            Assert.AreEqual(AttractionId.BalloonThrow, vm.AttractionId);
        }

        static BalloonThrowSession CreateSession(int points, int throws, int balloons)
        {
            return new BalloonThrowSession(new BalloonThrowRules(points, throws, balloons));
        }

        sealed class AlwaysWinEndCondition : IBalloonThrowEndCondition
        {
            public BalloonThrowPhase Evaluate(
                BalloonThrowRules rules,
                int balloonsPopped,
                int throwsRemaining,
                int balloonsRemaining)
            {
                return BalloonThrowPhase.Won;
            }
        }
    }
}
