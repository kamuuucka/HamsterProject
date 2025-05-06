using System;
using Prototypes.Character;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterDeath : MonoBehaviour
{
    [Tooltip("Events that will happen on character's death.")]
    [SerializeField] private UnityEvent onCharacterDeath;
    [Tooltip("Layers that will cause character's death.")]
    [SerializeField] private LayerMask deathLayers;
    [Tooltip("Height at which character will die.")][Range(0,-10)]
    [SerializeField] private float deadlyHeight = -5f;
    [Tooltip("Set if the character should die by height.")]
    [SerializeField] private bool dieByHeight = true;
    [Space(10)]
    [SerializeField] private bool isDebug;
    [Tooltip("Show the gizmo for the deadly height value.")]
    [SerializeField] private bool showDeadlyHeight;
    private CharacterMovement _cm;

    private void Start()
    {
        _cm = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (dieByHeight && !_cm.Grounded && transform.position.y <= deadlyHeight)
        {
            onCharacterDeath?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDebug) Debug.Log("Colliding!");
        if ((deathLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            onCharacterDeath?.Invoke();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (showDeadlyHeight)
        {
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawSphere(new Vector3(position.x, deadlyHeight, position.z), 0.2f);
        }
    }
}
