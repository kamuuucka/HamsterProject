using System;
using UnityEngine;

public class HamsterBall : MonoBehaviour
{
    [SerializeField] private bool canTurn;
    [SerializeField] private KeyCode hamsterBall = KeyCode.Alpha1;
    [SerializeField] private GameObject hamsterPrefab;
    [SerializeField] private GameObject ballPrefab;

    private bool _isBall;
    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(hamsterBall))
        {
            if (!_isBall) TurnIntoBall();
            else if (_isBall) TurnBack();
        }
    }

    private void TurnIntoBall()
    {
        if (canTurn)
        {
            _isBall = true;
            hamsterPrefab.SetActive(false);
            ballPrefab.SetActive(true);
            _characterController.radius = 1f;
        }
    }

    private void TurnBack()
    {
        if (canTurn)
        {
            _isBall = false;
            hamsterPrefab.SetActive(true);
            ballPrefab.SetActive(false);
            _characterController.radius = 0.5f;
        }
    }

    public void CanTurnOn()
    {
        canTurn = true;
    }
}
