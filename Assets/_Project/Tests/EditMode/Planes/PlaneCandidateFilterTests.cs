using Fairground.Model;
using NUnit.Framework;

namespace Fairground.Tests.EditMode
{
    public class PlaneCandidateFilterTests
    {
        [Test]
        public void Floor_OutranksTable_AndTableOutranksOpenSurface()
        {
            var floor = Plane("floor", PlaneSemanticClassification.Floor, PlaneAlignmentKind.HorizontalUp);
            var table = Plane("table", PlaneSemanticClassification.Table, PlaneAlignmentKind.HorizontalUp);
            var open = Plane("open", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryRank(floor, out var floorRank));
            Assert.IsTrue(PlaneCandidateFilter.TryRank(table, out var tableRank));
            Assert.IsTrue(PlaneCandidateFilter.TryRank(open, out var openRank));
            Assert.Greater(floorRank.CompareTo(tableRank), 0);
            Assert.Greater(tableRank.CompareTo(openRank), 0);
        }

        [Test]
        public void LargerFloor_OutranksSmallerFloor()
        {
            var small = Plane("small", PlaneSemanticClassification.Floor, PlaneAlignmentKind.HorizontalUp);
            var large = new DetectedPlane("large", PlanePose.Identity, Wide(), PlaneSemanticClassification.Floor, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryRank(small, out var smallRank));
            Assert.IsTrue(PlaneCandidateFilter.TryRank(large, out var largeRank));
            Assert.Greater(largeRank.CompareTo(smallRank), 0);
        }

        [Test]
        public void SmallFloor_OutranksLargeTable()
        {
            var floor = Plane("floor", PlaneSemanticClassification.Floor, PlaneAlignmentKind.HorizontalUp);
            var table = new DetectedPlane("table", PlanePose.Identity, Wide(), PlaneSemanticClassification.Table, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryRank(floor, out var floorRank));
            Assert.IsTrue(PlaneCandidateFilter.TryRank(table, out var tableRank));
            Assert.Greater(floorRank.CompareTo(tableRank), 0);
        }

        [Test]
        public void StructuralLabels_AreAccepted_AndRankBelowFloor()
        {
            var floor = Plane("floor", PlaneSemanticClassification.Floor, PlaneAlignmentKind.HorizontalUp);
            Assert.IsTrue(PlaneCandidateFilter.TryRank(floor, out var floorRank));
            AssertAcceptedBelow(floorRank, Plane("wall", PlaneSemanticClassification.Wall, PlaneAlignmentKind.Other));
            AssertAcceptedBelow(floorRank, Plane("ceiling", PlaneSemanticClassification.Ceiling, PlaneAlignmentKind.Other));
            AssertAcceptedBelow(floorRank, Plane("door", PlaneSemanticClassification.Door, PlaneAlignmentKind.Other));
            AssertAcceptedBelow(floorRank, Plane("window", PlaneSemanticClassification.Window, PlaneAlignmentKind.Other));
        }

        [Test]
        public void NonHorizontal_IsAccepted()
        {
            Assert.IsTrue(PlaneCandidateFilter.TryRank(Plane("floor", PlaneSemanticClassification.Floor, PlaneAlignmentKind.Other), out _));
            Assert.IsTrue(PlaneCandidateFilter.TryRank(Plane("open", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.Other), out _));
        }

        [Test]
        public void ShortBoundary_IsRejected()
        {
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Tiny(), out _));
        }

        static void AssertAcceptedBelow(PlaneRank floorRank, DetectedPlane plane)
        {
            Assert.IsTrue(PlaneCandidateFilter.TryRank(plane, out var rank));
            Assert.Greater(floorRank.CompareTo(rank), 0);
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

        static PlanePoint[] Wide()
        {
            return new[]
            {
                new PlanePoint(-4f, -4f),
                new PlanePoint(-4f, 4f),
                new PlanePoint(4f, 4f),
                new PlanePoint(4f, -4f),
            };
        }
    }
}
