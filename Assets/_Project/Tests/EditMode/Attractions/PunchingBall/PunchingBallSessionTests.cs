using Fairground.Core.Attractions;
using Fairground.Model.Attractions.PunchingBall;
using Fairground.ViewModel.Attractions.PunchingBall;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.PunchingBall
{
    public sealed class PunchingBallSessionTests
    {
        [Test]
        public void AttractionScenes_MapsPunchingBallId()
        {
            Assert.AreEqual("PunchingBall", AttractionScenes.GetSceneName(AttractionId.PunchingBall));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/PunchingBall.unity",
                AttractionScenes.GetScenePath(AttractionId.PunchingBall));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("PunchingBall", out var id));
            Assert.AreEqual(AttractionId.PunchingBall, id);
        }

        [Test]
        public void Session_Start_InitializesCounters()
        {
            var session = CreateSession(5, 700, 999, 8f);
            session.Start();
            Assert.AreEqual(PunchingBallPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(0, session.BestScore);
            Assert.AreEqual(5, session.PunchesRemaining);
        }

        [Test]
        public void Session_StrongPunch_CanWin()
        {
            var session = CreateSession(3, 500, 999, 8f);
            session.Start();
            Assert.IsTrue(session.TryRegisterPunch(8f));
            Assert.AreEqual(999, session.LastPunchPower);
            Assert.AreEqual(PunchingBallPhase.Won, session.Phase);
        }

        [Test]
        public void Session_WeakPunchesExhausted_Loses()
        {
            var session = CreateSession(2, 700, 999, 8f);
            session.Start();
            Assert.IsTrue(session.TryRegisterPunch(2f));
            Assert.AreEqual(PunchingBallPhase.Playing, session.Phase);
            Assert.IsTrue(session.TryRegisterPunch(2f));
            Assert.AreEqual(PunchingBallPhase.Lost, session.Phase);
            Assert.IsFalse(session.TryRegisterPunch(8f));
        }

        [Test]
        public void Session_KeepsBestScoreAcrossPunches()
        {
            var session = CreateSession(3, 950, 999, 8f);
            session.Start();
            Assert.IsTrue(session.TryRegisterPunch(4f));
            int first = session.BestScore;
            Assert.IsTrue(session.TryRegisterPunch(2f));
            Assert.AreEqual(first, session.BestScore);
            Assert.AreEqual(first, session.Score);
            Assert.Less(session.LastPunchPower, session.BestScore);
        }

        [Test]
        public void Rules_ScoreForImpact_ClampsToMax()
        {
            var rules = new PunchingBallRules(5, 700, 999, 8f);
            Assert.AreEqual(0, rules.ScoreForImpact(0f));
            Assert.AreEqual(999, rules.ScoreForImpact(20f));
            Assert.AreEqual(500, rules.ScoreForImpact(4f));
        }

        [Test]
        public void EndCondition_Strategy_CanForceWin()
        {
            var session = new PunchingBallSession(new PunchingBallRules(5, 700, 999, 8f), new AlwaysWinEndCondition());
            session.Start();
            session.TryRegisterPunch(1f);
            Assert.AreEqual(PunchingBallPhase.Won, session.Phase);
        }

        [Test]
        public void ViewModel_NotifiesStateChanges()
        {
            var vm = new PunchingBallViewModel(new PunchingBallRules(3, 700, 999, 8f));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartGame();
            Assert.IsTrue(vm.NotifyPunch(4f));
            Assert.AreEqual(2, events);
            Assert.AreEqual("Hit 700+ to win!", vm.StatusText);
            Assert.AreEqual(AttractionId.PunchingBall, vm.AttractionId);
        }

        [Test]
        public void ViewModel_Restart_ResetsMatch()
        {
            var vm = new PunchingBallViewModel(2, 700, 999, 8f);
            vm.StartGame();
            Assert.IsTrue(vm.NotifyPunch(3f));
            Assert.IsTrue(vm.CanPunch);
            Assert.AreEqual(1, vm.PunchesRemaining);
            vm.Restart();
            Assert.IsTrue(vm.CanPunch);
            Assert.AreEqual(2, vm.PunchesRemaining);
            Assert.AreEqual(0, vm.Score);
        }

        static PunchingBallSession CreateSession(int punches, int winThreshold, int maxScore, float maxImpact)
        {
            return new PunchingBallSession(new PunchingBallRules(punches, winThreshold, maxScore, maxImpact));
        }

        sealed class AlwaysWinEndCondition : IPunchingBallEndCondition
        {
            public PunchingBallPhase Evaluate(PunchingBallRules rules, int bestScore, int punchesRemaining)
            {
                return PunchingBallPhase.Won;
            }
        }
    }
}
