using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototypes.Mechanics.BreakableWall
{
    [RequireComponent(typeof(Rigidbody))]
    public class Debris :MonoBehaviour
    {
        [SerializeField] private float explosionIntensity;
        [SerializeField] private float minLifetime;
        [SerializeField] private float maxLifetime;
        [SerializeField] private Vector3 minimumDirectionAngle;
        [SerializeField] private Vector3 maximumDirectionAngle;
        private void OnEnable()
        {
            var direction = new Vector3(Random.Range(minimumDirectionAngle.x, maximumDirectionAngle.x),Random.Range(minimumDirectionAngle.y, maximumDirectionAngle.y),Random.Range(minimumDirectionAngle.z, maximumDirectionAngle.z));
            GetComponent<Rigidbody>().AddForce(direction * explosionIntensity, ForceMode.VelocityChange);
            Destroy(gameObject, Random.Range(minLifetime, maxLifetime));
            
        }
    }
}