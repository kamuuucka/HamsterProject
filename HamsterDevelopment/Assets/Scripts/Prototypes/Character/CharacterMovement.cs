using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Prototypes.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        enum ControlState
        {
            Grounded, Airborne, Special
        }
        public enum VelocityState
        {
            Acceleration, Control, Brake
        }

        [DisplayInspector]
        [Tooltip("This is where special movement values show up. If something shows up here (and you didn't add it manually), it means these values are used \nThis should usually get attached through the SetSpecialValues function, but you can also attach one manually. If you do this (or change the Scriptable Object) during playmode, make sure to press the [Reload Values] button at the bottom (or the values won't do anything!).")]
        [SerializeField] private CharacterMovementValues specialValues;

        [Description("\nAny values you see above are part of a scriptable object. \n" +
        "Any values you change there are permanent and don't revert to their previous value upon exiting play mode.\n", messageType = MessageType.Warning)]

        [Tooltip("State machine (debug)")]
        [SerializeField, ReadOnly] ControlState controlState = ControlState.Grounded;

        [Tooltip("Not a state machine")]
        [SerializeField, ReadOnly] public VelocityState velocityState;


        [Header("Choices for Control or Acceleration")]


        [Tooltip("Used for deciding control or acceleration. \nDo you want to require total speed of desired velocity to be bigger than total speed of current velocity?")]
        [SerializeField][Foldout("Control or Acceleration")] private bool requireBiggerTotalSpeed;

        [Tooltip("Used for deciding control or acceleration. \nWhen changing move direction, should the calculations use the look direction (true) or the velocity direction (false)?")]
        [SerializeField][Foldout("Control or Acceleration")] private bool isLookForward;

        [Tooltip("Used for deciding control or acceleration. \nShould the length of the desired velocity on the chosen direction be bigger than the total length of the current velocity?")]
        [SerializeField][Foldout("Control or Acceleration")] private bool useVelocityTotal;

        [Tooltip("Used for deciding control or acceleration. \nShould the length of the desired velocity on the chosen direction be bigger than the length of the current velocity on the chosen direction? \n This is ignored if above is true.")]
        [SerializeField][Foldout("Control or Acceleration")] private bool useVelocityDirectional;



        #region Movement values
        [Space]
        [Header("Movement parameters")]

        [Description("\nThe values you see below are actively used during gameplay. \nThey're overwritten when [controlState] changes or if the values are reloaded. \nIf you can't see them, set [displayActualValues] at the bottom to true\n", messageType = MessageType.Info)]
        
        [Tooltip("Should movement be disabled? This means that this script doesn't look at move inputs while true")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private bool _disableMovement = false;
        
        [Tooltip("Actual maximum horizontal velocity that's used. This can be changed with the CharacterMovementValues Scriptable Object \n\n[Units]/[Seconds] \n(If moving at maximum speed, your horizontal position changes by this value every second)")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _maxHorVelocity;
        
        [Tooltip("Acceleration for direction player is facing \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _acceleration;
        
        [Tooltip("Acceleration for directions player isn't facing (left, right, back) \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _control;
        
        [Tooltip("Deceleration when player doesn't input movement \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _brake;


        [Separator("Indirect values")]
        [Description("\nThe values below don't directly influence the current movement values. Either change [controlState] (by jumping/landing) or click the [Reload Values] button to reload values for current [controlState]\n", messageType = MessageType.Info)]

        [Tooltip("Maximum horizontal velocity \n\n[Units]/[Seconds] \n(If moving at maximum speed, your horizontal position changes by this value every second)")]
        [SerializeField] private float maxHorVelocity;
        
        [Tooltip("Acceleration for direction player is facing \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float groundAcceleration;
        
        [Tooltip("Acceleration for directions player isn't facing (left, right, back) \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float groundControl;
        
        [Tooltip("Deceleration when player doesn't input movement \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float groundBrake;

        
        [Tooltip("Acceleration for direction player is facing \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float airAcceleration;
        
        [Tooltip("Acceleration for directions player isn't facing (left, right, back) \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float airControl;
        
        [Tooltip("Deceleration when player doesn't input movement \n\n[Units]/[Seconds]^2 \n(Velocity changes by this value every second)")]
        [SerializeField] private float airBrake;
        #endregion


        #region Turn values
        [Space]
        [Header("Turn parameters")]

        
        [Tooltip("Amount of degrees per frame the player can rotate when standing still")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _minTurnSpeed = 1f;
        
        [Tooltip("Amount of degrees per frame the player can rotate when moving at top speed")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _maxTurnSpeed = 7.5f;


        [Separator("Indirect values")]

        [Tooltip("Do you want the player to rotate slower at lower velocity?")]
        [SerializeField] private bool enableMinTurnSpeed;

        [Tooltip("Amount of degrees per frame the player can rotate when standing still on the ground")]
        [SerializeField] private float minTurnSpeedGround = 1f;
        
        [Tooltip("Amount of degrees per frame the player can rotate when moving at top speed on the ground")]
        [SerializeField] private float maxTurnSpeedGround = 7.5f;
        
        [Tooltip("Amount of degrees per frame the player can rotate when airborne without horizontal speed")]
        [SerializeField] private float minTurnSpeedAir = 1f;
        
        [Tooltip("Amount of degrees per frame the player can rotate when airborne with top horizontal speed")]
        [SerializeField] private float maxTurnSpeedAir = 3f;
        #endregion


        #region Jump values
        [Space]
        [Header("Jump parameters")]


        [Tooltip("Should jumping be disabled? This means there's no way to jump")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private bool _disableJumping = false;
        
        [Tooltip("Actual jump height parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _jumpHeight = 2f;
        
        [Tooltip("Actual short jump multiplier parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _shortJumpMult = .5f;
        
        [Tooltip("Actual jump buffer parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _jumpBuffer = .2f;
        
        [Tooltip("Actual coyote time parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _coyoteTime = .2f;


        [Separator("Indirect values")]

        [Range(0.5f,5f)] [Tooltip("The height of the jump. 1 is 1 unity cube.")]
        [SerializeField] private float jumpHeight = 2f;
        
        [Range(0f, 1f)] [Tooltip("Variable jump height. If you let go of the jump button before reaching the peak of the jump, the velocity gets multiplied with this amount.\n\n0 = full stop to upwards velocity\n1 = no special changes to upwards velocity")]
        [SerializeField] private float shortJumpMult = .5f;


        [Range(0f, .5f)] [Tooltip("Not implemented yet!!! When you press jump while still in the air, you'll still jump if you land on the ground within [jumpBuffer] seconds")]
        [SerializeField] private float jumpBuffer = .2f;
        
        [Range(0f, .5f)] [Tooltip("Not implemented yet!!! When you fall off a platform, you can still jump for [coyoteTime] seconds")]
        [SerializeField] private float coyoteTime = .2f;
        #endregion


        #region Gravity values
        [Space]
        [Header("Gravity parameters")]


        [Tooltip("Should gravity be disabled? This makes it so the gravity values aren't used")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private bool _disableGravity = false;
        
        [Tooltip("Actual gravity parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _gravity = -9.81f;
        
        [Tooltip("Actual bonus gravity parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _bonusGravity = 1;
        
        [Tooltip("Actual terminal velocity parameter that's used. This can be changed with the CharacterMovementValues Scriptable Object")]
        [SerializeField, ConditionalField(nameof(displayActualValues)), ReadOnly(nameof(changeActualValues), true)] private float _terminalVelocity = 20f;


        [Separator("Indirect values")]

        [Tooltip("The modifier used to fake gravity. -9.81 is the default setting that is supposed to fake the real world gravity.")]
        [SerializeField] private float gravity = -9.81f;
        
        [Range(0f, 2f)] [Tooltip("Extra bit of gravity multiplier when falling. Should be subtle")]
        [SerializeField] private float bonusGravity = 1;
        
        [Range(0.1f, 30f)] [Tooltip("Max fall speed. Only applies for falling here, but realistically, it should apply in all directions. \nAccording to quora, a hamster can reach a terminal velocity on the order of 15-25 m/s")]
        [SerializeField] private float terminalVelocity = 20f;
        #endregion


        [Space]
        [Separator("Other")]



        [Header("Ground Check")]
        
        
        [Tooltip("Transform that is treated as a ground check. Please position at the character's feet.")]
        [SerializeField] private Transform groundCheck;
        
        [Range(0.1f,0.5f)][Tooltip("Radius of the ground checking sphere.")]
        [SerializeField] private float groundSphereRadius = 0.4f;
        
        [Tooltip("Layers that should be classified as ground.")]
        [SerializeField] private LayerMask groundMask;


        
        [Header("Debug Options")] 
        
        
        [Tooltip("Enables the debug logs.")]
        [SerializeField] private bool isDebug;
        
        [Tooltip("Enables the gizmo for the ground checking sphere.")]
        [SerializeField] private bool showGroundSphere;


        [Description("\nEnable the values below to see and temporarily change the current movement values. \nWATCH OUT: When [controlState] is changed or [Reload Values] is pressed, they'll get overridden with no way to recover them.\n", messageType = MessageType.Info)]
        
        [Tooltip("There's groundAcceleration and airAcceleration, but the value that's currently being used is hidden. Set to true to show them")]
        [SerializeField] private bool displayActualValues;
        
        [Tooltip("Allows you to change the actual values, but the change will only last until the value is loaded again")]
        [SerializeField] private bool changeActualValues;

        public bool Grounded => _isGrounded;
        public Vector2 MoveInput => _moveInput;
        
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        private Vector2 _moveInput;
        private InputAction _jumpInput;
        private bool _jumpButtonPressed;
        private bool _jumpButtonReleased;
        private bool _jumpVariableBuffer;

        private float _jumpBufferTimer;
        private float _coyoteJumpTimer;

        private CharacterPivot _pivot;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (groundCheck == null)
            {
                groundCheck = transform;
            }
            CheckGrounded();
            ReloadValues();
            
        }

        private void Update()
        {

            // Only knowing what buttons are pressed. Needs to be in Update
            CheckInputs();

        }

        private void FixedUpdate()
        {
            CheckGrounded();
           // _isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);
            //_isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(.5f, .1f, .5f), transform.rotation, groundMask);
            
            if (isDebug) SuperDebug.Log($"{transform.position}");
            
            // Making sure the downwards velocity doesn't go very high when grounded
            ResetVelocity();

            // Calculating horizontal movement
            CalculateMovement();

            CalculateJump();


            ApplyGravity();

            if (_controller.enabled) ApplyMovement();
        }

        private void CheckInputs()
        {
            // Automatically normalizes
            _moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();
            RotateInputToCamera();

            _jumpInput = InputSystem.actions["Jump"];
            if (_jumpInput.triggered) _jumpButtonPressed = true;
            if (_jumpInput.WasReleasedThisFrame()) _jumpButtonReleased = true;
        }

        private void CheckGrounded()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask, QueryTriggerInteraction.Ignore);

            if (_isGrounded)
            {
                _coyoteJumpTimer = _coyoteTime;
            }
            else
            {
                _coyoteJumpTimer = Mathf.Max(0f, _coyoteJumpTimer - Time.fixedDeltaTime);
            }

            // If controlState is the opposite of what it's supposed to be
            if (controlState == (_isGrounded ? ControlState.Airborne : ControlState.Grounded))
            {
                controlState = _isGrounded ? ControlState.Grounded : ControlState.Airborne;
                SetValuesToGroundedState();
            }

        }


        /// <summary>
        /// Starts with determining whether to use Acceleration, Control or Brake
        /// and moves velocity by that value to where you want to go.
        /// Also applies rotation
        /// </summary>
        private void CalculateMovement()
        {

            Vector2 horVel = new Vector2(_velocity.x, _velocity.z);
            Vector2 desiredVelocity = _moveInput * _maxHorVelocity;

            float speedChange;



            if (_moveInput.magnitude != 0)
            {
                // Looking how much the input direction matches the forward direction
                //Vector2 forwardVector = new Vector2(transform.forward.x, transform.forward.z).normalized;
                Vector2 forwardVector = transform.forward.ToVector2XZ().normalized;

                speedChange = CalculateSpeedChange(forwardVector, horVel, desiredVelocity);


                // Rotation
                if (_moveInput.magnitude != 0 && forwardVector != _moveInput.normalized)
                {
                    float desiredRotation = Vector2.SignedAngle(forwardVector, _moveInput);
                    transform.Rotate(-Mathf.MoveTowards(0, desiredRotation, GetTurnSpeed(horVel.magnitude)) * Vector3.up);
                }
            }
            else
            {
                speedChange = _brake;
                velocityState = VelocityState.Brake;
            }

            Vector2 a = Vector2.MoveTowards(horVel, desiredVelocity, speedChange * Time.fixedDeltaTime);

            _velocity.x = a.x;
            _velocity.z = a.y;


        }

        // Me when I use 40 lines of code for 7 real lines of code:
        private float CalculateSpeedChange(Vector2 pForwardVector, Vector2 pHorVel, Vector2 pDesiredVelocity)
        {
            velocityState = VelocityState.Control;
            // Direction
            // How much does the input direction match the forward direction?
            float forwardDot = Vector2.Dot(_moveInput, isLookForward ? pForwardVector : pHorVel.normalized);

            // If the input direction doesn't have a positive forward direction, we always use control
            // I don't know if this is better for performance or not, but shouldn't make a huge difference
            //if (forwardDot <= 0) return pControl;

          //  Debug.Log("CM Forward look vector: (" + pForwardVector.x + ", " + pForwardVector.y + ") Magnitude: (" + pForwardVector.magnitude + ")");

          //  Debug.Log("First check. Desired vel: " + pDesiredVelocity.magnitude + "  " + pDesiredVelocity + " (" + _moveInput + ")  \n Hor vel: " + pHorVel.magnitude + "  " + pHorVel + " (" + pHorVel/_maxHorVelocity + ")");
            // Include or exclude lower total speeds?
            if (requireBiggerTotalSpeed && pDesiredVelocity.magnitude < pHorVel.magnitude) return _control;            


            // Length to compare the forwardDot with
            // (lime, yellow. Difference is direction)
            float compareToValue = 0;
            // (purple, pink. Difference is direction)
            if (useVelocityTotal) compareToValue = pHorVel.magnitude;
            // (brown, gray. Difference is include or exclude innner circle)
            else if (useVelocityDirectional) compareToValue = Vector2.Dot(pHorVel, isLookForward ? pForwardVector : pHorVel.normalized);


            // This system doesn't account for if you want to use the length of the input for one direction
            // and length of speed for another

            //Debug.Log("Forward dot: " + forwardDot*maxHorVelocity + " (" + forwardDot + ").\nValue it's compared with: " + compareToValue);

           // Debug.Log("Second check. Move input dot: " + forwardDot * _maxHorVelocity + " (" + forwardDot + ")  \n value compared to: " + compareToValue + " (" + compareToValue/_maxHorVelocity + ")");
          //  Debug.LogError("Second check. Move input dot: " + forwardDot + "  \n value compared to: " + compareToValue/_maxHorVelocity + " (" + (Mathf.Abs(forwardDot) - Mathf.Abs(compareToValue/_maxHorVelocity))+ ")");
            // If length of forward is smaller than required threshold
            if (forwardDot < compareToValue / _maxHorVelocity - 0.001f) return _control;


            // Summary of what's going on:
            // If forwardDot is 0 or negative, pControl is guaranteed
            // Then direction is chosen: look direction or velocity direction?
            // Get length of input on chosen direction
            // Choose length to compare with for current velocity
            // Choose from the following: 0, length on chosen direction or total length
            // If input length on chosen direction is bigger, use acceleration
            // If current velocity length on chosen direction is bigger, use control

          //  Debug.Log("Last check (not really a check anymore)");
            velocityState = VelocityState.Acceleration;
            return _acceleration;
        }

        // Knowing how fast the player can rotate around
        private float GetTurnSpeed(float pCurrentVelocity)
        {
            if (!enableMinTurnSpeed) return _maxTurnSpeed;

            float percent = pCurrentVelocity / _maxHorVelocity;

            return _minTurnSpeed + (_maxTurnSpeed - _minTurnSpeed) * percent;
        }



        private void CalculateJump()
        {
            
            if (CheckJumpWithBuffer() && (_isGrounded || _coyoteJumpTimer > 0))
            {
                Jump();
            }


            // Variable jump height
            if (_jumpButtonReleased)
            {
                _jumpButtonReleased = false;
                
                DoVariableJumpHeight();
                
            }


        }

        private bool CheckJumpWithBuffer()
        {

            if (_jumpButtonPressed)
            {
                _jumpBufferTimer = _jumpBuffer;
                _jumpButtonPressed = false;

                // You (typically) can't both press and release a button in the same frame
                _jumpVariableBuffer = false;
                _jumpButtonReleased = false;
                return true;
            }

            _jumpBufferTimer = Mathf.Max(0, _jumpBufferTimer - Time.fixedDeltaTime);


            if (_jumpBufferTimer > 0)
            {
                if (_jumpButtonReleased)
                {
                    _jumpButtonReleased = false;
                    // Immediately apply variable jump height
                    _jumpVariableBuffer = true;
                }
                return true;
            }


            return false;
        }

        private void Jump()
        {
            _jumpBufferTimer = 0;
            _coyoteJumpTimer = 0;

            if (_disableJumping) return;
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

            if (_jumpVariableBuffer)
            {
                DoVariableJumpHeight();
            }
            
        }

        private void DoVariableJumpHeight()
        {
            if (_velocity.y > 0)
            {
                _velocity.y *= _shortJumpMult;

                _jumpVariableBuffer = false;
            }
        }


        private void ApplyGravity()
        {
            if (_disableGravity) return;
            // What if gravity changes? (like during climbing)
            // What if this part doesn't happen if the player is grounded?
            float g = _gravity;

            // Bonus gravity if falling down
            if (_velocity.y < 0) g *= _bonusGravity;


            // Increase for velocity is linear, so we just add gravity's acceleration since last frame
            g *= Time.fixedDeltaTime;

            // Alternative way of enforcing terminal velocity?
            //if (g > _velocity.y + terminalVelocity) g = _velocity.y + terminalVelocity;

            // Increase falling speed.
            _velocity.y += g;

            // Terminal velocity (Easy and robust method)
            if (_velocity.y < -_terminalVelocity) _velocity.y = -_terminalVelocity;

        }

        private void ApplyMovement()
        {
            _controller.Move(_velocity * Time.fixedDeltaTime);
        }


        // This creates a nasty slowdown effect upon landing. This is because the grounded sphere extends far below the player
        private void ResetVelocity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }


        private void RotateInputToCamera()
        {
            Vector3 cameraForward = GetCameraForward();

            // Looking forward = 0. Angle is in radians
            float angle = -Mathf.Atan2(cameraForward.x, cameraForward.z);

            // Rotating a vector by an angle: (cos(a) * x - sin(a) * y, cos(a) * y + sin(a) * x)
            float x = Mathf.Cos(angle) * _moveInput.x - Mathf.Sin(angle) * _moveInput.y;
            float y = Mathf.Cos(angle) * _moveInput.y + Mathf.Sin(angle) * _moveInput.x;

            _moveInput.x = x;
            _moveInput.y = y;

            if (_moveInput.magnitude > 1) _moveInput.Normalize();
        }

        // Public for MovementVisualisation
        public Vector3 GetCameraForward()
        {
            // TODO: Make better system for this!
            if (_pivot == null) TryGetComponent(out _pivot);

            return _pivot == null ? Vector3.forward : _pivot.PivotForward;
        }


        public void SetSpecialValues(CharacterMovementValues pValues)
        {
            specialValues = pValues;
            if (!SetValuesToSpecial())
            {
                Debug.LogError("Tried setting special values but it's null");
            }
        }

        public void RemoveSpecialValues()
        {
            specialValues = null;
            _disableMovement = false;
            _disableJumping = false;
            _disableGravity = false;
            controlState = ControlState.Grounded;
            SetValuesToGroundedState();
        }

        [ButtonMethod]
        private string ReloadValues()
        {
            // Returns false if no CharacterMovementValues is attached
            if (!SetValuesToSpecial()) SetValuesToGroundedState();
            
            return "Reloaded values";
        }


        #region Setting Values

        private bool SetValuesToSpecial()
        {
            if (specialValues == null) return false;

            _disableMovement = specialValues.GetDisableMovement();
            _disableJumping = specialValues.GetDisableJumping();
            _disableGravity = specialValues.GetDisableGravity();
            if (specialValues.GetResetVelocity()) _velocity = Vector3.zero;

            controlState = ControlState.Special;

            if (specialValues.GetUseMovementValues()) { SetMovementValuesToSpecial(); }
            else { SetMovementValuesToGroundedState(); }

            if (specialValues.GetUseTurnValues()) { SetTurnValuesToSpecial(); }
            else { SetTurnValuesToGroundedState(); }

            if (specialValues.GetUseJumpValues()) { SetJumpValuesToSpecial(); }
            else { SetJumpValuesToGroundedState(); }

            if (specialValues.GetUseGravityValues()) { SetGravityValuesToSpecial(); }
            else { SetGravityValuesToGroundedState(); }

            return true;
        }

        private void SetValuesToGroundedState()
        {
            SetMovementValuesToGroundedState();
            SetTurnValuesToGroundedState();
            SetJumpValuesToGroundedState();
            SetGravityValuesToGroundedState();
        }
        private void SetMovementValuesToGroundedState()
        {
            _maxHorVelocity = maxHorVelocity;
            _acceleration = _isGrounded ? groundAcceleration : airAcceleration;
            _control = _isGrounded ? groundControl : airControl;
            _brake = _isGrounded ? groundBrake : airBrake;
        }
        private void SetTurnValuesToGroundedState()
        {
            _minTurnSpeed = _isGrounded ? minTurnSpeedGround : minTurnSpeedAir;
            _maxTurnSpeed = _isGrounded ? maxTurnSpeedGround : maxTurnSpeedAir;
        }
        private void SetJumpValuesToGroundedState()
        {
            _jumpHeight = jumpHeight;
            _shortJumpMult = shortJumpMult;
            _jumpBuffer = jumpBuffer;
            _coyoteTime = coyoteTime;
        }
        private void SetGravityValuesToGroundedState()
        {
            _gravity = gravity;
            _bonusGravity = bonusGravity;
            _terminalVelocity = terminalVelocity;
        }

        private void SetMovementValuesToSpecial()
        {
            if (specialValues == null) return;

            _maxHorVelocity = specialValues.GetValueMaxHorVelocity();
            _acceleration = specialValues.GetValueAcceleration();
            _control = specialValues.GetValueControl();
            _brake = specialValues.GetValueBrake();
        }
        private void SetTurnValuesToSpecial()
        {
            if (specialValues == null) return;

            _minTurnSpeed = specialValues.GetValueMinTurnSpeed();
            _maxTurnSpeed = specialValues.GetValueMaxTurnSpeed();
        }
        private void SetJumpValuesToSpecial()
        {
            if (specialValues == null) return;

            _jumpHeight = specialValues.GetValueJumpHeight();
            _shortJumpMult = specialValues.GetValueShortJumpMult();
            _jumpBuffer = specialValues.GetValueJumpBuffer();
            _coyoteTime = specialValues.GetValueCoyoteTime();
        }
        private void SetGravityValuesToSpecial()
        {
            if (specialValues == null) return;

            _gravity = specialValues.GetValueGravity();
            _bonusGravity = specialValues.GetValueBonusGravity();
            _terminalVelocity = specialValues.GetValueTerminalVelocity();
        }
        #endregion


        public Vector2 GetPercentualVelocity()
        {
            return new Vector2(_velocity.x, _velocity.z) / _maxHorVelocity;
        }

        /// <summary>
        /// Returns one of the booleans that incluence whether control or acceleration is used
        /// </summary>
        /// <param name="pId">0 = requireBiggerTotalSpeed, 1 = isLookForward, 2 = useVelocityTotal, 3 = useVelocityDirectional</param>
        /// <returns>0 = requireBiggerTotalSpeed, 1 = isLookForward, 2 = useVelocityTotal, 3 = useVelocityDirectional</returns>
        public bool GetBoolControlAcceleration(int pId)
        {
            switch (pId)
            {
                case 0: return requireBiggerTotalSpeed;
                case 1: return isLookForward;
                case 2: return useVelocityTotal;
                case 3: return useVelocityDirectional;
                default:
                    Debug.LogWarning("Incorrect value given");
                    return false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (showGroundSphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundSphereRadius);
            }

            //TODO?: Add Input and Velocity arrows?
        }

        public CharacterController GetCharacterController()
        {
            return _controller;
        }




        public void DoSpringJump(float pMinHeight, float pVelHeight, float pMaxHeight, bool pClampMinHeight, float pHorVelMult, float pHorInputBoost, float pMaxHorVelOutput)
        {
            if (_velocity.y > 0) return;

            float bounceHeight = Mathf.Min(pVelHeight * -_velocity.y / _terminalVelocity, pMaxHeight);

            if (pClampMinHeight)
            {
                if (bounceHeight < pMinHeight) bounceHeight = pMinHeight;
            }
            else
            {
                bounceHeight = Mathf.Min(pMinHeight + bounceHeight, pMaxHeight);
            }

            _velocity.y = Mathf.Sqrt(bounceHeight * -2f * _gravity);






            Vector2 horVel = new Vector2(_velocity.x, _velocity.z);

            horVel *= pHorVelMult;

            Vector2 inputBoost = _moveInput * pHorInputBoost;

            horVel += inputBoost;

            if (pMaxHorVelOutput > 0 && horVel.magnitude > pMaxHorVelOutput)
            {
                horVel = horVel.normalized * pMaxHorVelOutput;
            }

            _velocity.x = horVel.x;
            _velocity.z = horVel.y;

        }



    }
}