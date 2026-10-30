using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions.BasketballHoop;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions.BasketballHoop
{
    public sealed class BasketballHoopMatchTests
    {
        [Test]
        public void AttractionScenes_MapsBasketballHoopId()
        {
            Assert.AreEqual(5, (int)AttractionId.BasketballHoop);
            Assert.AreEqual("BasketballHoop", AttractionScenes.GetSceneName(AttractionId.BasketballHoop));
            Assert.AreEqual(
                "Assets/_Project/Scenes/Attractions/BasketballHoop.unity",
                AttractionScenes.GetScenePath(AttractionId.BasketballHoop));
            Assert.IsTrue(AttractionScenes.TryGetAttractionId("BasketballHoop", out var id));
            Assert.AreEqual(AttractionId.BasketballHoop, id);
        }

        [Test]
        public void Match_Start_OpensTheClock()
        {
            var session = new BasketballHoopMatch(new BasketballHoopRules(2, 40));
            session.Start();
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(40f, session.SecondsRemaining);
        }

        [Test]
        public void Match_Basket_AddsPointsWithoutEndingTheRound()
        {
            var session = new BasketballHoopMatch(new BasketballHoopRules(2, 40));
            session.Start();
            Assert.IsTrue(session.TryRegisterBasket());
            Assert.IsTrue(session.TryRegisterBasket());
            Assert.AreEqual(4, session.Score);
            Assert.AreEqual(MatchPhase.Playing, session.Phase);
        }

        [Test]
        public void Match_TimeUp_LocksTheScore()
        {
            var session = new BasketballHoopMatch(new BasketballHoopRules(2, 10));
            session.Start();
            Assert.IsTrue(session.TryRegisterBasket());
            session.Tick(10f);
            Assert.AreEqual(0f, session.SecondsRemaining);
            Assert.AreEqual(MatchPhase.Won, session.Phase);
            Assert.AreEqual(2, session.Score);
            Assert.IsFalse(session.TryRegisterBasket());
        }

        [Test]
        public void Match_TimeUpWithNoBaskets_EndsWithoutPoints()
        {
            var session = new BasketballHoopMatch(new BasketballHoopRules(2, 5));
            session.Start();
            session.Tick(5f);
            Assert.AreEqual(MatchPhase.Lost, session.Phase);
            Assert.AreEqual(0, session.Score);
        }

        [Test]
        public void ViewModel_ThrowDoesNotSpendTheRound()
        {
            var vm = new BasketballHoopViewModel(new BasketballHoopRules(2, 40));
            int events = 0;
            vm.StateChanged += () => events++;
            vm.StartMatch();
            Assert.IsTrue(vm.TryRegisterThrow());
            Assert.IsTrue(vm.CanThrow);
            Assert.IsTrue(vm.TryRegisterBasket());
            Assert.AreEqual(2, events);
            Assert.AreEqual(2, vm.Score);
            Assert.AreEqual(40, vm.SecondsLeft);
            Assert.AreEqual("¡A la canasta!", BasketballHoopHud.Status(vm));
            Assert.AreEqual(AttractionId.BasketballHoop, vm.AttractionId);
        }

        [Test]
        public void ViewModel_Tick_EndsTheRoundAndStopsThrows()
        {
            var vm = new BasketballHoopViewModel(2, 8);
            vm.StartMatch();
            vm.Tick(8f);
            Assert.AreEqual(0, vm.SecondsLeft);
            Assert.IsFalse(vm.CanThrow);
            Assert.IsFalse(vm.TryRegisterThrow());
            Assert.IsFalse(vm.TryRegisterBasket());
            Assert.AreEqual("¡Tiempo!", BasketballHoopHud.Status(vm));
            vm.Restart();
            Assert.IsTrue(vm.CanThrow);
            Assert.AreEqual(8, vm.SecondsLeft);
            Assert.AreEqual(0, vm.Score);
            Assert.AreEqual(MatchPhase.Playing, vm.Phase);
        }
    }
}
