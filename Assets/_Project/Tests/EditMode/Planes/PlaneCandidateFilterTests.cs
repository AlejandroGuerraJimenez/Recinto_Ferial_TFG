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
        public void VerticalLabels_AreRejected()
        {
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("wall", PlaneSemanticClassification.Wall, PlaneAlignmentKind.Other), out _));
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("ceiling", PlaneSemanticClassification.Ceiling, PlaneAlignmentKind.Other), out _));
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("door", PlaneSemanticClassification.Door, PlaneAlignmentKind.Other), out _));
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("window", PlaneSemanticClassification.Window, PlaneAlignmentKind.Other), out _));
        }

        [Test]
        public void NonHorizontal_IsRejected()
        {
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("floor", PlaneSemanticClassification.Floor, PlaneAlignmentKind.Other), out _));
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Plane("open", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.Other), out _));
        }

        [Test]
        public void DownwardPose_IsRejected_EvenWhenFlaggedHorizontal()
        {
            var ceiling = new DetectedPlane(
                "ceiling",
                TiltedAroundX(180f),
                Square(),
                PlaneSemanticClassification.Ceiling,
                PlaneAlignmentKind.HorizontalUp);
            Assert.IsFalse(PlaneCandidateFilter.TryRank(ceiling, out _));
        }

        [Test]
        public void ShortBoundary_IsRejected()
        {
            Assert.IsFalse(PlaneCandidateFilter.TryRank(Tiny(), out _));
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

        static PlanePose TiltedAroundX(float degrees)
        {
            var halfRadians = degrees * System.Math.PI / 180d * 0.5d;
            var sine = (float)System.Math.Sin(halfRadians);
            var cosine = (float)System.Math.Cos(halfRadians);
            return new PlanePose(0f, 0f, 0f, sine, 0f, 0f, cosine);
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
