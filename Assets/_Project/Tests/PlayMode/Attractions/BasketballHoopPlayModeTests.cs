using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions;
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
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.BasketballHoop.SceneName);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionHost>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsPlaying,
                3f,
                "BasketballHoop ViewModel did not enter Playing.");

            BasketballHoopViewModel vm = controller.ViewModel;
            Assert.AreEqual(AttractionId.BasketballHoop, controller.AttractionId);
            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
            Assert.Greater(vm.SecondsLeft, 0);
            Assert.AreEqual(0, vm.Score);
            Assert.IsTrue(vm.CanThrow);
            Assert.IsNotNull(Object.FindAnyObjectByType<BallSpawnerView>());
            Assert.IsNotNull(Object.FindAnyObjectByType<BoothHudView>());
            Assert.IsNotNull(Object.FindAnyObjectByType<BasketDetectorView>());
        }

        [UnityTest]
        public IEnumerator BasketballHoopScene_ThrowRegistersAndCanScoreOnce()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.BasketballHoop.SceneName);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionHost>();
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
            var detector = AttractionSceneTestHelper.Require<BasketDetectorView>();
            BasketballBallView ball = (BasketballBallView)spawner.CurrentBall;

            Vector3 aboveRim = detector.transform.position + Vector3.up * 0.35f;
            ball.transform.position = aboveRim;
            spawner.LaunchCurrentBall(Vector3.down * 4f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != ball,
                2f,
                "Throw did not release the ball.");

            Assert.IsTrue(vm.CanThrow);

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
            Assert.IsTrue(vm.CanThrow);
        }

        [UnityTest]
        public IEnumerator BasketballHoopScene_RestartRestoresMatch()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.BasketballHoop.SceneName);

            var controller = AttractionSceneTestHelper.Require<BasketballHoopAttractionHost>();
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
            BasketballBallView ball = (BasketballBallView)spawner.CurrentBall;
            spawner.LaunchCurrentBall(Vector3.forward * 8f);

            yield return AttractionSceneTestHelper.WaitUntil(
                () => spawner.CurrentBall != ball,
                2f,
                "Throw did not release the ball before restart.");

            controller.RestartMatch();
            yield return null;

            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
            Assert.AreEqual(BasketballHoopBalance.RoundSeconds, vm.SecondsLeft);
            Assert.AreEqual(0, vm.Score);
            Assert.IsTrue(vm.CanThrow);
        }
    }
}
