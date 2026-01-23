using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace EventBus
{
    public class EventBusPublisher : MonoBehaviour
    {
        [SerializeField] protected string eventName;
        [SerializeField] protected bool clearAfterPublish = false;
        [SerializeField] protected UnityEvent onPublished;
        
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