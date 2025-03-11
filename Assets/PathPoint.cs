using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public PathType pathType = PathType.Linear;
    public float acceleration = 1f;
    public float initialVelocity;
    public float finalVelocity;

    public Transform[] nextPoints;  // Multiple possible next points
    public bool shouldRemoveOnReach = false;  // Boolean flag for removal

    public void CalculateFinalVelocity(float distance)
    {
        finalVelocity = Mathf.Sqrt(initialVelocity * initialVelocity + 2 * acceleration * distance);
    }
}
