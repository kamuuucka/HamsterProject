using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace EventBus
{
    /// <summary>
    /// THIS IS NOT A SAFE LISTNEER TO USE. IT SIMPLY HOLDS THE COMMON USED VALUES AND TYPES OF THE EVENT BUS LISTENERS
    /// </summary>
    public abstract class BaseEventBusListener : MonoBehaviour
    {
        [SerializeField] protected bool overrideEventNameSelection = false;

        [ConditionalField(nameof(overrideEventNameSelection), true), DefinedValues(nameof(GetNames)), SerializeField]
        protected string eventName;

        [ConditionalField(nameof(overrideEventNameSelection)), SerializeField]
        protected string overriddenEventName;

        [SerializeField] protected bool subscribeOnEnable = true;
        [ReadOnly] public bool Subscribed { get; private set; }




        protected bool stopOnSubScribeOnDisable = false;
        protected string passedEventName => overrideEventNameSelection ? overriddenEventName : eventName;
        public string PassedEventName => passedEventName;
            
        protected virtual string[] GetNames()
        {
            List<string> names = EventBusManager.Instance.GetAllEventsInScene().ToList();
            names.Insert(0, "");
            return names.ToArray();
        }

        protected virtual void OnEnable()
        {
            if(subscribeOnEnable) Subscribe();
        }
        [ButtonMethod]
        public virtual void Subscribe()
        {
            Subscribed = true;
        }
        [ButtonMethod]
        public virtual void UnSubscribe()
        {
            Subscribed = false;
        }

        protected virtual void OnApplicationQuit()
        {
            stopOnSubScribeOnDisable = true;
        }

        protected virtual void OnDisable()
        {
            if (!stopOnSubScribeOnDisable && subscribeOnEnable)
            {
                UnSubscribe();
            }
        }
    }
}