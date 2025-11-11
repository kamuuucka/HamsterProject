using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Prototypes.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {

        [Header("Brams version")]

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
        [Tooltip("THIS IS PROBABLY USELESS Should Unity smoothen the transition from no input to input? \nThis is a huge difference for acceleration. Having this disabled basically means there's an extra build-up and build-down for acceleration and things may not work properly")]
        [SerializeField] private bool useRawInputs;


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


        [Header("Kama's stuff")]

        [Header("Movement Settings")]
        [Range(3f,10f)][Tooltip("The speed that the character moves with.")]
        [SerializeField] private float moveSpeed = 5f;
        [Tooltip("The modifier used to fake gravity. -9.81 is the default setting that is supposed to fake the real world gravity.")]
        [SerializeField] private float gravity = -9.81f;
        [Range(0.5f,5f)][Tooltip("The height of the jump. 1 is 1 unity cube.")]
        [SerializeField] private float jumpHeight = 2f;
    
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
            
            ResetVelocity();
            CheckInputs();
            if (_controller.enabled) CalculateMovementv2(); // MoveCharacter();
            
            if (_isGrounded && Input.GetButtonDown("Jump"))
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            
            if (_controller.enabled) ApplyGravity();
        }

        private void CheckInputs()
        {
            // Automatically normalizes
            _moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();
            //Debug.Log(_moveInput);
            // Take camera rotation into account?
        }


        private void CalculateMovementv2()
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

                /*float forwardDot = Vector2.Dot(_moveInput, isLookForward ? forwardVector : horVel);

                // If input is forward and the input going forward is bigger than current speed
                //if (forwardDot > 0 && forwardDot * maxHorVelocity > horVel.magnitude) speedChange = acceleration;
                // If input is forward and desired new speed is bigger than the current speed
                if (forwardDot > 0 && desiredVelocity.magnitude > horVel.magnitude) speedChange = acceleration;
                else speedChange = control;*/


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

            Debug.Log("Speed change: "+speedChange);

            Vector2 a = Vector2.MoveTowards(horVel, desiredVelocity, speedChange);

            _velocity.x = a.x;
            _velocity.z = a.y;


        }

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

            Debug.Log("Forward dot: " + forwardDot*maxHorVelocity + " (" + forwardDot + ").\nValue it's compared with: " + compareToValue);

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



        // Probably not using this. It uses the old input system
        private void CalculateMovement()
        {
            // Using the right numbers for air and ground
            float acceleration = _isGrounded ? groundAcceleration : airAcceleration;
            float control = _isGrounded ? groundControl : airControl;
            float brake = _isGrounded ? groundBrake : airBrake;


            // When the camera can rotate, add the camera's rotation to this
            /*Vector2 moveInput = useRawInputs ? 
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : 
                new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (moveInput.magnitude > 1) moveInput.Normalize();*/
            Vector2 desiredVelocity = _moveInput * maxHorVelocity;
            Vector2 moveOutput = Vector2.zero;


            if (_moveInput.magnitude != 0)
            {
                Vector2 forwardVector = new Vector2(transform.forward.x, transform.forward.z).normalized;
                Vector2 rightVector = new Vector2(transform.right.x, transform.right.z).normalized;

                // How much does the input go forward?
                float forwardDot = Vector2.Dot(_moveInput, forwardVector);
                // How much does the input go to the side?
                float rightDot = Vector2.Dot(_moveInput, rightVector);

                // If move input matches foward direction, use acceleration
                if (forwardDot > 0)
                {
                    moveOutput += forwardDot * acceleration * forwardVector;
                }
                // Any input direction that isn't forward uses control
                else moveOutput += forwardDot * control * forwardVector;
                moveOutput += rightDot * control * rightVector;


                // FIX THIS: DOESN'T WORK AFTER ROTATING
                // Relevant when switching direction while moving
                if (Mathf.Abs(moveOutput.x) <= 0.001f && _moveInput.x == 0)
                {
                    moveOutput.x = brake;
                }
                if (Mathf.Abs(moveOutput.y) <= 0.001f && _moveInput.y == 0)
                {
                    moveOutput.y = brake;
                }

                // Capping speed
                if (moveOutput.magnitude > maxHorVelocity)
                {
                    moveOutput = moveOutput.normalized * maxHorVelocity;
                }

                // Does the character need to rotate?
                if (_moveInput.magnitude != 0 && forwardVector != _moveInput.normalized)
                {
                    float desiredRotation = Vector2.SignedAngle(forwardVector, _moveInput);
                    transform.Rotate(-Mathf.MoveTowards(0, desiredRotation, GetTurnSpeed(moveOutput.magnitude)) * Vector3.up);
                }

            }
            else
            {
                // No input = brake
                moveOutput += new Vector2(_velocity.x, _velocity.z).normalized * brake;
            }


            Debug.Log(Vector2.SignedAngle(Vector2.up, moveOutput));

            _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, Mathf.Abs(moveOutput.x));
            _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.y, Mathf.Abs(moveOutput.y));
            

        }

        private float GetTurnSpeed(float pCurrentVelocity)
        {
            if (!enableMinTurnSpeed) return maxTurnSpeed;

            float percent = pCurrentVelocity / maxHorVelocity;
            return minTurnSpeed + (maxTurnSpeed - minTurnSpeed) * percent;
        }


        // Definitely not using this. This is Kama's version
        private void MoveCharacter()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            
            _controller.Move(move * (moveSpeed * Time.deltaTime));
        }

        private void ApplyGravity()
        {
            _velocity.y += gravity * Time.deltaTime;
            
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void ResetVelocity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (showGroundSphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundSphereRadius);
            }
        }

        public CharacterController GetCharacterController()
        {
            return _controller;
        }
    }
}