using System;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.Event
{
    [CreateAssetMenu(menuName="Torii/Event")]
    public class ToriiEvent : ScriptableObject
    {
        /// <summary>
        /// Action used for programmatic event handlers.
        /// </summary>
        public Action OnEventRaised { get; protected set; }

        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<IToriiEventListener> eventListeners = new List<IToriiEventListener>();

        public void Raise()
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
        }

        public void RegisterListener(IToriiEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void RegisterListener(Action listener)
        {
            var eventListener = new actionEventListener(listener);
            if (!eventListeners.Contains(eventListener))
                eventListeners.Add(eventListener);
        }

        public void UnregisterListener(IToriiEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

        public void UnregisterListener(Action listener)
        {
            var eventListener = new actionEventListener(listener);
            if (eventListeners.Contains(eventListener))
                eventListeners.Remove(eventListener);
        }
        
        protected readonly struct actionEventListener : IToriiEventListener, IEquatable<actionEventListener>
        {
            public readonly Action Action;
            
            public actionEventListener(Action action) => Action = action;

            public void OnEventRaised() => Action?.Invoke();

            public bool Equals(actionEventListener other) => Equals(Action, other.Action);

            public override bool Equals(object obj) => obj is actionEventListener other && Equals(other);

            public override int GetHashCode() => Action != null ? Action.GetHashCode() : 0;

            public static bool operator ==(actionEventListener left, actionEventListener right) => left.Equals(right);
            public static bool operator !=(actionEventListener left, actionEventListener right) => !left.Equals(right);
        }
    }
}