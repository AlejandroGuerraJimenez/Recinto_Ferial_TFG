using Fairground.Core.Attractions;
using Fairground.ViewModel;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Fairground
{
    public sealed class ParcelEntryTests
    {
        [Test]
        public void Miss_DoesNotChooseAnAttraction()
        {
            Assert.IsFalse(ParcelEntry.TryChoose(false, AttractionId.BalloonThrow, out AttractionId missed));
            Assert.AreEqual(AttractionId.None, missed);

            Assert.IsFalse(ParcelEntry.TryChoose(true, AttractionId.None, out AttractionId empty));
            Assert.AreEqual(AttractionId.None, empty);
        }

        [Test]
        public void Hit_ChoosesTheAimedParcel()
        {
            Assert.IsTrue(ParcelEntry.TryChoose(true, AttractionId.DuckFishing, out AttractionId chosen));
            Assert.AreEqual(AttractionId.DuckFishing, chosen);
        }
    }
}
