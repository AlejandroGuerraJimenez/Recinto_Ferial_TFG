using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions.BalloonThrow;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fairground.Tests.PlayMode.Attractions
{
    public sealed class BalloonThrowPlayModeTests
    {
        [UnityTest]
        public IEnumerator BalloonThrowScene_LoadsAndStartsPlaying()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BalloonThrow);

            var controller = AttractionSceneTestHelper.Require<BalloonThrowAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BalloonThrow ViewModel did not enter Playing.");

            BalloonThrowViewModel vm = controller.ViewModel;
            Assert.AreEqual(AttractionId.BalloonThrow, controller.AttractionId);
            Assert.AreEqual(BalloonThrowPhase.Playing, vm.Phase);
            Assert.Greater(vm.ThrowsRemaining, 0);
            Assert.Greater(vm.BalloonsRemaining, 0);
            Assert.IsNotNull(Object.FindFirstObjectByType<BallSpawnerView>());
            Assert.IsNotNull(Object.FindFirstObjectByType<BalloonThrowHudView>());
            Assert.Greater(Object.FindObjectsByType<BalloonTargetView>(FindObjectsSortMode.None).Length, 0);
        }

        [UnityTest]
        public IEnumerator BalloonThrowScene_ThrowRegistersAndCanPopBalloon()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BalloonThrow);

            var controller = AttractionSceneTestHelper.Require<BalloonThrowAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BalloonThrow ViewModel did not enter Playing.");

            var spawner = AttractionSceneTestHelper.Require<BallSpawnerView>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != null,
                3f,
                "Ball spawner did not provide a ball.");

            BalloonThrowViewModel vm = controller.ViewModel;
            int throwsBefore = vm.ThrowsRemaining;
            int balloonsBefore = vm.BalloonsRemaining;
            var balloon = AttractionSceneTestHelper.Require<BalloonTargetView>();

            ThrowableBallView ball = spawner.CurrentBall;
            Vector3 target = balloon.transform.position;
            ball.transform.position = target + Vector3.back * 0.8f;
            Vector3 toTarget = (target - ball.transform.position).normalized;
            spawner.LaunchCurrentBall(toTarget * 12f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.ThrowsRemaining == throwsBefore - 1,
                2f,
                "Throw was not registered in the ViewModel.");

            // Prefer physics contact; fall back to explicit hit registration if timing is flaky.
            var body = ball.GetComponent<Rigidbody>();
            float contactDeadline = Time.realtimeSinceStartup + 2.5f;
            while (vm.Score == 0 && Time.realtimeSinceStartup < contactDeadline)
            {
                if (ball != null && body != null)
                {
                    Vector3 next = Vector3.MoveTowards(body.position, target, 0.35f);
                    body.MovePosition(next);
                    body.linearVelocity = (target - next).normalized * 8f;
                    Physics.SyncTransforms();
                }

                yield return new WaitForFixedUpdate();
            }

            if (vm.Score == 0)
                Assert.IsTrue(balloon.TryRegisterHitFrom(ball), "Thrown ball could not register a balloon hit.");

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.Score > 0 && vm.BalloonsRemaining < balloonsBefore,
                2f,
                "Balloon hit did not update score/balloon counters.");

            Assert.Greater(vm.Score, 0);
            Assert.Less(vm.BalloonsRemaining, balloonsBefore);
        }

        [UnityTest]
        public IEnumerator BalloonThrowScene_RestartRestoresMatch()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionScenes.BalloonThrow);

            var controller = AttractionSceneTestHelper.Require<BalloonThrowAttractionController>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BalloonThrow ViewModel did not enter Playing.");

            var spawner = AttractionSceneTestHelper.Require<BallSpawnerView>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != null,
                3f,
                "Ball spawner did not provide a ball.");

            BalloonThrowViewModel vm = controller.ViewModel;
            int startingThrows = vm.ThrowsRemaining;
            spawner.LaunchCurrentBall(Vector3.forward * 8f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => vm.ThrowsRemaining == startingThrows - 1,
                2f,
                "Throw was not registered before restart.");

            controller.RestartMatch();
            yield return null;

            Assert.AreEqual(BalloonThrowPhase.Playing, vm.Phase);
            Assert.AreEqual(startingThrows, vm.ThrowsRemaining);
            Assert.AreEqual(0, vm.Score);
            Assert.Greater(vm.BalloonsRemaining, 0);
        }
    }
}
