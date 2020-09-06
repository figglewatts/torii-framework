using Torii.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Torii.Injection
{
    public class SceneInjector : MonoSingleton<SceneInjector>
    {
        private readonly Injector _injector = new Injector();

        public override void Init() { SceneManager.sceneLoaded += (scene, mode) => _injector.AutoResolve(); }

        public void Resolve(MonoBehaviour script) { _injector.Resolve(script); }
    }
}
