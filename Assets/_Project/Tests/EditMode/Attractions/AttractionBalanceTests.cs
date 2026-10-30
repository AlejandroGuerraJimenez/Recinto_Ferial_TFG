using Fairground.Core.Attractions;
using Fairground.ViewModel.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions.FerrisWheel;
using Fairground.ViewModel.Attractions.PunchingBall;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions
{
    public sealed class AttractionBalanceTests
    {
        [Test]
        public void DefaultViewModels_ReadBalance()
        {
            var balloons = new BalloonThrowViewModel();
            Assert.AreEqual(BalloonThrowBalance.BalloonCount, balloons.BalloonCount);
            balloons.StartMatch();
            Assert.IsTrue(balloons.TryRegisterThrow());
            Assert.IsTrue(balloons.TryRegisterPop());
            Assert.AreEqual(BalloonThrowBalance.PointsPerBalloon, balloons.Score);
            Assert.AreEqual(BalloonThrowBalance.StartingThrows - 1, balloons.AttemptsRemaining);

            var ducks = new DuckFishingViewModel();
            Assert.AreEqual(DuckFishingBalance.DuckCount, ducks.DuckCount);
            Assert.AreEqual(DuckFishingBalance.RoundSeconds, ducks.SecondsLeft);
            ducks.StartMatch();
            Assert.IsTrue(ducks.TryRegisterCatch());
            Assert.AreEqual(1, ducks.Score);
            Assert.AreEqual(1, ducks.DucksCaught);

            var hoop = new BasketballHoopViewModel();
            hoop.StartMatch();
            Assert.AreEqual(BasketballHoopBalance.StartingBalls, hoop.AttemptsRemaining);
            Assert.IsTrue(hoop.TryRegisterThrow());
            Assert.IsTrue(hoop.TryRegisterBasket());
            Assert.AreEqual(BasketballHoopBalance.PointsPerBasket, hoop.Score);

            var punch = new PunchingBallViewModel();
            punch.StartMatch();
            Assert.AreEqual(PunchingBallBalance.StartingPunches, punch.PunchesRemaining);
            Assert.AreEqual(PunchingBallBalance.WinScoreThreshold, punch.WinScoreThreshold);
            Assert.AreEqual(PunchingBallBalance.MaxScore, punch.MaxScore);

            var wheel = new FerrisWheelViewModel();
            Assert.AreEqual(FerrisWheelBalance.DegreesPerSecond, wheel.DegreesPerSecond);
        }
    }
}
