using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EventBus
{
    public class TypedActionEventBus<T> : TypedEventBus
    {
        public override Type EventType => typeof(T);
        private readonly Dictionary<string, List<UnityAction<T>>> events =
            new Dictionary<string, List<UnityAction<T>>>();

        public void Subscribe(string key, UnityAction<T> evt)
        {
            if (!events.TryGetValue(key, out var list))
            {
                list = new List<UnityAction<T>>();
                events[key] = list;
            }
            list.Add(evt);
        }

        public void ClearEvent(string eventName)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName].Clear();
            }
        }
        
        public void Publish(string key, T payload)
        {
            if (!events.TryGetValue(key, out var list)) return;
            foreach (var evt in list)
                evt.Invoke(payload);
        }

        public void UnSubscribe(string eventName, UnityAction<T> unityEvent)
        {
            if (events.TryGetValue(eventName, out var @event))
            {
                @event.Remove(unityEvent);
            }
        }
    }
}