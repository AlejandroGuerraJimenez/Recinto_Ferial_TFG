using Fairground.Core.Attractions;
using Fairground.ViewModel;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// One scene change at a time. A second press while the load is running is ignored.
    /// </summary>
    public static class AttractionExit
    {
        static bool _loading;

        public static void ReturnToFairground(FairgroundVisitState visit)
        {
            if (_loading)
                return;

            visit?.LeaveAttraction();
            Load(AttractionScenes.Fairground);
        }

        public static void Enter(string sceneName)
        {
            if (_loading || string.IsNullOrEmpty(sceneName))
                return;

            Load(sceneName);
        }

        static void Load(string sceneName)
        {
            _loading = true;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                _loading = false;
                return;
            }

            operation.completed += _ => _loading = false;
        }
    }
}
