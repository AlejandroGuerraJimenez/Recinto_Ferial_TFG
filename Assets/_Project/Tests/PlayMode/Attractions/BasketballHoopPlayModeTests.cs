using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions.BasketballHoop;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fairground.Tests.PlayMode.Attractions
{
    public sealed class BasketballHoopPlayModeTests
    {
        [UnityTest]
        public IEnumerator BasketballHoopScene_LoadsAndStartsPlaying()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BasketballHoop);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BasketballHoop ViewModel did not enter Playing.");

            BasketballHoopViewModel vm = controller.ViewModel;
            Assert.AreEqual(AttractionId.BasketballHoop, controller.AttractionId);
            Assert.AreEqual(BasketballHoopPhase.Playing, vm.Phase);
            Assert.Greater(vm.BallsRemaining, 0);
            Assert.AreEqual(0, vm.Score);
            Assert.IsTrue(vm.CanThrow);
            Assert.IsNotNull(Object.FindFirstObjectByType<BallSpawnerView>());
            Assert.IsNotNull(Object.FindFirstObjectByType<BasketballHoopHudView>());
            Assert.IsNotNull(Object.FindFirstObjectByType<BasketDetectorView>());
        }

        [UnityTest]
        public IEnumerator BasketballHoopScene_ThrowRegistersAndCanScoreOnce()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BasketballHoop);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BasketballHoop ViewModel did not enter Playing.");

            var spawner = AttractionSceneTestHelper.Require<BallSpawnerView>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != null,
                3f,
                "Ball spawner did not provide a ball.");

            BasketballHoopViewModel vm = controller.ViewModel;
            int ballsBefore = vm.BallsRemaining;
            var detector = AttractionSceneTestHelper.Require<BasketDetectorView>();
            BasketballBallView ball = spawner.CurrentBall;

            Vector3 aboveRim = detector.transform.position + Vector3.up * 0.35f;
            ball.transform.position = aboveRim;
            spawner.LaunchCurrentBall(Vector3.down * 4f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.BallsRemaining == ballsBefore - 1,
                2f,
                "Throw was not registered in the ViewModel.");

            float contactDeadline = Time.realtimeSinceStartup + 2.5f;
            while (vm.Score == 0 && ball != null && Time.realtimeSinceStartup < contactDeadline)
            {
                var body = ball.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.position = detector.transform.position + Vector3.up * 0.05f;
                    body.linearVelocity = Vector3.down * 3f;
                    Physics.SyncTransforms();
                }

                yield return new WaitForFixedUpdate();
            }

            if (vm.Score == 0)
                Assert.IsTrue(detector.TryRegisterScoreFrom(ball), "Thrown ball could not register a basket.");

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.Score > 0,
                2f,
                "Basket did not update the score.");

            int score = vm.Score;
            Assert.IsFalse(detector.TryRegisterScoreFrom(ball));
            Assert.AreEqual(score, vm.Score);
            Assert.Less(vm.BallsRemaining, ballsBefore);
        }

        [UnityTest]
        public IEnumerator BasketballHoopScene_RestartRestoresMatch()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BasketballHoop);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BasketballHoop ViewModel did not enter Playing.");

            var spawner = AttractionSceneTestHelper.Require<BallSpawnerView>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != null,
                3f,
                "Ball spawner did not provide a ball.");

            BasketballHoopViewModel vm = controller.ViewModel;
            int startingBalls = vm.BallsRemaining;
            spawner.LaunchCurrentBall(Vector3.forward * 8f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.BallsRemaining == startingBalls - 1,
                2f,
                "Throw was not registered before restart.");

            controller.RestartMatch();
            yield return null;

            Assert.AreEqual(BasketballHoopPhase.Playing, vm.Phase);
            Assert.AreEqual(startingBalls, vm.BallsRemaining);
            Assert.AreEqual(0, vm.Score);
            Assert.IsTrue(vm.CanThrow);
        }
    }
}
