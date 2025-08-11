using Prototypes.Character;
using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    /// <summary>
    /// The Hamster Wheel object. Makes sure to attach the player in a correct way and detach when the game is finished.
    /// </summary>
    public class HamsterWheel : MonoBehaviour
    {
        [SerializeField] private Transform attachPoint;

        private Transform _lastKnownTransform;

        /// <summary>
        /// Attaches the object with the CharacterMovement script to the AttachPoint.
        /// </summary>
        /// <param name="player">Object that has the Character Movement script.</param>
        public void AttachToWheel(CharacterMovement player)
        {
            _lastKnownTransform = player.transform;
            player.transform.position = attachPoint.position;
            player.transform.localRotation = attachPoint.localRotation;
            player.GetCharacterController().enabled = false;
        }
    }
}
