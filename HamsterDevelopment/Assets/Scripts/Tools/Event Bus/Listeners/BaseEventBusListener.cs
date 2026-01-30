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
        [SerializeField,Tooltip("Override the dropdown list. Use if the publisher is not in the same scene as the listener (say with a prefab for example)")] protected bool overrideEventNameSelection = false;

        [Tooltip("The name of the event you want to respond to."),ConditionalField(nameof(overrideEventNameSelection), true), DefinedValues(nameof(GetNames)), SerializeField]
        protected string eventName;

        [Tooltip("The name of the event you want to respond to"), ConditionalField(nameof(overrideEventNameSelection)), SerializeField]
        protected string overriddenEventName;

        [SerializeField, Tooltip("Subscribe the gameobject on enable THIS ALSO UNSUBSCRIBES ON DISABLE")] protected bool subscribeOnEnable = true;
        [ReadOnly] public bool Subscribed { get; private set; }




        protected bool stopOnSubScribeOnDisable = false;
        protected string passedEventName => overrideEventNameSelection ? overriddenEventName : eventName;
        public string PassedEventName => passedEventName;
        
        /// <summary>
        /// Get the names of all events that currently have a publisher on the, in the scene.
        /// <seealso cref="EventBusManager.GetAllEventsInScene()"/>
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Most typed listeners technically have different logic for subscribing and unsubscribing.
        /// So the base simply sets subscribed to true or false 
        /// </summary>
        [ButtonMethod]
        public virtual void Subscribe()
        {
            Subscribed = true;
        }
        
        /// <summary>
        /// Most typed listeners technically have different logic for subscribing and unsubscribing.
        /// So the base simply sets subscribed to true or false 
        /// </summary>
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