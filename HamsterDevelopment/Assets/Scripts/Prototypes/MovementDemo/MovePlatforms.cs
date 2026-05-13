using UnityEngine;

public class MovePlatforms : MonoBehaviour
{
    private enum Direction
    {
        X, Z
    }
    [Tooltip("Which direction changes the jump distance when you move this transform?")]
    [SerializeField] private Direction forwardDirection;

    public void MoveForward(float pAmount)
    {
        switch (forwardDirection)
        {
            case Direction.X:
                transform.position += Vector3.right * pAmount;
                break;
            case Direction.Z:
                transform.position += Vector3.up * pAmount;
                break;
        }
    }

    public void MoveUp(float pAmount)
    {
        transform.position += Vector3.up * pAmount;
    }
}
