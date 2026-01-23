using MyBox;
using UnityEngine;

namespace EventBus
{
    public class TypedEventBusPublisher<T> : EventBusPublisher
    {
        [SerializeField] private bool useVariable = false;
        [SerializeField, ConditionalField(nameof(useVariable), true)] protected T value;
        [SerializeField] private bool includeUntypedEvents = false;
        
      
        public override void PublishEvent()
        {
            EventBusManager.Instance.Publish<T>(eventName, value);
            if (includeUntypedEvents) EventBusManager.Instance.Publish(eventName);
            
            if(clearAfterPublish)  EventBusManager.Instance.UnSubScribeAllSubScribedListeners<T>(eventName);
            onPublished?.Invoke();
        }
        
        
        
        public void SetValue(T value)
        {
            this.value = value;
        }
        
    }
}