using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    /// <summary>
    /// Class detecting collisions between steps and finish line for the hamster wheel mini game.
    /// </summary>
    public class HwMiniGameFinish : MonoBehaviour
    {
        private HwMiniGame _hwMiniGame;

        private void Start()
        {
            _hwMiniGame = GetComponentInParent<HwMiniGame>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _hwMiniGame.CollisionDetected(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _hwMiniGame.CollisionEnded(other);
        }
    }
}
