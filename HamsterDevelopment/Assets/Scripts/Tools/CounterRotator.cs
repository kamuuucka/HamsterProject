using UnityEngine;

public class CounterRotator : MonoBehaviour
{
   [SerializeField] private Transform _target;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
