using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Torii.UI
{
    public class UISelectMe : MonoBehaviour
    {
        public bool OnlyIfControllerPresent = false;

        public void OnEnable()
        {
            if (OnlyIfControllerPresent && !isControllerConnected()) return;

            StartCoroutine(selectLater());
        }

        private bool isControllerConnected() { return Input.GetJoystickNames().Length > 0; }

        private IEnumerator selectLater()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
