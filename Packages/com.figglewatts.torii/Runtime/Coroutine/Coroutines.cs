using System;
using System.Collections;
using Torii.Util;

namespace Torii.Coroutine
{
    /// <summary>
    /// Used to allow for access to StartCoroutine anywhere, like within a ScriptableObject.
    /// Just do Coroutines.Begin(...)
    /// </summary>
    public class Coroutines : MonoSingleton<Coroutines>
    {
        protected static IEnumerator callNextFrame(Action callback)
        {
            yield return null;
            callback();
        }

        public static UnityEngine.Coroutine Begin(IEnumerator coroutine) { return Instance.StartCoroutine(coroutine); }

        public static void CallNextFrame(Action callback) { Begin(callNextFrame(callback)); }
    }
}
