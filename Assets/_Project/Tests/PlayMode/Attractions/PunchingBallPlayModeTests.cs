using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.PunchingBall;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions.PunchingBall;
using Fairground.ViewModel.Attractions.PunchingBall;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fairground.Tests.PlayMode.Attractions
{
    public sealed class PunchingBallPlayModeTests
    {
        [UnityTest]
        public IEnumerator PunchingBallScene_LoadsAndStartsPlaying()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.PunchingBall);

            var controller = AttractionSceneTestHelper.Require<PunchingBallAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "PunchingBall ViewModel did not enter Playing.");

            PunchingBallViewModel vm = controller.ViewModel;
            Assert.AreEqual(AttractionId.PunchingBall, controller.AttractionId);
            Assert.AreEqual(PunchingBallPhase.Playing, vm.Phase);
            Assert.Greater(vm.PunchesRemaining, 0);
            Assert.AreEqual(0, vm.BestScore);
            Assert.IsNotNull(Object.FindFirstObjectByType<PunchPadView>());
            Assert.IsNotNull(Object.FindFirstObjectByType<PunchPowerMeterView>());
            Assert.IsNotNull(Object.FindFirstObjectByType<PunchingBallHudView>());
        }

        [UnityTest]
        public IEnumerator PunchingBallScene_StrongPunch_WinsMatch()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.PunchingBall);

            var controller = AttractionSceneTestHelper.Require<PunchingBallAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.CanPunch,
                3f,
                "PunchingBall ViewModel was not ready to punch.");

            var pad = AttractionSceneTestHelper.Require<PunchPadView>();
            PunchingBallViewModel vm = controller.ViewModel;
            int punchesBefore = vm.PunchesRemaining;

            Assert.IsTrue(pad.TryApplyImpact(8f), "Strong impact should be accepted by the pad.");

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.PunchesRemaining == punchesBefore - 1,
                2f,
                "Punch was not registered in the ViewModel.");

            Assert.GreaterOrEqual(vm.BestScore, vm.WinScoreThreshold);
            Assert.AreEqual(PunchingBallPhase.Won, vm.Phase);
            Assert.AreEqual("Strongman!", vm.StatusText);
        }

        [UnityTest]
        public IEnumerator PunchingBallScene_WeakPunches_CanLoseAndRestart()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.PunchingBall);

            var controller = AttractionSceneTestHelper.Require<PunchingBallAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.CanPunch,
                3f,
                "PunchingBall ViewModel was not ready to punch.");

            var pad = AttractionSceneTestHelper.Require<PunchPadView>();
            PunchingBallViewModel vm = controller.ViewModel;

            while (vm.Phase == PunchingBallPhase.Playing)
            {
                int punchesBefore = vm.PunchesRemaining;
                Assert.Greater(punchesBefore, 0, "Match still Playing but no punches remain.");
                Assert.IsTrue(pad.TryApplyImpact(1.5f), "Weak impact should still consume a punch.");

                yield return AttractionSceneTestHelper.WaitUntil(
                    () => vm.PunchesRemaining < punchesBefore || vm.Phase != PunchingBallPhase.Playing,
                    2f,
                    "Pad/ViewModel did not react to weak punch.");

                // Wait out pad cooldown between punches.
                yield return new WaitForSeconds(0.55f);
            }

            Assert.AreEqual(PunchingBallPhase.Lost, vm.Phase);
            Assert.Less(vm.BestScore, vm.WinScoreThreshold);

            controller.RestartMatch();
            yield return null;

            Assert.AreEqual(PunchingBallPhase.Playing, vm.Phase);
            Assert.Greater(vm.PunchesRemaining, 0);
            Assert.AreEqual(0, vm.BestScore);
            Assert.IsTrue(vm.CanPunch);
        }
    }
}
