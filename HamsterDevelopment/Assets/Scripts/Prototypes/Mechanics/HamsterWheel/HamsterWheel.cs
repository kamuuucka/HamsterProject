using MyBox;
using Prototypes.Character;
using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    /// <summary>
    /// The Hamster Wheel object. Makes sure to attach the player in a correct way and detach when the game is finished.
    /// </summary>
    public class HamsterWheel : MonoBehaviour
    {
        [Tooltip("The point where the hamster will be attached. The hamster will take both position and rotation from it.")]
        [SerializeField] private Transform attachPoint;
        [Space(10)]
        [SerializeField] private bool isDebug;

        private Vector3 _lastKnownPosition;
        private Quaternion _lastKnownRotation;


        [SerializeField, ReadOnly] private bool gameStarted = false;

        [SerializeField] private Transform wheelTransform;
        
        
        
        
        /// <summary>
        /// Attaches the object with the CharacterMovement script to the AttachPoint.
        /// </summary>
        /// <param name="player">Object that has the Character Movement script.</param>
        public void AttachToWheel(CharacterMovement player)
        {
            if (isDebug) SuperDebug.Log("Hamster attached!");

            var playerTransform = player.transform;
            
            _lastKnownPosition = playerTransform.position;
            _lastKnownRotation = playerTransform.rotation;
            
            playerTransform.position = attachPoint.position;
            playerTransform.localRotation = attachPoint.localRotation;
            
            player.GetCharacterController().enabled = false;
        }

        
        
        

        public void StartGame()
        {
            gameStarted = true;
        }

        public void EndGame()
        {
            gameStarted = false;
        }
        

        /// <summary>
        /// Detaches teh object with the CharacterMovement script from the AttachPoint.
        /// </summary>
        /// <param name="player">Object that has the Character Movement script.</param>
        public void DetachFromWheel(CharacterMovement player)
        {
            if (isDebug) SuperDebug.Log("Hamster detached!");
            
            var playerTransform = player.transform;
            
            playerTransform.position = _lastKnownPosition;
            playerTransform.rotation = _lastKnownRotation;
            
            player.GetCharacterController().enabled = true;
        }
    }
}
