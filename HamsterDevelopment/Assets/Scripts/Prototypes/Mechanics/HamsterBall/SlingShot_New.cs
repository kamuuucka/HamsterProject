using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShot_New : MonoBehaviour
{
    [SerializeField] private float _maxForce = 5;
    [SerializeField] private float _forceIncrements = .1f;
    [SerializeField, Tooltip("Essentially we're working with big numbers and the direction is accurately portrayed. This means if we show a ration of 1:1 you pull yourself off screen. 10 here means youre going 10x as far as shown on the string.")] 
    private float _stringTensionMultiplier = 10f;
    [SerializeField] private float cooldown = 5f;
    private bool onCooldown = false;
    private float cooldownTimer = 0;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private GameObject _minigameCamera;
    [SerializeField] private Transform _seatTransform;
    [SerializeField] private Transform _arrowTransform;
    [SerializeField] private float maxArrowScale = 5;
    
    [SerializeField] private InputActionAsset _actionAsset;
    private bool inSlingShot = false;

    private Vector3 ballOffset;
    private Vector3 forceVector = Vector3.zero;
    private HamsterBallMovement  _hamsterBallMovementHolder;
    private Vector3 localSeatPosition = Vector3.zero;


    [SerializeField] private float xNoiseAmplifier = 1;
    [SerializeField] private float xNoiseSpeed = 1;
    [SerializeField] private float yNoiseAmplifier = 1;
    [SerializeField] private float yNoiseSpeed = 1;
    private Vector3 noiseVector = Vector3.zero;
    
    private void Start()
    {
        localSeatPosition = _seatTransform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(onCooldown) return;
        if (other.CompareTag(_playerTag) && !inSlingShot)
        {
            if(_hamsterBallMovementHolder == null)
                _hamsterBallMovementHolder = other.GetComponent<HamsterBallMovement>();
            
            _hamsterBallMovementHolder.AllowMovement = false;
           
            other.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            ballOffset = other.transform.position - _seatTransform.position;
            _minigameCamera.SetActive(true);
           
            inSlingShot = true;
        }
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldown)
            {
                cooldownTimer = 0;
                onCooldown = false;
            }
        }
        if (inSlingShot)
        {
            
            Vector2 input = _actionAsset.FindAction("Move", true).ReadValue<Vector2>();
            if (input != Vector2.zero && input.y < 0)
            {
                // pulling back

                float horizontalFactor = 0.5f; // tweak for less/more side influence

                Vector3 localPull =
                    transform.forward * input.y +          // forward/back (inverted)
                    transform.right * input.x * horizontalFactor; // horizontal aim

                Vector3 forceInput = localPull * _forceIncrements * Time.deltaTime;
                
                forceVector += forceInput;
                forceVector = Vector3.ClampMagnitude(forceVector, _maxForce); // optional but recommended

                if (forceVector.magnitude >= _maxForce)
                {
                    //Start noise;
                    noiseVector.x = Mathf.Sin(Time.time * xNoiseSpeed) * xNoiseAmplifier;
                    noiseVector.y = Mathf.Abs(Mathf.Cos(Time.time * yNoiseSpeed) * yNoiseAmplifier) ;
                }
                Debug.Log($"{gameObject.name}, total force: {forceVector} total Noise: {noiseVector}");
                _seatTransform.localPosition = localSeatPosition + (forceVector * (_stringTensionMultiplier / 100));
                _hamsterBallMovementHolder.transform.position = _seatTransform.position + ballOffset;
                SetArrowScale();
            }
            else if (input.y > 0)
            {
                // Cancel
                _seatTransform.localPosition = localSeatPosition;
                _hamsterBallMovementHolder.AllowMovement = true;

                forceVector = Vector3.zero;
                noiseVector = Vector3.zero;
                SetArrowScale();
            }
            else if (input == Vector2.zero && forceVector != Vector3.zero)
            {
                // Release
                _seatTransform.localPosition = localSeatPosition;

                Vector3 translatedForce = (forceVector) * -1 + noiseVector;
                translatedForce = Vector3.ClampMagnitude(translatedForce, _maxForce);
                _hamsterBallMovementHolder.AllowMovement = true;
                _hamsterBallMovementHolder.SetVelocity(translatedForce);
                _hamsterBallMovementHolder.SetPivotToDirection(transform.forward);
                Debug.Log($"{this.gameObject.name}: Releasing with force vector: {translatedForce}");
                noiseVector = Vector3.zero;
                forceVector = Vector3.zero;
                SetArrowScale();
            }
            

                
        }
      
        
    }

    private void SetArrowScale()
    {
        Vector3 lScale = _arrowTransform.localScale;
        lScale.z = Mathf.Lerp(0, maxArrowScale, (1/_maxForce * forceVector.magnitude));
        _arrowTransform.localScale =  lScale;
        _arrowTransform.LookAt(_arrowTransform.position + ((forceVector.normalized ) * -1 + noiseVector));
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag) && _hamsterBallMovementHolder != null)
        {
            _hamsterBallMovementHolder.HardSetPivot(transform.forward);
            _minigameCamera.SetActive(false);
            _hamsterBallMovementHolder.AllowMovement =  true;
            _hamsterBallMovementHolder.ReleaseHardPivot();
            
            _hamsterBallMovementHolder = null;
           
            inSlingShot = false;
            onCooldown = true;
        }
    }
}
