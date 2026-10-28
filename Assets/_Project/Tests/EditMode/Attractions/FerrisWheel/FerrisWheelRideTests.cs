using Fairground.Core.Attractions;
using Fairground.Model.Attractions.FerrisWheel;
using Fairground.ViewModel.Attractions.FerrisWheel;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.FerrisWheel
{
    public sealed class FerrisWheelRideTests
    {
        [Test]
        public void AttractionScenes_MapsFerrisWheelId()
        {
            Assert.AreEqual(4, (int)AttractionId.FerrisWheel);
            Assert.AreEqual("FerrisWheel", AttractionScenes.GetSceneName(AttractionId.FerrisWheel));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/FerrisWheel.unity",
                AttractionScenes.GetScenePath(AttractionId.FerrisWheel));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("FerrisWheel", out var id));
            Assert.AreEqual(AttractionId.FerrisWheel, id);
        }

        [Test]
        public void Motion_WrapsPastFullCircle()
        {
            Assert.AreEqual(10f, FerrisWheelMotion.Advance(350f, 20f, 1f), 0.001f);
        }

        [Test]
        public void Motion_WrapsNegativeAngles()
        {
            Assert.AreEqual(350f, FerrisWheelMotion.Wrap(-10f), 0.001f);
        }

        [Test]
        public void Motion_IgnoresNonPositiveDelta()
        {
            Assert.AreEqual(40f, FerrisWheelMotion.Advance(40f, 16f, 0f), 0.001f);
            Assert.AreEqual(40f, FerrisWheelMotion.Advance(40f, 16f, -1f), 0.001f);
        }

        [Test]
        public void Ride_DoesNotSpinUntilStarted()
        {
            var session = new FerrisWheelRide(new FerrisWheelRules());
            session.Advance(1f);
            Assert.AreEqual(FerrisWheelPhase.NotStarted, session.Phase);
            Assert.AreEqual(0f, session.AngleDegrees, 0.001f);
        }

        [Test]
        public void Ride_AdvancesWhileRiding()
        {
            var session = new FerrisWheelRide(new FerrisWheelRules(16f));
            session.Start();
            session.Advance(0.5f);

            Assert.AreEqual(FerrisWheelPhase.Riding, session.Phase);
            Assert.AreEqual(8f, session.AngleDegrees, 0.001f);
        }

        [Test]
        public void Rules_RejectNonPositiveSpeed()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new FerrisWheelRules(0f));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new FerrisWheelRules(-1f));
        }

        [Test]
        public void ViewModel_StartsRidingAndReportsAttraction()
        {
            var vm = new FerrisWheelViewModel();
            Assert.AreEqual(FerrisWheelPhase.NotStarted, vm.Phase);
            Assert.IsFalse(vm.IsRiding);

            vm.StartRide();
            vm.Advance(1f);

            Assert.AreEqual(AttractionId.FerrisWheel, vm.AttractionId);
            Assert.AreEqual("FerrisWheel", AttractionCatalog.GetSceneName(vm.AttractionId));
            Assert.AreEqual(FerrisWheelPhase.Riding, vm.Phase);
            Assert.AreEqual(FerrisWheelRules.DefaultDegreesPerSecond, vm.AngleDegrees, 0.001f);
        }

        [Test]
        public void ViewModel_AdvanceBeforeStartDoesNothing()
        {
            var vm = new FerrisWheelViewModel(20f);
            vm.Advance(2f);
            Assert.AreEqual(0f, vm.AngleDegrees, 0.001f);
            Assert.AreEqual(FerrisWheelPhase.NotStarted, vm.Phase);
        }
    }
}
