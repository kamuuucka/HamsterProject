using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace EventBus
{
    public class EventBusPublisher : MonoBehaviour
    {
        [SerializeField] protected string eventName;
        [SerializeField,Tooltip("After firing your event clear the current event.")] protected bool clearAfterPublish = false;
        [SerializeField, Tooltip("Some actions need to happen AFTER publication (for example turning the publisher off. Use this event for those actions.")] 
        protected UnityEvent onPublished;
        
        public string Eventname
        {
            get => eventName;
        }
        
        public virtual void PublishEvent()
        {
            EventBusManager.Instance.Publish(eventName);
            if (clearAfterPublish) EventBusManager.Instance.UnSubScribeAllSubScribedListeners(eventName);
            onPublished?.Invoke();
        }
        [ButtonMethod]
        public void Publish()
        {
            PublishEvent();
        }
        
    }

   
    
}