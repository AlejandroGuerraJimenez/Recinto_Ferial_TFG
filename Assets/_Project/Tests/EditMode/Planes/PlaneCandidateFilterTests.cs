using Fairground.Model;
using NUnit.Framework;

namespace Fairground.Tests.EditMode
{
    public class PlaneCandidateFilterTests
    {
        [Test]
        public void Table_IsAccepted_WithHigherPriorityThanUnlabeledHorizontal()
        {
            var table = Plane("table", PlaneSemanticClassification.Table, PlaneAlignmentKind.Other);
            var unlabeled = Plane("open", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryGetPriority(table, out var tablePriority));
            Assert.IsTrue(PlaneCandidateFilter.TryGetPriority(unlabeled, out var unlabeledPriority));
            Assert.Greater(tablePriority, unlabeledPriority);
        }

        [Test]
        public void UnlabeledHorizontalUp_IsAccepted()
        {
            var plane = Plane("surface", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryGetPriority(plane, out var priority));
            Assert.AreEqual(PlaneCandidateFilter.UnlabeledHorizontalPriority, priority);
        }

        [Test]
        public void UnlabeledNonHorizontal_IsRejected()
        {
            var plane = Plane("wall", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.Other);
            Assert.IsFalse(PlaneCandidateFilter.TryGetPriority(plane, out _));
        }

        [Test]
        public void LabeledNonTable_IsRejected()
        {
            var plane = Plane("floor", PlaneSemanticClassification.Other, PlaneAlignmentKind.HorizontalUp);
            Assert.IsFalse(PlaneCandidateFilter.TryGetPriority(plane, out _));
        }

        [Test]
        public void ShortBoundary_IsRejected()
        {
            Assert.IsFalse(PlaneCandidateFilter.TryGetPriority(Tiny(), out _));
        }

        static DetectedPlane Plane(string id, PlaneSemanticClassification classification, PlaneAlignmentKind alignment)
        {
            return new DetectedPlane(id, PlanePose.Identity, Square(), classification, alignment);
        }

        static DetectedPlane Tiny()
        {
            return new DetectedPlane(
                "tiny",
                PlanePose.Identity,
                new[] { new PlanePoint(0f, 0f), new PlanePoint(1f, 1f) },
                PlaneSemanticClassification.Table,
                PlaneAlignmentKind.HorizontalUp);
        }

        static PlanePoint[] Square()
        {
            return new[]
            {
                new PlanePoint(-1f, -1f),
                new PlanePoint(-1f, 1f),
                new PlanePoint(1f, 1f),
                new PlanePoint(1f, -1f),
            };
        }
    }
}
