using Fairground.Core.Attractions;
using Fairground.Core.Planes;
using Fairground.ViewModel;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Fairground
{
    public sealed class FairgroundVisitStateTests
    {
        [Test]
        public void Remember_StaysOnThatInstance()
        {
            var first = new FairgroundVisitState();
            var second = new FairgroundVisitState();

            first.Remember(Plane("table"), AttractionId.BalloonThrow);

            Assert.AreEqual("table", first.SelectedPlane?.Id);
            Assert.AreEqual(AttractionId.BalloonThrow, first.PendingAttraction);
            Assert.IsFalse(second.SelectedPlane.HasValue);
            Assert.AreEqual(AttractionId.None, second.PendingAttraction);
        }

        [Test]
        public void RememberPlane_ClearsThePendingAttraction()
        {
            var session = new FairgroundVisitState();
            session.Remember(Plane("floor"), AttractionId.DuckFishing);

            session.Remember(Plane("floor"));

            Assert.AreEqual("floor", session.SelectedPlane?.Id);
            Assert.AreEqual(AttractionId.None, session.PendingAttraction);
        }

        [Test]
        public void LeaveAttraction_KeepsTheFloor()
        {
            var session = new FairgroundVisitState();
            session.Remember(Plane("floor"), AttractionId.FerrisWheel);

            session.LeaveAttraction();

            Assert.AreEqual("floor", session.SelectedPlane?.Id);
            Assert.AreEqual(AttractionId.None, session.PendingAttraction);
        }

        [Test]
        public void Clear_DropsFloorAndAttraction()
        {
            var session = new FairgroundVisitState();
            session.Remember(Plane("floor"), AttractionId.BasketballHoop);

            session.Clear();

            Assert.IsFalse(session.SelectedPlane.HasValue);
            Assert.AreEqual(AttractionId.None, session.PendingAttraction);
        }

        static DetectedPlane Plane(string id)
        {
            return new DetectedPlane(
                id,
                PlanePose.Identity,
                System.Array.Empty<PlanePoint>(),
                PlaneSemanticClassification.Floor,
                PlaneAlignmentKind.HorizontalUp);
        }
    }
}
