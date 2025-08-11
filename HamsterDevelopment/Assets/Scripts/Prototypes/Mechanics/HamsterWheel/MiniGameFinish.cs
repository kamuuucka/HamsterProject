using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    public class MiniGameFinish : MonoBehaviour
    {
        private MiniGame _miniGame;

        private void Start()
        {
            _miniGame = GetComponentInParent<MiniGame>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _miniGame.CollisionDetected(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _miniGame.CollisionEnded(other);
        }
    }
}
