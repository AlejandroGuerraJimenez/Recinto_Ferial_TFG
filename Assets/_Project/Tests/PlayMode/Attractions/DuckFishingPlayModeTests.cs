using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions.DuckFishing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fairground.Tests.PlayMode.Attractions
{
    public sealed class DuckFishingPlayModeTests
    {
        [UnityTest]
        public IEnumerator DuckFishingScene_LoadsAndStartsPlaying()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.DuckFishing.SceneName);

            var host = AttractionSceneTestHelper.Require<DuckFishingAttractionHost>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => host.ViewModel != null && host.ViewModel.IsPlaying,
                3f,
                "DuckFishing ViewModel did not enter Playing.");

            DuckFishingViewModel viewModel = host.ViewModel;
            Assert.AreEqual(AttractionId.DuckFishing, host.AttractionId);
            Assert.AreEqual(MatchPhase.Playing, viewModel.Phase);
            Assert.Greater(viewModel.AttemptsRemaining, 0);
            Assert.AreEqual(DuckFishingBalance.DuckCount, viewModel.DuckCount);
            Assert.Greater(Object.FindObjectsByType<DuckTargetView>().Length, 0);
        }

        [UnityTest]
        public IEnumerator DuckFishingScene_CatchScoresAndRestartRestores()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.DuckFishing.SceneName);

            var host = AttractionSceneTestHelper.Require<DuckFishingAttractionHost>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => host.ViewModel != null && host.ViewModel.IsPlaying,
                3f,
                "DuckFishing ViewModel did not enter Playing.");

            DuckFishingViewModel viewModel = host.ViewModel;
            int attempts = viewModel.AttemptsRemaining;
            var duck = AttractionSceneTestHelper.Require<DuckTargetView>();
            var hook = AttractionSceneTestHelper.Require<FishingHookView>();

            Assert.IsTrue(hook.TryStartDip());
            Assert.IsTrue(hook.TryCatch(duck));

            yield return AttractionSceneTestHelper.WaitUntil(
                () => viewModel.Score == DuckFishingBalance.PointsPerDuck
                    && viewModel.AttemptsRemaining == attempts - 1,
                2f,
                "Duck catch did not update the ViewModel.");

            host.RestartMatch();
            yield return null;

            Assert.AreEqual(MatchPhase.Playing, viewModel.Phase);
            Assert.AreEqual(attempts, viewModel.AttemptsRemaining);
            Assert.AreEqual(0, viewModel.Score);
            Assert.AreEqual(0, viewModel.DucksCaught);
        }
    }
}
