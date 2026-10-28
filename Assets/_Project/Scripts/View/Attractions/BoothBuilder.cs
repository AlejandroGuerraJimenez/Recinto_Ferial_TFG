using System.Collections.Generic;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Shared booth shell. Each attraction passes a <see cref="BoothLayout"/> and implements <see cref="BuildGameplay"/>.
    /// </summary>
    public abstract class BoothBuilder
    {
        readonly BoothAssembly _shell;
        readonly BoothHudFactory _hudFactory = new BoothHudFactory();
        readonly List<Transform> _gameplayRoots = new List<Transform>();

        protected BoothBuilder(BoothLayout layout, GameObject xrOriginPrefab)
        {
            Layout = layout ?? new BoothLayout();
            _shell = new BoothAssembly(Layout, xrOriginPrefab, Layout.LogTag, Layout.Sky);
        }

        protected BoothLayout Layout { get; }

        public GameObject XrOrigin => _shell.XrOrigin;

        public BoothHudView Hud { get; private set; }

        protected GameObject Environment => _shell.Environment;

        /// <summary>
        /// Environment, XR rig, attraction gameplay, then the HUD described by the layout.
        /// </summary>
        public void BuildShell(Transform bakedEnvironment, GameObject environmentPrefab)
        {
            if (bakedEnvironment != null)
            {
                _shell.UseBakedEnvironment(bakedEnvironment);
                OnEnvironmentReady(bakedEnvironment);
            }
            else
            {
                _shell.BuildEnvironment(environmentPrefab);
                OnEnvironmentReady(Environment != null ? Environment.transform : null);
            }

            _shell.BuildXrRig(OnRigReady);
            BuildGameplay();
            Hud = _hudFactory.Create(Layout.HudPosition, Layout.HudLines);
            Track(Hud != null ? Hud.transform : null);
        }

        protected virtual void OnEnvironmentReady(Transform environmentRoot)
        {
        }

        protected virtual void OnRigReady(GameObject xrOrigin)
        {
        }

        protected abstract void BuildGameplay();

        protected void Track(Transform root)
        {
            if (root != null)
                _gameplayRoots.Add(root);
        }

        public Transform[] BoothSpaceRoots()
        {
            var roots = new Transform[_gameplayRoots.Count + 1];
            roots[0] = Environment != null ? Environment.transform : null;
            for (int i = 0; i < _gameplayRoots.Count; i++)
                roots[i + 1] = _gameplayRoots[i];

            return roots;
        }
    }
}
