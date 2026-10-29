using System.Collections;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.FerrisWheel;
using Fairground.Tests.PlayMode.Support;
using Fairground.View.Attractions.FerrisWheel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fairground.Tests.PlayMode.Attractions
{
    public sealed class FerrisWheelPlayModeTests
    {
        [UnityTest]
        public IEnumerator FerrisWheelScene_SpinsAndKeepsCabinsUpright()
        {
            yield return AttractionSceneTestHelper.LoadAttractionScene(AttractionCatalog.FerrisWheel.SceneName);

            var controller = AttractionSceneTestHelper.Require<FerrisWheelAttractionHost>();
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel != null && controller.ViewModel.IsRiding,
                3f,
                "FerrisWheel ViewModel did not enter Riding.");

            var rig = AttractionSceneTestHelper.Require<FerrisWheelRigView>();
            Assert.AreEqual(AttractionId.FerrisWheel, controller.AttractionId);
            Assert.AreEqual(FerrisWheelPhase.Riding, controller.ViewModel.Phase);
            Assert.AreEqual(FerrisWheelSiteLayout.CabinCount, rig.CabinCount);
            Assert.IsNotNull(rig.Wheel);

            float startAngle = controller.ViewModel.AngleDegrees;
            yield return AttractionSceneTestHelper.WaitUntil(
                () => controller.ViewModel.AngleDegrees > startAngle + 8f,
                3f,
                "Ferris wheel angle did not advance.");

            float wheelZ = rig.Wheel.localEulerAngles.z;
            float delta = Mathf.DeltaAngle(controller.ViewModel.AngleDegrees, wheelZ);
            Assert.Less(Mathf.Abs(delta), 1f);
            Assert.Greater(Vector3.Dot(rig.GetCabin(0).up, Vector3.up), 0.98f);
        }
    }
}
