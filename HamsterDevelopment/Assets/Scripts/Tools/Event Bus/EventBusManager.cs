using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace EventBus
{
    public class EventBusManager
    {
        private Dictionary<string, List<UnityEvent>> events = new Dictionary<string, List<UnityEvent>>();
        private Dictionary<string,List<Action>> eventActions = new Dictionary<string, List<Action>>();
        private Dictionary<Type, TypedEventBus> typedUnityEvents = new Dictionary<Type, TypedEventBus>();
        private Dictionary<Type, TypedEventBus> typedActions = new Dictionary<Type, TypedEventBus>();
        
        private static EventBusManager _instance;

        public static EventBusManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventBusManager();
                }
                return _instance;
            }
        }
        /// <summary>
        /// Subscribe to a typed event using UnityEvents (The inspector way).
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        /// <typeparam name="T"></typeparam>
        public void Subscribe<T>(string eventName, UnityEvent<T> unityEvent)
        {
            if (typedUnityEvents.ContainsKey(typeof(T)))
            {
               (typedUnityEvents[typeof(T)] as TypedEventBus<T>)?.Subscribe(eventName, unityEvent);
            }
            else
            {
                var newBus = new TypedEventBus<T>();
                newBus.Subscribe(eventName, unityEvent);
                typedUnityEvents.Add(typeof(T), newBus);
            }
        }
        /// <summary>
        /// Subscribe to a typed event using UnityActions (The code way).
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityAction"></param>
        /// <typeparam name="T"></typeparam>
        public void Subscribe<T>(string eventName, UnityAction<T> unityAction)
        {
            if (typedActions.ContainsKey(typeof(T)))
            {
                (typedActions[typeof(T)] as TypedActionEventBus<T>)?.Subscribe(eventName, unityAction);
            }
            else
            {
                var newBus = new TypedActionEventBus<T>();
                newBus.Subscribe(eventName, unityAction);
                typedActions.Add(typeof(T), newBus);
                
            }
        }
        /// <summary>
        /// Unsubscribe to a typed event using UnityEvents (The inspector way).
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        /// <typeparam name="T"></typeparam>
        public void UnSubscribe<T>(string eventName, UnityEvent<T> unityEvent)
        {
            if (typedUnityEvents.ContainsKey(typeof(T)))
            {
                (typedUnityEvents[typeof(T)] as TypedEventBus<T>)?.UnSubscribe(eventName, unityEvent);
            }
        }
        /// <summary>
        /// Unsubscribe to a typed event using UnityActions (The code way).
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityAction"></param>
        /// <typeparam name="T"></typeparam>
        public void UnSubscribe<T>(string eventName, UnityAction<T> unityEvent)
        {
            if (typedActions.ContainsKey(typeof(T)))
            {
                (typedActions[typeof(T)] as TypedActionEventBus<T>)?.UnSubscribe(eventName, unityEvent);
            }
        }
        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        public void Subscribe(string eventName, UnityEvent unityEvent)
        {
            if (!events.TryAdd(eventName, new List<UnityEvent>() { unityEvent }))
            {
                events[eventName].Add(unityEvent);
            }
        }
        /// <summary>
        /// Publish a typed event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void Publish<T>(string eventName, T data)
        {
            if (typedUnityEvents.ContainsKey(typeof(T)))
            {
                (typedUnityEvents[typeof(T)] as TypedEventBus<T>)?.Publish(eventName, data);

            }

            if (typedActions.ContainsKey(typeof(T)))
            {
                (typedActions[typeof(T)] as TypedActionEventBus<T>)?.Publish(eventName, data);
            }
        }
        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        public void Subscribe(string eventName, Action unityEvent)
        {
            if (!eventActions.TryAdd(eventName, new List<Action>() { unityEvent }))
            {
                eventActions[eventName].Add(unityEvent);
            }
        }
        /// <summary>
        /// Unsubscribe to an event. It is important to note that if you used a delegate to subscribe. You cannot unsubscribe said event (I.E. Don't do () => code here) but pass a method in the parameter.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        public void UnSubscribe(string eventName, Action unityEvent)
        {
            if (eventActions.ContainsKey(eventName))
            {
                eventActions[eventName].Remove(unityEvent);
            }
        }
        /// <summary>
        /// Unsubscribe using a unity event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="unityEvent"></param>
        public void UnSubscribe(string eventName, UnityEvent unityEvent)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName].Remove(unityEvent);
            }
        }
        /// <summary>
        /// Publish a regular event.
        /// </summary>
        /// <param name="eventName"></param>
        public void Publish(string eventName)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName]?.ForEach(unityEvent => unityEvent?.Invoke());
            }

            if (eventActions.ContainsKey(eventName))
            {
                eventActions[eventName]?.ForEach(unityEvent => unityEvent?.Invoke());
            }
        }
        /// <summary>
        /// Finds all publishers in the scene and gets their event name.
        /// </summary>
        /// <returns>A list of the event names with publishers in  the scene.</returns>
        public string[] GetAllEventsInScene()
        {
            List<EventBusPublisher> publishers =
                GameObject.FindObjectsOfType<EventBusPublisher>(true).ToList();
            if (publishers.Count == 0)
            {
                return new[] { "" };
            }

            return publishers.Select(p => p.Eventname).ToArray();

        }
        /// <summary>
        /// Finds all typed publishers in the scene and gets their event name.
        /// </summary>
        /// <returns>A list of the event names with publishers in  the scene.</returns>
        public string[] GetAllEventsInScene<T>()
        {
            List<TypedEventBusPublisher<T>> publishers =
                GameObject.FindObjectsOfType<TypedEventBusPublisher<T>>(true).ToList();
            if (publishers.Count == 0)
            {
                return new[] { "" };
            }

            return publishers.Select(p => p.Eventname).ToArray();

        }
        /// <summary>
        /// Find all unsubscribed listeners of event name and subscribe them.
        /// </summary>
        /// <param name="eventName"></param>
        public void SubScribeAllUnsubScribedListeners(string eventName)
        {
            var allUnTypedListeners = GameObject.FindObjectsOfType<EventBusListener>(true);
            var listenersOfEventName =
                allUnTypedListeners.Where(l => !l.Subscribed && l.PassedEventName == eventName).ToList();
            foreach (var listener in listenersOfEventName)
            {
                listener.Subscribe();
            }
        }
        /// <summary>
        /// Clear out all subscribers of an event.
        /// </summary>
        /// <param name="eventName"></param>
        public void UnSubScribeAllSubScribedListeners(string eventName)
        {
            //events;
            if (events.ContainsKey(eventName))
            {
                if (events[eventName].Count > 0)
                {
                    //If we clear we break the game object listeners that are subscribed. Find a cleaner way for this
                    var listenersOfEventName = GameObject.FindObjectsOfType<EventBusListener>(true).Where(e => e.PassedEventName == eventName).ToList();
                    foreach (var listener in listenersOfEventName)
                    {
                        listener.UnSubscribe();
                    }
                }
            }
            if(eventActions.ContainsKey(eventName)) eventActions[eventName].Clear();
        }
        /// <summary>
        /// <inheritdoc cref="UnSubScribeAllSubScribedListeners"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="T"></typeparam>
        public void UnSubScribeAllSubScribedListeners<T>(string eventName)
        {
            //just remove it from the base dictionaries
            UnSubScribeAllSubScribedListeners(eventName);
            if (typedUnityEvents.ContainsKey(typeof(T)))
            {
                var eventBus = typedUnityEvents[typeof(T)];
                //we got a type match
                if (eventBus is TypedEventBus<T> typedEventBus)
                {
                    typedEventBus.ClearEvent(eventName);
                }
            }

            if (typedActions.ContainsKey(typeof(T)))
            {
                var eventBus = typedActions[typeof(T)];
                if (eventBus is TypedActionEventBus<T> typedActionEventBus)
                {
                    typedActionEventBus.ClearEvent(eventName);
                }
            }
            
        }
        
    }
    
}