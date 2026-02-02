using MyBox;
using Prototypes.Character;
using UnityEngine;
using UnityEngine.UI;

public class MovementVisualisation : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool isFrozen;
    [SerializeField, ReadOnly] private bool hideInnerCircle;
    [Tooltip("True -> Going north in world space shows going up in screen space. Rotating the camera doesn't impact orientation. (the inputs on keyboard/controller appear rotated on screen by the same rotation the camera has) \n\nFalse -> Pressing left on controls means direction points left in screen space. This rotates to make the [camera forward vector] point to up in screen space. (the inputs on keyboard/controller are directly displayed on screen)")]
    [SerializeField] private bool isNorthUp;
    [SerializeField] private float scale = 100;

    [SerializeField] CharacterMovement cMovement;
    [Tooltip("Send a message every frame where the seperately calculated velocity type doesn't match the character's velocity type")]
    [SerializeField] private bool showDebugMessages;


    [Header("The visuals")]
    [SerializeField] RectTransform playerInputImage;
    [SerializeField] RectTransform percentualVelocityImage;
    [SerializeField] RectTransform speedCircleImage;
    [SerializeField] RectTransform velocityDirectionArrow;
    [SerializeField] RectTransform lookDirectionArrow;
    [SerializeField] Image velocityTypeIndicator;


    [Header("Filled area stuff")]
    [SerializeField] Image innerCircle;
    [SerializeField] RectTransform betweenMasks;

    [SerializeField] RectTransform filledArea;


    [Header("To look at")]

    [SerializeField] private float velocityAngle;
    [SerializeField] private float lookAngle;

    Vector2 playerInputVector;
    Vector2 percentualVelocityVector;


    void OnEnable()
    {
        isFrozen = false;
        if (cMovement == null)
        {
            isFrozen = true;
            Debug.LogError("Character Movement not defined in MovementVisualisation");
            gameObject.SetActive(false);
        }
    }


    // Everything is visual, probably no need to calculate between frames
    void Update()
    {
        if (isFrozen) return;
        ResetRotation();
        SetInput();
        SetPercVelocity();
        SetSpeedCircle();
        SetVelocityArrow();
        SetLookArrow();
        CalculateFilledArea();
        GetVelocityType();
        CheckOwnVelocityType();
        if (!isNorthUp) UndoCameraRotation();


        //Vector2.an

        // Get filledArea position
        // playerInput.Dot(position)
        // compare magnitude of Dot to magnitude of position
        // if Dot bigger -> Acceleration
        // position bigger -> Control
        // Except this doesn't always work due to inner circle

        /*Rect rect = filledArea.rect;
        if (rect.Contains(playerInputVector))
        {
            if (cMovement.velocityState == CharacterMovement.VelocityState.Control)
            {
                Debug.LogWarning("THIS IS NOT RIGHT");
                Debug.Log(rect.ToString() + "   " + playerInputVector);
                //isFrozen = true;
            }
            else if (cMovement.velocityState == CharacterMovement.VelocityState.Acceleration)
            {
                Debug.Log("It's all good hurray");
                Debug.Log(rect.ToString() + "   " + playerInputVector);

            }
        }*/
    }

    private void CheckOwnVelocityType()
    {

        if (playerInputVector.magnitude == 0)
        {
            MessageCorrectness("brake");
            return;
        }

        if (cMovement.GetBoolControlAcceleration(0))
        {
            if (playerInputVector.magnitude < percentualVelocityVector.magnitude - 0.001f)
            {
                MessageCorrectness("control");
                return;
            }
        }

        //float transformRotationAngle = 0.0f;
       // Vector3 transformRotationAxis = Vector3.zero;
       // filledArea.rotation.ToAngleAxis(out transformRotationAngle, out transformRotationAxis);


        //float asdf = filledArea.rotation.eulerAngles.z;
        float fdsa = filledArea.localEulerAngles.z;

        //Debug.Log(lookDirectionArrow.rotation.eulerAngles.z + "  " + asdf);
        // Looking forward = 0. Angle is in radians
       // float angle = -Mathf.Atan2(cameraForward.x, cameraForward.z);

        // Rotating a vector by an angle: (cos(a) * x - sin(a) * y, cos(a) * y + sin(a) * x)
        float x = -Mathf.Sin(fdsa * Mathf.Deg2Rad);
        float y = Mathf.Cos(fdsa * Mathf.Deg2Rad);

        Vector2 direction = new Vector2(x, y).normalized;

        direction.Normalize();

      //  Debug.Log("Move input: (" + playerInputVector.x + ", " + playerInputVector.y + ")");

      //  Vector2 backScaled = filledArea.localPosition.ToVector2() / scale * 2;

      //  Debug.Log("filledArea backscaled: (" + backScaled.x + ", " + backScaled.y + ")");

      //  Debug.Log("MV Forward direction: (" + direction.x + "  " + direction.y + ")  (" + direction.magnitude + ")");
      //  Debug.LogError(filledArea.localPosition.ToVector2() / scale * 2 + "    " + playerInputVector);

        float a = Vector2.Dot(playerInputVector, direction);
        float b = Vector2.Dot(filledArea.localPosition.ToVector2() / scale * 2, direction);


      //  Debug.LogError("Dot products: Input = " + a + ", Filled area pivot (scaled back) = " + b);


        if (a >= b - 0.001f)
        {
            MessageCorrectness("acceleration", a, b);// Debug.Log("Acceleration is used");
        }
        else
        {
            MessageCorrectness("control");// Debug.Log("Control is used");
        }

        //Debug.Log("End of script that independently calculates which is used");

    }

    private void MessageCorrectness(string pType, float pA = 0f, float pB = 0f)
    {
        if (!showDebugMessages) return;
        Debug.Log("Looking at " + pType);
        bool wrong = true;
        switch (cMovement.velocityState)
        {
            case CharacterMovement.VelocityState.Acceleration:
                if (pType.ToLower() == "acceleration") wrong = false;
                break;
            case CharacterMovement.VelocityState.Control:
                if (pType.ToLower() == "control") wrong = false;
                break;
            case CharacterMovement.VelocityState.Brake:
                if (pType.ToLower() == "brake") wrong = false;
                break;
        }

        if (wrong)
        {
            Debug.LogWarning("Something did not match up. Calculated type is " + pType + ", character says type is " +  cMovement.velocityState);
        }

        if (pType == "acceleration" && cMovement.velocityState == CharacterMovement.VelocityState.Control)
        {
            //isFrozen = true;
            Debug.LogWarning("Difference: " + (Mathf.Abs(pA) - Mathf.Abs(pB)) + " (" + pA + " " + pB + ")");
        }
    }

    private void CalculateFilledArea()
    {
        // requireBiggerTotalSpeed
        innerCircle.enabled = cMovement.GetBoolControlAcceleration(0);

        // isLookForward
        filledArea.rotation = cMovement.GetBoolControlAcceleration(1) ? lookDirectionArrow.rotation : velocityDirectionArrow.rotation; 

        // useVelocityTotal
        if (cMovement.GetBoolControlAcceleration(2))
        {
            if (cMovement.GetBoolControlAcceleration(1))
            {
                // Placing the pivot where the look direction and speed circle overlap
                float percent = cMovement.GetPercentualVelocity().magnitude;

                // WATCH OUT: This won't work properly if the character ever decides to look up. There's two instances in this script where this happens.
                Vector3 aaa = cMovement.transform.forward * percent * scale/2;

                aaa.y = aaa.z;
                aaa.z = 0;

                //aaa.SetXY(aaa.x, aaa.z);

                filledArea.position = aaa + transform.position;
            }
            else
            {
                // No need to calculate where the velocity vector overlaps on the speed circle: just take the percentual velocity
                filledArea.position = percentualVelocityImage.position;
            }
        }
        // useVelocityDirectional
        else if (cMovement.GetBoolControlAcceleration(3))
        {
            filledArea.position = percentualVelocityImage.position;
        }
        else
        {
            filledArea.position = transform.position;
        }
    }

    private void SetInput()
    {
        playerInputVector = cMovement.MoveInput;
        playerInputImage.position = playerInputVector * scale/2;
        playerInputImage.position += transform.position;
    }

    private void SetPercVelocity()
    {
        percentualVelocityVector = cMovement.GetPercentualVelocity();
        percentualVelocityImage.position = percentualVelocityVector * scale/2;
        percentualVelocityImage.position += transform.position;
    }

    private void SetSpeedCircle()
    {
        speedCircleImage.localScale = Vector3.one * percentualVelocityVector.magnitude;

        // For the filled area. The thing between masks is supposed to keep the same world size
        innerCircle.rectTransform.localScale = speedCircleImage.localScale;
        // Preventing division by 0
        if (percentualVelocityVector.magnitude <= 0.001f) return;
        betweenMasks.localScale = Vector3.one / percentualVelocityVector.magnitude;
    }

    private void SetVelocityArrow()
    {
        Vector2 forw = cMovement.GetPercentualVelocity();

        if (forw.magnitude <= 0.01f)
        {
            velocityDirectionArrow.gameObject.SetActive(false);
        }
        else
        {
            velocityDirectionArrow.gameObject.SetActive(true);
            forw.Normalize();
        }

        float angle = -Mathf.Atan2(forw.x, forw.y) * Mathf.Rad2Deg;

        velocityDirectionArrow.Rotate(Vector3.forward, angle - velocityAngle);

        velocityAngle = angle;
    }

    private void SetLookArrow()
    {
        // WATCH OUT: This won't work properly if the character ever decides to look up. There's two instances in this script where this happens.
        Vector3 forw = cMovement.transform.forward;

        float angle = -Mathf.Atan2(forw.x, forw.z) * Mathf.Rad2Deg;

        lookDirectionArrow.Rotate(Vector3.forward, angle - lookAngle);

        lookAngle = angle;
    }

    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void UndoCameraRotation()
    {
        Vector3 cameraForward = cMovement.GetCameraForward();

        // Looking forward = 0. Angle is in radians
        float angle = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;

        transform.Rotate(Vector3.forward, angle);
    }

    private void GetVelocityType()
    {
        switch (cMovement.velocityState)
        {
            case CharacterMovement.VelocityState.Acceleration:
                velocityTypeIndicator.color = Color.green;
                break;
            case CharacterMovement.VelocityState.Control:
                velocityTypeIndicator.color = Color.yellow;
                break;
            case CharacterMovement.VelocityState.Brake:
                velocityTypeIndicator.color = Color.red;
                break;
            default:
                Debug.Log("Oh no");
                break;
        }
    }

    [ButtonMethod]
    private string ToggleFreeze()
    {
        isFrozen = !isFrozen;

        return (isFrozen ? "Froze" : "Unfroze") + " Movement visualisation";
    }

    [ButtonMethod]
    private string ToggleInnerCircle()
    {
        hideInnerCircle = !hideInnerCircle;
        innerCircle.gameObject.SetActive(!hideInnerCircle);

        return (hideInnerCircle ? "Now hiding" : "Now showing") + " inner circle";
    }
}
