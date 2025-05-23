using UnityEngine;

public class Step : MonoBehaviour
{
    private float speed = 80f;
    RectTransform RectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        RectTransform.localPosition = Vector3.zero;  
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform.localPosition = new Vector2(RectTransform.localPosition.x - speed* Time.deltaTime, RectTransform.localPosition.y);
    }
}
