using Fairground.Model;
using Fairground.ViewModel;
using NUnit.Framework;

namespace Fairground.Tests.EditMode
{
    public class PlaneSelectionViewModelTests
    {
        [Test]
        public void SelectPlane_ConfirmsCandidate_AndRaisesEvent()
        {
            var received = new Tally();
            var viewModel = Listening(received);
            var selected = viewModel.SelectPlane(Table());
            AssertConfirmed(viewModel, selected, received.Value);
        }

        [Test]
        public void SelectPlane_RejectsSecondConfirm_AndNonCandidates()
        {
            AssertSecondConfirmIsIgnored();
            AssertFloorIsRejected();
        }

        [Test]
        public void RestartSelection_ReturnsToAwaiting_AndAllowsAnotherConfirm()
        {
            var states = new Tally();
            var viewModel = Counting(states);
            viewModel.SelectPlane(Table());
            viewModel.RestartSelection();
            AssertRestarted(viewModel, states.Value);
        }

        static PlaneSelectionViewModel Listening(Tally received)
        {
            var viewModel = new PlaneSelectionViewModel();
            viewModel.OnPlaneSelected += _ => received.Add();
            return viewModel;
        }

        static void AssertConfirmed(PlaneSelectionViewModel viewModel, bool selected, int received)
        {
            Assert.IsTrue(selected);
            Assert.AreEqual(PlaneSelectionState.PlaneSelected, viewModel.State);
            Assert.AreEqual("table", viewModel.SelectedPlane?.Id);
            Assert.AreEqual(1, received);
        }

        static void AssertSecondConfirmIsIgnored()
        {
            var viewModel = new PlaneSelectionViewModel();
            var received = 0;
            viewModel.OnPlaneSelected += _ => received++;
            Assert.IsTrue(viewModel.SelectPlane(Table()));
            Assert.IsFalse(viewModel.SelectPlane(Table()));
            Assert.AreEqual(1, received);
        }

        static void AssertFloorIsRejected()
        {
            var fresh = new PlaneSelectionViewModel();
            Assert.IsFalse(fresh.SelectPlane(Floor()));
            Assert.AreEqual(PlaneSelectionState.AwaitingSelection, fresh.State);
            Assert.IsFalse(fresh.SelectedPlane.HasValue);
        }

        static PlaneSelectionViewModel Counting(Tally states)
        {
            var viewModel = new PlaneSelectionViewModel();
            viewModel.StateChanged += _ => states.Add();
            return viewModel;
        }

        static void AssertRestarted(PlaneSelectionViewModel viewModel, int states)
        {
            Assert.AreEqual(PlaneSelectionState.AwaitingSelection, viewModel.State);
            Assert.IsFalse(viewModel.SelectedPlane.HasValue);
            Assert.AreEqual(2, states);
            Assert.IsTrue(viewModel.SelectPlane(UnlabeledHorizontal()));
            Assert.AreEqual("open", viewModel.SelectedPlane?.Id);
        }

        static DetectedPlane Table() => Surface("table", PlaneSemanticClassification.Table, PlaneAlignmentKind.HorizontalUp);

        static DetectedPlane UnlabeledHorizontal() => Surface("open", PlaneSemanticClassification.Unknown, PlaneAlignmentKind.HorizontalUp);

        static DetectedPlane Floor() => Surface("floor", PlaneSemanticClassification.Other, PlaneAlignmentKind.HorizontalUp);

        static DetectedPlane Surface(string id, PlaneSemanticClassification classification, PlaneAlignmentKind alignment)
        {
            return new DetectedPlane(id, PlanePose.Identity, Triangle(), classification, alignment);
        }

        static PlanePoint[] Triangle()
        {
            return new[]
            {
                new PlanePoint(0f, 0f),
                new PlanePoint(0f, 1f),
                new PlanePoint(1f, 1f),
            };
        }

        sealed class Tally
        {
            public int Value { get; private set; }

            public void Add() => Value++;
        }
    }
}
