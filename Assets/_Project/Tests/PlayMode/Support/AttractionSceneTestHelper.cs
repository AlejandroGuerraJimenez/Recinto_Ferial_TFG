using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Tests.PlayMode.Support
{
    /// <summary>
    /// Shared helpers for attraction scene PlayMode smoke/gameplay tests.
    /// </summary>
    public static class AttractionSceneTestHelper
    {
        public static IEnumerator LoadAttractionScene(string sceneName)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(sceneName), "Scene name is required.");

            var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            Assert.IsNotNull(load, $"Scene '{sceneName}' is missing from Build Settings.");

            while (!load.isDone)
                yield return null;

            // Allow Awake/Start + installer Build to finish.
            yield return null;
            yield return null;
        }

        public static IEnumerator WaitUntil(Func<bool> condition, float timeoutSeconds, string failureMessage)
        {
            float deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup > deadline)
                    Assert.Fail(failureMessage);

                yield return null;
            }
        }

        public static T Require<T>() where T : UnityEngine.Object
        {
            var found = UnityEngine.Object.FindFirstObjectByType<T>();
            Assert.IsNotNull(found, $"Expected {typeof(T).Name} in the loaded scene.");
            return found;
        }

        public static IEnumerator WaitFor<T>(float timeoutSeconds = 3f) where T : UnityEngine.Object
        {
            T found = null;
            yield return WaitUntil(
                () =>
                {
                    found = UnityEngine.Object.FindFirstObjectByType<T>();
                    return found != null;
                },
                timeoutSeconds,
                $"Timed out waiting for {typeof(T).Name}.");

            Assert.IsNotNull(found);
        }
    }
}
