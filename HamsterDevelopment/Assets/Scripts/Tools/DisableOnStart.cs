using System;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
    [SerializeField] private bool disableOnStart = true;
    private void Start()
    {
        if (disableOnStart) enabled = false;
    }
}
