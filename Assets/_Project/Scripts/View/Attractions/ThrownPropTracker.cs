using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Watches the spawner and every live prop. The booth hears a throw and a resolve, nothing else.
    /// </summary>
    public sealed class ThrownPropTracker
    {
        readonly HashSet<IThrownProp> _live = new HashSet<IThrownProp>();
        BallSpawnerView _spawner;

        public event Action Thrown;
        public event Action Resolved;

        public void Attach(BallSpawnerView spawner)
        {
            if (ReferenceEquals(_spawner, spawner) && spawner != null)
                return;

            Detach();
            _spawner = spawner;
            if (spawner == null)
                return;

            spawner.BallSpawned += OnSpawned;
            if (spawner.CurrentBall != null)
                Watch(spawner.CurrentBall);
        }

        public void Detach()
        {
            if (_spawner != null)
                _spawner.BallSpawned -= OnSpawned;

            _spawner = null;
            Release();
        }

        public void DestroyAll(BallSpawnerView spawner)
        {
            var snapshot = new List<IThrownProp>(_live);
            Release();
            spawner?.Reset();
            foreach (var prop in snapshot)
            {
                if (prop is Component component && component != null)
                    UnityEngine.Object.Destroy(component.gameObject);
            }
        }

        void OnSpawned(IThrownProp prop) => Watch(prop);

        void Watch(IThrownProp prop)
        {
            if (prop == null || !_live.Add(prop))
                return;

            prop.Thrown += OnThrown;
            prop.Resolved += OnResolved;
            prop.Destroyed += OnDestroyed;
        }

        void OnThrown(IThrownProp prop) => Thrown?.Invoke();

        void OnResolved(IThrownProp prop)
        {
            Resolved?.Invoke();
            Forget(prop);
        }

        void OnDestroyed(IThrownProp prop) => Forget(prop);

        void Release()
        {
            if (_live.Count == 0)
                return;

            var snapshot = new List<IThrownProp>(_live);
            _live.Clear();
            foreach (var prop in snapshot)
                Forget(prop);
        }

        void Forget(IThrownProp prop)
        {
            if (prop != null)
            {
                prop.Thrown -= OnThrown;
                prop.Resolved -= OnResolved;
                prop.Destroyed -= OnDestroyed;
                _live.Remove(prop);
            }
        }
    }
}
