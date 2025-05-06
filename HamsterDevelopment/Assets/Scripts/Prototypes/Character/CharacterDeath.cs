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
    private CharacterMovement _cm;

    private void Start()
    {
        _cm = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (!_cm.Grounded && transform.position.y <= deadlyHeight)
        {
            onCharacterDeath?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == deathLayers)
        {
            onCharacterDeath?.Invoke();
        }
    }
}
