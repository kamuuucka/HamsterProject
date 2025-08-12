using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    /// <summary>
    /// Step for the Hamster Wheel Mini Game.
    /// </summary>
    public class HwStep : MonoBehaviour
    {
        [SerializeField] private float speed = 200f;
    
        private RectTransform _rectTransform;
    
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localPosition = Vector3.zero;  
        }
        
        private void Update()
        {
            var localPosition = _rectTransform.localPosition;
            localPosition = new Vector2(localPosition.x, localPosition.y - speed * Time.deltaTime);
            _rectTransform.localPosition = localPosition;
        }
    }
}
