using UnityEngine;
using UnityEngine.Serialization;

namespace Prototypes.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
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

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

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
            
            if (isDebug) SuperDebug.LogBool(_isGrounded);
            
            ResetVelocity();
            
            MoveCharacter();
            
            if (_isGrounded && Input.GetButtonDown("Jump"))
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            
            ApplyGravity();
        }

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
    }
}