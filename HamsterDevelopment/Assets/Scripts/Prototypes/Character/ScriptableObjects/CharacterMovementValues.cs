using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMovementValues", menuName = "Scriptable Objects/CharacterMovementValues")]
public class CharacterMovementValues : ScriptableObject
{

    [Tooltip("Disables movement upon loading these values")]
    [SerializeField] private bool disableMovement;
    [Tooltip("Disables jumping upon loading these values")]
    [SerializeField] private bool disableJumping;
    [Tooltip("Disables normal gravity upon loading these values")]
    [SerializeField] private bool disableGravity;

    [Space]
    [Tooltip("Use different movement values while this is attached to the character")]
    [SerializeField] private bool useMovementValues;
    [Tooltip("Use different turn values while this is attached to the character")]
    [SerializeField] private bool useTurnValues;
    [Tooltip("Use different jump values while this is attached to the character")]
    [SerializeField] private bool useJumpValues;
    [Tooltip("Use different gravity values while this is attached to the character")]
    [SerializeField] private bool useGravityValues;

    [Space]
    [Tooltip("Resets velocity when loading in")]
    [SerializeField] private bool resetVelocity;

    [Separator("Values")]


    [Header("Movement values")]

    [Tooltip("Maximum horizontal velocity")]
    [SerializeField, ConditionalField(nameof(useMovementValues))] private float maxHorVelocity = 6;
    [Tooltip("Acceleration for direction player is facing")]
    [SerializeField, ConditionalField(nameof(useMovementValues))] private float acceleration = .5f;
    [Tooltip("Acceleration for directions player isn't facing (left, right, back)")]
    [SerializeField, ConditionalField(nameof(useMovementValues))] private float control = .1f;
    [Tooltip("Deceleration when player doesn't input movement")]
    [SerializeField, ConditionalField(nameof(useMovementValues))] private float brake = .3f;


    [Header("Turn values")]

    [Tooltip("Amount of degrees per frame the player can rotate when standing still")]
    [SerializeField, ConditionalField(nameof(useTurnValues))] private float minTurnSpeed = 1f;
    [Tooltip("Amount of degrees per frame the player can rotate when moving at top speed")]
    [SerializeField, ConditionalField(nameof(useTurnValues))] private float maxTurnSpeed = 7.5f;


    [Header("Jump values")]

    [Tooltip("The height of the jump. 1 is 1 unity cube. No fancy slider here because springs might require big value")]
    [SerializeField, ConditionalField(nameof(useJumpValues))] private float jumpHeight = 2f;
    [Range(0f, 1f)] [Tooltip("Variable jump height. If you let go of the jump button before reaching the peak of the jump, the velocity gets multiplied with this amount.\n\n0 = full stop to upwards velocity\n1 = no special changes to upwards velocity")]
    [SerializeField, ConditionalField(nameof(useJumpValues))] private float shortJumpMult = .5f;
    [Range(0f, .5f)] [Tooltip("Not implemented yet!!! When you press jump while still in the air, you'll still jump if you land on the ground within [jumpBuffer] seconds")]
    [SerializeField, ConditionalField(nameof(useJumpValues))] private float jumpBuffer = .2f;
    [Range(0f, .5f)] [Tooltip("Not implemented yet!!! When you fall off a platform, you can still jump for [coyoteTime] seconds")]
    [SerializeField, ConditionalField(nameof(useJumpValues))] private float coyoteTime = .2f;

    [Header("Gravity values")]

    [Tooltip("The modifier used to fake gravity. -9.81 is the default setting that is supposed to fake the real world gravity.")]
    [SerializeField, ConditionalField(nameof(useGravityValues))] private float gravity = -9.81f;
    [Range(0f, 2f)] [Tooltip("Extra bit of gravity multiplier when falling. Should be subtle")]
    [SerializeField, ConditionalField(nameof(useGravityValues))] private float bonusGravity = 1;
    [Range(0.1f, 30f)] [Tooltip("Max fall speed. Only applies for falling here, but realistically, it should apply in all directions. \nAccording to quora, a hamster can reach a terminal velocity on the order of 15-25 m/s")]
    [SerializeField, ConditionalField(nameof(useGravityValues))] private float terminalVelocity = 20f;


    public bool GetDisableMovement() { return disableMovement; }
    public bool GetDisableJumping() { return disableJumping; }
    public bool GetDisableGravity() { return disableGravity; }

    public bool GetUseMovementValues() { return useMovementValues; }
    public bool GetUseTurnValues() { return useTurnValues; }
    public bool GetUseJumpValues() { return useJumpValues; }
    public bool GetUseGravityValues() { return useGravityValues; }

    public bool GetResetVelocity() { return resetVelocity; }

    public float GetValueMaxHorVelocity() { return maxHorVelocity; }
    public float GetValueAcceleration() { return acceleration; }
    public float GetValueControl() { return control; }
    public float GetValueBrake() { return brake; }
    public float GetValueMinTurnSpeed() { return minTurnSpeed; }
    public float GetValueMaxTurnSpeed() { return maxTurnSpeed; }
    public float GetValueJumpHeight() { return jumpHeight; }
    public float GetValueShortJumpMult() { return shortJumpMult; }
    public float GetValueJumpBuffer() { return jumpBuffer; }
    public float GetValueCoyoteTime() { return coyoteTime; }
    public float GetValueGravity() { return gravity; }
    public float GetValueBonusGravity() { return bonusGravity; }
    public float GetValueTerminalVelocity() { return terminalVelocity; }
}
