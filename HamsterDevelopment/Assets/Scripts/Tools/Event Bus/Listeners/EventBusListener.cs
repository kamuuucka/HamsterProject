using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace EventBus
{
    /// <summary>
    /// An untyped event bus listener. Use this if you do not require dynamic parameters.
    /// </summary>
    public class EventBusListener : BaseEventBusListener
    {
        
        [ SerializeField] private UnityEvent callBackAction;
        
        /// <summary>
        /// Subscribe the event.
        /// </summary>
        [ButtonMethod]
        public override void Subscribe()
        {
            EventBusManager.Instance.Subscribe(passedEventName, callBackAction);
            base.Subscribe();
        }
              
        /// <summary>
        /// Subscribe the event.
        /// </summary>
        [ButtonMethod]
        public override void UnSubscribe()
        {
            EventBusManager.Instance.UnSubscribe(passedEventName, callBackAction);
            base.UnSubscribe();
        }
        
    }

  
}
