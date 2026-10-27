using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Grab setup for objects that must stay on the controller until they are thrown.
    /// The rig's far interactor otherwise keeps the object at the ray hit.
    /// </summary>
    public static class HeldThrowGrab
    {
        public static void Apply(XRGrabInteractable grab)
        {
            if (grab == null)
                return;

            grab.farAttachMode = InteractableFarAttachMode.Near;
            grab.useDynamicAttach = false;
            grab.matchAttachPosition = true;
            grab.matchAttachRotation = true;
            grab.snapToColliderVolume = false;
            grab.attachEaseInTime = 0.1f;
            grab.movementType = XRBaseInteractable.MovementType.Instantaneous;
            grab.trackPosition = true;
            grab.trackRotation = true;
            grab.smoothPosition = false;
            grab.smoothRotation = false;
            // Release speed is applied by HeldThrowRelease. The toolkit throw uses the
            // last frames of the swing, which are already slow when the grip opens.
            grab.throwOnDetach = false;
            grab.forceGravityOnDetach = true;
        }
    }
}
