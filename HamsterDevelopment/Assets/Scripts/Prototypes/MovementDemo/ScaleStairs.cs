using UnityEngine;

public class ScaleStairs : MonoBehaviour
{

    [SerializeField] private Vector3 scale;

    private void Start()
    {
        scale = transform.localScale;
    }
    private void ReloadScale()
    {
        transform.localScale = scale;
    }

    public void IncreaseXScale(float pAmount)
    {
        scale.x += pAmount;
        ReloadScale();
    }

    public void IncreaseYScale(float pAmount)
    {
        scale.y += pAmount;
        ReloadScale();
    }

    public void IncreaseZScale(float pAmount)
    {
        scale.z += pAmount;
        ReloadScale();
    }

}
