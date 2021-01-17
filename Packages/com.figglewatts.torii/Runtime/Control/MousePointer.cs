using System.Collections.Generic;
using UnityEngine;

namespace Torii.Control
{
    public static class MousePointer
    {
        internal class PointerState
        {
            public CursorLockMode LockMode;
            public bool Visible;

            public void Apply()
            {
                Cursor.lockState = LockMode;
                Cursor.visible = Visible;
            }
        }

        private static readonly Stack<PointerState> _pointerStateStack = new Stack<PointerState>();

        private static readonly PointerState _defaultState = new PointerState
            {LockMode = CursorLockMode.None, Visible = true};

        public static void ResetState()
        {
            _pointerStateStack.Clear();
            _defaultState.Apply();
        }

        public static void PushVisible()
        {
            var pointerState = new PointerState {LockMode = CursorLockMode.None, Visible = true};
            pointerState.Apply();
            _pointerStateStack.Push(pointerState);
        }

        public static void PushHidden()
        {
            var pointerState = new PointerState {LockMode = CursorLockMode.Locked, Visible = false};
            pointerState.Apply();
            _pointerStateStack.Push(pointerState);
        }

        public static void Pop()
        {
            if (_pointerStateStack.Count <= 1)
            {
                if (_pointerStateStack.Count == 1) _pointerStateStack.Pop();
                _defaultState.Apply();
                return;
            }

            _pointerStateStack.Pop();
            _pointerStateStack.Peek().Apply();
        }
    }
}
