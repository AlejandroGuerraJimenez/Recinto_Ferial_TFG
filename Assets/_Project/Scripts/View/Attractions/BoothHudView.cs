using System;
using TMPro;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// World-space HUD. The attraction supplies the lines; this view only redraws them when state changes.
    /// </summary>
    public sealed class BoothHudView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI[] lines;

        Action _refresh;
        Action<Action> _unsubscribe;

        public void AssignLines(TextMeshProUGUI[] labels)
        {
            lines = labels;
        }

        public void Bind(Func<string[]> readLines, Action<Action> subscribe, Action<Action> unsubscribe)
        {
            Unbind();
            if (readLines == null || subscribe == null)
                return;

            _unsubscribe = unsubscribe;
            _refresh = () => Apply(readLines());
            subscribe(_refresh);
            _refresh();
        }

        void OnDestroy()
        {
            Unbind();
        }

        void Unbind()
        {
            if (_refresh != null && _unsubscribe != null)
                _unsubscribe(_refresh);

            _refresh = null;
            _unsubscribe = null;
        }

        void Apply(string[] values)
        {
            if (lines == null || values == null)
                return;

            int count = Mathf.Min(lines.Length, values.Length);
            for (int i = 0; i < count; i++)
            {
                if (lines[i] != null)
                    lines[i].text = values[i];
            }
        }
    }
}
