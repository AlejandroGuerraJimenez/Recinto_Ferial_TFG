using System;
using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// Carries one <see cref="FairgroundSession"/> from the fairground into the attraction scene.
    /// The fairground delivers it when a floor is confirmed. The attraction installer reads it on load.
    /// Leaving an attraction keeps the floor. <see cref="Close"/> drops the visit.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public sealed class FairgroundVisit : MonoBehaviour
    {
        static FairgroundVisit _active;

        public FairgroundSession Session { get; private set; }

        /// <summary>
        /// Hands <paramref name="session"/> to whichever scene loads next.
        /// </summary>
        public static void Deliver(FairgroundSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (_active == null)
            {
                var host = new GameObject(nameof(FairgroundVisit));
                DontDestroyOnLoad(host);
                _active = host.AddComponent<FairgroundVisit>();
            }

            _active.Session = session;
        }

        /// <summary>
        /// The session delivered before this scene loaded, if the fairground opened a visit.
        /// </summary>
        public static bool TryReceive(out FairgroundSession session)
        {
            if (_active == null)
            {
                session = null;
                return false;
            }

            session = _active.Session;
            return session != null;
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
            visit.Session?.Clear();
            visit.Session = null;
            Destroy(visit.gameObject);
        }

        void OnDestroy()
        {
            if (ReferenceEquals(_active, this))
                _active = null;
        }
    }
}
