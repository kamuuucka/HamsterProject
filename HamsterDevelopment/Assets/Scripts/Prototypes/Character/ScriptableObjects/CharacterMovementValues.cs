using MyBox;
using UnityEditor;
using UnityEngine;

// Online examples for detecting entering play mode all include this, but here it also works without
//[InitializeOnLoad]
[CreateAssetMenu(fileName = "CharacterMovementValues", menuName = "Scriptable Objects/CharacterMovementValues")]
public class CharacterMovementValues : ScriptableObject
{
    // Used to disable editing the values when play mode is active
    private static bool enableEditing;

    private bool IsEditingEnabled() => enableEditing;

    [ButtonMethod(ButtonMethodDrawOrder.BeforeInspector)]
    private string ToggleEditingScriptable()
    {
        enableEditing = !enableEditing;

        return "Toggled editing for all CharacterMovementValues Scriptable Objects. Now set to " + enableEditing;
    }



    [Description("You're now looking inside the scriptable object. \n" +
        "Any values you change here are permanent and don't revert to their previous value upon exiting play mode.", messageType = MessageType.Warning)]

    #region Disable mechanics
    [Tooltip("Disables movement upon loading these values")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool disableMovement;
    [Tooltip("Disables jumping upon loading these values")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool disableJumping;
    [Tooltip("Disables normal gravity upon loading these values")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool disableGravity;
    #endregion


    #region Use values selection
    [Space]
    [Tooltip("Use different movement values while this is attached to the character")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool useMovementValues;
    [Tooltip("Use different turn values while this is attached to the character")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool useTurnValues;
    [Tooltip("Use different jump values while this is attached to the character")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool useJumpValues;
    [Tooltip("Use different gravity values while this is attached to the character")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool useGravityValues;
    #endregion

    [Space]
    [Tooltip("Resets velocity when loading in")]
    [SerializeField, ReadOnly(true, nameof(IsEditingEnabled), true)] private bool resetVelocity;

    [Separator("Values")]

    // [FoldOut("name")] is very nice, but it doesn't show up in [DisplayInspector] so I'm not using it for consistency
    #region Movement values
    [Header("Movement values")]


    [Tooltip("Maximum horizontal velocity \n\n[Units]/[Seconds] \n(If moving at maximum speed, your horizontal position changes by this value every second)")]
    [SerializeField, ConditionalField(nameof(useMovementValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float maxHorVelocity = 6;
    
    [Tooltip("Acceleration for direction player is facing \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
    [SerializeField, ConditionalField(nameof(useMovementValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float acceleration = 40f;
    
    [Tooltip("Acceleration for directions player isn't facing (left, right, back) \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
    [SerializeField, ConditionalField(nameof(useMovementValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float control = 8f;
    
    [Tooltip("Deceleration when player doesn't input movement \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
    [SerializeField, ConditionalField(nameof(useMovementValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float brake = 24f;
    #endregion


    #region Turn values
    [Header("Turn values")]


    [Tooltip("Amount of degrees per frame the player can rotate when standing still")]
    [SerializeField, ConditionalField(nameof(useTurnValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float minTurnSpeed = 1f;
    
    [Tooltip("Amount of degrees per frame the player can rotate when moving at top speed")]
    [SerializeField, ConditionalField(nameof(useTurnValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float maxTurnSpeed = 7.5f;
    #endregion


    #region Jump values
    [Header("Jump values")]


    [Tooltip("The height of the jump. 1 is 1 unity cube")]
    [SerializeField, ConditionalField(nameof(useJumpValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float jumpHeight = 2f;
    
    [Tooltip("Variable jump height. If you let go of the jump button before reaching the peak of the jump, the velocity gets multiplied with this amount.\n\n0 = full stop to upwards velocity\n1 = no special changes to upwards velocity")]
    [SerializeField, ConditionalField(nameof(useJumpValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float shortJumpMult = .5f;
    
    [Tooltip("Not implemented yet!!! When you press jump while still in the air, you'll still jump if you land on the ground within [jumpBuffer] seconds")]
    [SerializeField, ConditionalField(nameof(useJumpValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float jumpBuffer = .2f;
    
    [Tooltip("Not implemented yet!!! When you fall off a platform, you can still jump for [coyoteTime] seconds")]
    [SerializeField, ConditionalField(nameof(useJumpValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float coyoteTime = .2f;
    #endregion


    #region Gravity values
    [Header("Gravity values")]

    
    [Tooltip("The modifier used to fake gravity. -9.81 is the default setting that is supposed to fake the real world gravity.")]
    [SerializeField, ConditionalField(nameof(useGravityValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float gravity = -9.81f;
    
    [Tooltip("Extra bit of gravity multiplier when falling. Should be subtle")]
    [SerializeField, ConditionalField(nameof(useGravityValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float bonusGravity = 1;
    
    [Tooltip("Max fall speed. Only applies for falling here, but realistically, it should apply in all directions. \nAccording to quora, a hamster can reach a terminal velocity on the order of 15-25 m/s")]
    [SerializeField, ConditionalField(nameof(useGravityValues)), ReadOnly(true, nameof(IsEditingEnabled), true)] private float terminalVelocity = 20f;
    #endregion


    #region Public get functions
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
    #endregion


    // When hitting play mode, sets all values to ReadOnly to avoid accidentally changing the values
    static CharacterMovementValues()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode) enableEditing = false;
    }

}
