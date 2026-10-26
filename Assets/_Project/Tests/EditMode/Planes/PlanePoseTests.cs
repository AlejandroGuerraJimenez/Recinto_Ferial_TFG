using Fairground.Model;
using NUnit.Framework;

namespace Fairground.Tests.EditMode
{
    public class PlanePoseTests
    {
        [Test]
        public void Identity_FacesUp()
        {
            Assert.IsTrue(PlanePose.Identity.FacesUp());
        }

        [Test]
        public void SlightTilt_FacesUp()
        {
            Assert.IsTrue(TiltedAroundX(20f).FacesUp());
        }

        [Test]
        public void SteepTilt_StillFacesUp()
        {
            Assert.IsTrue(TiltedAroundX(50f).FacesUp());
        }

        [Test]
        public void NearVertical_DoesNotFaceUp()
        {
            Assert.IsFalse(TiltedAroundX(70f).FacesUp());
        }

        [Test]
        public void Wall_DoesNotFaceUp()
        {
            Assert.IsFalse(TiltedAroundX(90f).FacesUp());
        }

        [Test]
        public void Ceiling_DoesNotFaceUp()
        {
            Assert.IsFalse(TiltedAroundX(180f).FacesUp());
        }

        static PlanePose TiltedAroundX(float degrees)
        {
            var halfRadians = degrees * System.Math.PI / 180d * 0.5d;
            var sine = (float)System.Math.Sin(halfRadians);
            var cosine = (float)System.Math.Cos(halfRadians);
            return new PlanePose(0f, 0f, 0f, sine, 0f, 0f, cosine);
        }
    }
}
