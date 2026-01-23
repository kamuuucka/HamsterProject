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
    public class EventBusListener : BaseEventBusListener
    {
        
        [ SerializeField] private UnityEvent callBackAction;
        

        [ButtonMethod]
        public override void Subscribe()
        {
            EventBusManager.Instance.Subscribe(passedEventName, callBackAction);
            base.Subscribe();
        }
        [ButtonMethod]
        public override void UnSubscribe()
        {
            EventBusManager.Instance.UnSubscribe(passedEventName, callBackAction);
            base.UnSubscribe();
        }
        
    }

  
}
