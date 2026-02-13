using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EventBus
{
    public abstract class TypedEventBus
    { 
        public abstract Type EventType { get; }
    }

    public class TypedEventBus<T> : TypedEventBus
    {
        public override Type EventType => typeof(T);

        private readonly Dictionary<string, List<UnityEvent<T>>> events =
            new Dictionary<string, List<UnityEvent<T>>>();

        public void Subscribe(string key, UnityEvent<T> evt)
        {
            if (!events.TryGetValue(key, out var list))
            {
                list = new List<UnityEvent<T>>();
                events[key] = list;
            }
            list.Add(evt);
        }
        public void ClearEvent(string eventName)
        {
            if (events.ContainsKey(eventName))
            {
                var listeners = GameObject.FindObjectsOfType<TypedEventBusListener<T>>(true)
                    .Where(e => e.PassedEventName == eventName);
                foreach (var listener in listeners)
                {
                    listener.UnSubscribe();
                }
            }
  
            
        }
        public void Publish(string key, T payload)
        {
            if (!events.TryGetValue(key, out var list)) return;
            
            foreach (var evt in list)
                evt.Invoke(payload);
        }

        public void UnSubscribe(string eventName, UnityEvent<T> unityEvent)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName].Remove(unityEvent);
            }
        }
    }
}

