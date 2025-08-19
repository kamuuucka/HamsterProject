using UnityEngine;

namespace Tools
{
    public class DisableOnStart : MonoBehaviour
    {
        [SerializeField] private bool disableOnStart = true;
        private void Start()
        {
            if (disableOnStart) gameObject.SetActive(false);
        }
    }
}
