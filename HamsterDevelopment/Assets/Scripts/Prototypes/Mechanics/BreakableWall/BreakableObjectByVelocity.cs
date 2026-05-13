using System;
using EventBus;
using UnityEngine;

public class BreakableObjectByVelocity : MonoBehaviour
{
    [SerializeField] private float VelocityRequiredToBreak = 5;
    [SerializeField] private GameObject _debris;
    [SerializeField] private GameObject _mainWall;
    [SerializeField] private Collider _colliders;
    private EventBusPublisher publisher;
    private bool broken = false;
    
    private void OnEnable()
    {
        publisher = GetComponent<EventBusPublisher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(broken) return;
        //Confirm other is player and ball mode
        var hb = other.GetComponent<HamsterBallMovement>();
        if (hb != null)
        {
            //Confirm player is fast enough to remove the wall (and enable debris if possible).
            float speed = hb.CurrentVelocity.magnitude;
            Debug.Log(
                $"{gameObject.name}, {this.name}: Comparing velocity; Player speed:{speed}. Threshold: {VelocityRequiredToBreak}");
            if (speed > VelocityRequiredToBreak)
            {
                _colliders.enabled = false;
                _debris?.SetActive(true);
                _mainWall?.SetActive(false);
                
                publisher?.PublishEvent();
                broken = true;
            }   


        }
    }

}
