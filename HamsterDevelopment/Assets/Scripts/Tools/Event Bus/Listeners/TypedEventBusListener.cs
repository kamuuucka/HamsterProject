using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace EventBus
{
    public class TypedEventBusListener<T> : BaseEventBusListener
    {

        [SerializeField] protected UnityEvent<T> callBack;
        protected override string[] GetNames()
        {
            List<string> names = EventBusManager.Instance.GetAllEventsInScene<T>().ToList();
            names.Insert(0, "");
            return names.ToArray();
        }

        public override void Subscribe()
        {
            EventBusManager.Instance.Subscribe<T>(eventName, callBack);
            base.Subscribe();
        }
        public override void UnSubscribe()
        {
            EventBusManager.Instance.UnSubscribe<T>(eventName,callBack);
            base.UnSubscribe();

        }

    }
}