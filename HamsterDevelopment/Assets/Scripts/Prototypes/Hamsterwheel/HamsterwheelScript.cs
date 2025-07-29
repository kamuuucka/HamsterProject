using Unity.VisualScripting;
using UnityEngine;

public class HamsterwheelScript : MonoBehaviour
{
    private bool _interactable = false;
    [SerializeField] private GameObject _miniGame;
    private GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_interactable && Input.GetKeyDown(KeyCode.E))
        {
            _interactable = false;
            Debug.Log("registered E");
            Instantiate(_miniGame);
            Player.transform.parent = transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _interactable = true;
            Player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _interactable = false;
        }
    }
}
