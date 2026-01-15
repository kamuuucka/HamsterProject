using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Prototypes.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {

        [Header("Horizontal Movement")]

        [Header("Choices for Control or Acceleration")]

        [Tooltip("Used for deciding control or acceleration. \nDo you want to require total speed of desired velocity to be bigger than total speed of current velocity?")]
        [SerializeField] private bool requireBiggerTotalSpeed;

        [Tooltip("Used for deciding control or acceleration. \nWhen changing move direction, should the calculations use the look direction (true) or the velocity direction (false)?")]
        [SerializeField] private bool isLookForward;

        [Tooltip("Used for deciding control or acceleration. \nShould the length of the desired velocity on the chosen direction be bigger than the total length of the current velocity?")]
        [SerializeField] private bool useVelocityTotal;

        [Tooltip("Used for deciding control or acceleration. \nShould the length of the desired velocity on the chosen direction be bigger than the length of the current velocity on the chosen direction? \n This is ignored if above is true.")]
        [SerializeField] private bool useVelocityDirectional;


        [Header("Speed change parameters")]

        [Tooltip("Maximum horizontal velocity")]
        [SerializeField] private float maxHorVelocity;

        [Tooltip("Acceleration for direction player is facing")]
        [SerializeField] private float groundAcceleration;
        [Tooltip("Acceleration for directions player isn't facing (left, right, back)")]
        [SerializeField] private float groundControl;
        [Tooltip("Deceleration when player doens't input movement")]
        [SerializeField] private float groundBrake;

        [Tooltip("Acceleration for direction player is facing")]
        [SerializeField] private float airAcceleration;
        [Tooltip("Acceleration for directions player isn't facing (left, right, back)")]
        [SerializeField] private float airControl;
        [Tooltip("Deceleration when player doens't input movement")]
        [SerializeField] private float airBrake;


        [Header("Rotation speed parameters")]

        [Tooltip("Do you want the player to rotate slower at lower velocity?")]
        [SerializeField] private bool enableMinTurnSpeed;
        [Tooltip("Amount of degrees per frame the player can rotate when standing still")]
        [SerializeField] private float minTurnSpeed;
        [Tooltip("Amount of degrees per frame the player can rotate when moving at top speed")]
        [SerializeField] private float maxTurnSpeed;


        [Header("Jump related")]


        [Tooltip("The modifier used to fake gravity. -9.81 is the default setting that is supposed to fake the real world gravity.")]
        [SerializeField] private float gravity = -9.81f;
        [Range(0.5f,5f)][Tooltip("The height of the jump. 1 is 1 unity cube.")]
        [SerializeField] private float jumpHeight = 2f;

        [Range(0f, 1f)]
        [Tooltip("Variable jump height. If you let go of the jump button before reaching the peak of the jump, the velocity gets multiplied with this amount.\n\n0 = full stop to upwards velocity\n1 = no special changes to upwards velocity")]
        [SerializeField] private float shortJumpMult = .5f;
        [Range(0f, 2f)]
        [Tooltip("Extra bit of gravity multiplier when falling. Should be subtle")]
        [SerializeField] private float bonusGravity = 1;
        [Range(0.1f, 30f)]
        [Tooltip("Max fall speed. Only applies for falling here, but realistically, it should apply in all directions. \nAccording to quora, a hamster can reach a terminal velocity on the order of 15-25 m/s")]
        [SerializeField] private float terminalVelocity = 20f;



        [Header("Other")]

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

        public bool Grounded => _isGrounded;
        
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        private Vector2 _moveInput;
        private InputAction _jumpInput;

        private CharacterPivot _pivot;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            
            if (groundCheck == null)
            {
                groundCheck = transform;
            }
            
        }

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);
            
            if (isDebug) SuperDebug.Log($"{transform.position}");
            
            // ??????
            ResetVelocity();

            // Only knowing what buttons are pressed. Needs to be in Update
            CheckInputs();

            // Calculating horizontal movement
            CalculateMovement(); 

            // Calculating vertical movement
            CheckJump();

            ApplyGravity();

            if (_controller.enabled) ApplyMovement();


        }

        private void CheckInputs()
        {
            // Automatically normalizes
            _moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();
            RotateInputToCamera();

            _jumpInput = InputSystem.actions["Jump"];
        }


        // Horizontal movement
        private void CalculateMovement()
        {
            // Using the right numbers for air and ground
            float acceleration = _isGrounded ? groundAcceleration : airAcceleration;
            float control = _isGrounded ? groundControl : airControl;
            float brake = _isGrounded ? groundBrake : airBrake;


            Vector2 horVel = new Vector2(_velocity.x, _velocity.z);
            Vector2 desiredVelocity = _moveInput * maxHorVelocity;

            float speedChange;



            if (_moveInput.magnitude != 0)
            {
                // Looking how much the input direction matches the forward direction
                Vector2 forwardVector = new Vector2(transform.forward.x, transform.forward.z).normalized;

                speedChange = CalculateSpeedChange(forwardVector, horVel, desiredVelocity, acceleration, control);


                // Rotation
                if (_moveInput.magnitude != 0 && forwardVector != _moveInput.normalized)
                {
                    float desiredRotation = Vector2.SignedAngle(forwardVector, _moveInput);
                    transform.Rotate(-Mathf.MoveTowards(0, desiredRotation, GetTurnSpeed(horVel.magnitude)) * Vector3.up);
                }
            }
            else
            {
                speedChange = brake;
            }

            Vector2 a = Vector2.MoveTowards(horVel, desiredVelocity, speedChange);

            _velocity.x = a.x;
            _velocity.z = a.y;


        }

        // Me when I use 40 lines of code for 7 real lines of code:
        private float CalculateSpeedChange(Vector2 pForwardVector, Vector2 pHorVel, Vector2 pDesiredVelocity, float pAcceleration, float pControl)
        {
            // Direction
            // How much does the input direction match the forward direction?
            float forwardDot = Vector2.Dot(_moveInput, isLookForward ? pForwardVector : pHorVel.normalized);

            // If the input direction doesn't have a positive forward direction, we always use control
            // I don't know if this is better for performance or not, but shouldn't make a huge difference
            //if (forwardDot <= 0) return pControl;


            // Include or exclude lower total speeds?
            if (requireBiggerTotalSpeed && pDesiredVelocity.magnitude < pHorVel.magnitude) return pControl;            


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

            // If length of forward is smaller than required threshold
            if (forwardDot * maxHorVelocity <= compareToValue) return pControl;


            // Summary of what's going on:
            // If forwardDot is 0 or negative, pControl is guaranteed
            // Then direction is chosen: look direction or velocity direction?
            // Get length of input on chosen direction
            // Choose length to compare with for current velocity
            // Choose from the following: 0, length on chosen direction or total length
            // If input length on chosen direction is bigger, use acceleration
            // If current velocity length on chosen direction is bigger, use control


            return pAcceleration;
        }

        // Knowing how fast the player can rotate around
        //TODO?: Have different turn speeds in air and on ground?
        private float GetTurnSpeed(float pCurrentVelocity)
        {
            if (!enableMinTurnSpeed) return maxTurnSpeed;

            float percent = pCurrentVelocity / maxHorVelocity;
            return minTurnSpeed + (maxTurnSpeed - minTurnSpeed) * percent;
        }



        private void CheckJump()
        {
            // TODO: Come up with convenient way to disable jumping
            bool isJumping = _jumpInput.triggered;

            float newValue = _velocity.y;

            // Jumping
            if (_isGrounded && isJumping)
            {
                // This way, the character jumps exactly [jumpHeight] units high
                newValue = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Variable jump height
            if (_jumpInput.WasReleasedThisFrame() && _velocity.y > 0)
            {
                newValue = _velocity.y * shortJumpMult;
            }

            _velocity.y = newValue;
        }



        private void ApplyGravity()
        {
            // What if gravity changes? (like during climbing)
            // What if this part doesn't happen if the player is grounded?
            float g = gravity;

            // Bonus gravity if falling down
            if (_velocity.y < 0) g *= bonusGravity;


            // Increase for velocity is linear, so we just add gravity's acceleration since last frame
            g *= Time.deltaTime;

            // Alternative way of enforcing terminal velocity?
            //if (g > _velocity.y + terminalVelocity) g = _velocity.y + terminalVelocity;

            // Increase falling speed.
            _velocity.y += g;

            // Terminal velocity (Easy and robust method)
            if (_velocity.y < -terminalVelocity) _velocity.y = -terminalVelocity;

        }

        private void ApplyMovement()
        {
            _controller.Move(_velocity * Time.deltaTime);
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

        }

        private Vector3 GetCameraForward()
        {
            // TODO: Make better system for this!
            if (_pivot == null) TryGetComponent(out _pivot);

            return _pivot == null ? Vector3.forward : _pivot.PivotForward;
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
    }
}