using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Visual rod that lowers the hook while dipping.
    /// </summary>
    public sealed class FishingRodView : MonoBehaviour
    {
        [SerializeField] Transform hook;
        [SerializeField] Vector3 restLocalPosition = new Vector3(0f, 0f, 0.55f);
        [SerializeField] Vector3 dipLocalPosition = new Vector3(0f, -0.35f, 0.7f);
        [SerializeField] float moveSpeed = 10f;

        FishingHookView _hookView;
        Vector3 _targetLocal;

        public FishingHookView Hook => _hookView;

        public void Configure(Transform hookTransform, FishingHookView hookView)
        {
            hook = hookTransform;
            _hookView = hookView;
            _targetLocal = restLocalPosition;
            if (hook != null)
                hook.localPosition = restLocalPosition;
        }

        void Update()
        {
            if (hook == null)
                return;

            bool dipping = _hookView != null && _hookView.IsDipping;
            _targetLocal = dipping ? dipLocalPosition : restLocalPosition;
            hook.localPosition = Vector3.Lerp(hook.localPosition, _targetLocal, Time.deltaTime * moveSpeed);
        }
    }
}
