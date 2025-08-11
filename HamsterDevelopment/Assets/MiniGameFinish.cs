using System;
using UnityEngine;

public class MiniGameFinish : MonoBehaviour
{
    private Minigame _minigame;

    private void Start()
    {
        _minigame = GetComponentInParent<Minigame>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _minigame.CollisionDetected(this);
    }
}
