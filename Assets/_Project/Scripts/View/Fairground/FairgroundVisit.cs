using System;
using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// Carries one <see cref="FairgroundVisitState"/> from the fairground into the attraction scene.
    /// The fairground delivers it when a floor is confirmed. The attraction installer reads it on load.
    /// Leaving an attraction keeps the floor. <see cref="Close"/> drops the visit.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public sealed class FairgroundVisit : MonoBehaviour
    {
        static FairgroundVisit _active;

        public FairgroundVisitState State { get; private set; }

        /// <summary>
        /// Hands <paramref name="visit"/> to whichever scene loads next.
        /// </summary>
        public static void Deliver(FairgroundVisitState visit)
        {
            if (visit == null)
                throw new ArgumentNullException(nameof(visit));

            if (_active == null)
            {
                var host = new GameObject(nameof(FairgroundVisit));
                DontDestroyOnLoad(host);
                _active = host.AddComponent<FairgroundVisit>();
            }

            _active.State = visit;
        }

        /// <summary>
        /// The visit delivered before this scene loaded, if the fairground opened one.
        /// </summary>
        public static bool TryReceive(out FairgroundVisitState visit)
        {
            if (_active == null)
            {
                visit = null;
                return false;
            }

            visit = _active.State;
            return visit != null;
        }

        /// <summary>
        /// Drops the floor and the attraction. The next scene no longer sees this visit.
        /// </summary>
        public static void Close()
        {
            if (_active == null)
                return;

            var visit = _active;
            _active = null;
            visit.State?.Clear();
            visit.State = null;
            Destroy(visit.gameObject);
        }

        void OnDestroy()
        {
            if (ReferenceEquals(_active, this))
                _active = null;
        }
    }
}
