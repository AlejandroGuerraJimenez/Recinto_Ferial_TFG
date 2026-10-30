using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// The line stays cast. The hook rises only after a duck is hooked, then drops back.
    /// </summary>
    public sealed class FishingRodView : MonoBehaviour
    {
        [SerializeField] Transform hook;
        [SerializeField] Transform lineOrigin;
        [SerializeField] LineRenderer line;
        [SerializeField] Vector3 castLocalPosition = new Vector3(0f, -0.5f, 1f);
        [SerializeField] Vector3 caughtLocalPosition = new Vector3(0f, 0.22f, 0.35f);
        [SerializeField] float moveSpeed = 7f;

        FishingHookView _hookView;
        Vector3 _targetLocal;

        public FishingHookView Hook => _hookView;

        public void Configure(
            Transform hookTransform,
            FishingHookView hookView,
            Transform origin,
            LineRenderer lineRenderer)
        {
            hook = hookTransform;
            _hookView = hookView;
            lineOrigin = origin;
            line = lineRenderer;
            _targetLocal = castLocalPosition;
            if (hook != null)
                hook.localPosition = castLocalPosition;

            DrawLine();
        }

        void Update()
        {
            if (hook == null)
                return;

            bool reeling = _hookView != null && _hookView.IsReeling;
            _targetLocal = reeling ? caughtLocalPosition : castLocalPosition;
            hook.localPosition = Vector3.Lerp(hook.localPosition, _targetLocal, Time.deltaTime * moveSpeed);
            DrawLine();
        }

        void DrawLine()
        {
            if (line == null || lineOrigin == null || hook == null)
                return;

            line.SetPosition(0, lineOrigin.position);
            line.SetPosition(1, hook.position);
        }
    }
}
