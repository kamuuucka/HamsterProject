using System;
using System.Collections.Generic;
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
        _minigame.CollisionDetected(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _minigame.CollisionEnded(other);
    }
}
