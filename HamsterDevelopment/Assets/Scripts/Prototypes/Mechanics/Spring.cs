using Prototypes.Character;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Spring : MonoBehaviour
{
    [Header("Height")]
    [Tooltip("Always reach this height at minimum when bounced")]
    [SerializeField] private float minBounceHeight;

    [Tooltip("Additional height gain, depends on fall speed and terminal velocity. \n Output = Value * Fall Speed / Terminal Velocity")]
    [SerializeField] private float maxVelocityBounce;

    [Tooltip("Maximum height gained by the bounce. ")]
    [SerializeField] private float maxBounceHeight;

    [Tooltip("True: If maxVelocityBounce returns a value that's lower than minBounceHeight, set bounce height to minBounceHeight. \n False: Add minBounceHeight to the total bounce height")]
    [SerializeField] private bool clampMinValueDontAdd;


    [Header("Horizontal")]

    [Tooltip("If you land on the spring, the horizontal velocity is multiplied by this much (set to 0 to kill any initial horizontal velocity)")]
    [Range(0f, 1f)]
    [SerializeField] private float horVelMult;

    [Tooltip("Additional horizontal velocity in the direction of the input")]
    [SerializeField] private float horInputBoost;

    [Tooltip("Maximum horizontal velocity after a bounce. Set to 0 to disable")]
    [SerializeField] private float maxHorVelOutput;

    [Tooltip("Hitbox of the part that makes the hamster bounce")]
    [SerializeField] private BoxCollider boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Hamster Bounce Activate!
            other.GetComponent<CharacterMovement>().DoSpringJump(minBounceHeight, maxVelocityBounce, maxBounceHeight, clampMinValueDontAdd, horVelMult, horInputBoost, maxHorVelOutput);
        }
    }

    private void OnDrawGizmosSelected()
    {

        Vector3 basePos = transform.position + boxCollider.center + boxCollider.size.y * .5f * Vector3.up;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(basePos + Vector3.up * minBounceHeight, .5f);

        Gizmos.color = Color.blue;
        if (clampMinValueDontAdd)
        {
            Gizmos.DrawWireSphere(basePos + Vector3.up * maxVelocityBounce, .5f);
        }
        else
        {
            Gizmos.DrawWireSphere(basePos + Vector3.up * (maxVelocityBounce + minBounceHeight), .5f);

        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(basePos + Vector3.up * maxBounceHeight, .5f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(basePos + Vector3.up * minBounceHeight, 
            basePos + Vector3.up * Mathf.Min((clampMinValueDontAdd ? 0 : minBounceHeight) + maxVelocityBounce, maxBounceHeight));

    }
}
