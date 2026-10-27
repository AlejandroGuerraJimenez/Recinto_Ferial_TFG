using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Once the headset reports a pose, stands the player at the origin looking toward +Z,
    /// where the booths are authored.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AttractionSpawnAlign : MonoBehaviour
    {
        const int MaxWaitFrames = 45;

        Transform _head;
        Vector3 _initialLocal;
        int _frames;

        void Awake()
        {
            Camera camera = GetComponentInChildren<Camera>();
            _head = camera != null ? camera.transform : null;
            if (_head != null)
                _initialLocal = _head.localPosition;
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if (!AttractionDesktopPlay.IsLiveHeadset())
            {
                enabled = false;
                return;
            }
#endif
            if (_head == null)
            {
                enabled = false;
                return;
            }

            _frames++;
            if (!HeadPoseArrived() && _frames < MaxWaitFrames)
                return;

            AlignToBooth();
            enabled = false;
        }

        bool HeadPoseArrived()
        {
            if ((_head.localPosition - _initialLocal).sqrMagnitude > 0.01f)
                return true;

            return Quaternion.Angle(_head.localRotation, Quaternion.identity) > 2f;
        }

        void AlignToBooth()
        {
            Vector3 headPos = _head.position;
            transform.RotateAround(headPos, Vector3.up, -_head.eulerAngles.y);

            headPos = _head.position;
            Vector3 desired = new Vector3(0f, headPos.y, 0f);
            transform.position += desired - headPos;
        }
    }
}
