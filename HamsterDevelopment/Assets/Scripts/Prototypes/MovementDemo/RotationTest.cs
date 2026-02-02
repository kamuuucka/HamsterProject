using MyBox;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    [Tooltip("Rotates this many degrees around its respective axis")]
    [SerializeField] Vector3 eulerRotationApplied = Vector3.up;

    [Tooltip("If this object has a rigidbody: Check this box and drag rigidbody component in the field that appears below")]
    [SerializeField] bool hasRigidbody;
    [SerializeField, ConditionalField(nameof(hasRigidbody), true)] Rigidbody rb;

    [Tooltip("Reapplies force to rigidbody every [value] seconds")]
    [SerializeField, ConditionalField(nameof(hasRigidbody), true)] float velocityResetTimer = 10;
    bool velocityReset = true;



    private void FixedUpdate()
    {
        
        if (!hasRigidbody)
        {
            transform.Rotate(eulerRotationApplied * Time.fixedDeltaTime);
        }
        else
        {
            if (Time.timeSinceLevelLoadAsDouble % velocityResetTimer > velocityResetTimer/2 && !velocityReset)
            {
                velocityReset = true;
            }
            else if (Time.timeSinceLevelLoadAsDouble % velocityResetTimer < velocityResetTimer/2 && velocityReset)
            {
                rb.angularVelocity = eulerRotationApplied * Mathf.Deg2Rad;
                velocityReset = false;
            }
        }

    }





}
