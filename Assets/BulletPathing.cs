using System.Collections;
using UnityEngine;

public class BulletPathing : MonoBehaviour
{
    public Transform startingPoint;  // Set this in Unity
    private Transform currentTarget;
    private float speed = 0f;
    private PathPoint currentPointScript;

    public FocusPoint[] focusPoints;  // List of focus points
    private FocusPoint currentFocus;  // The active focus point
    private int currentFocusIndex = 0;  // The index of the current focus point

    public float rotationSpeed = 5f;  // Speed at which the bullet rotates towards the focus point

    void Start()
    {
        if (startingPoint == null)
        {
            Debug.LogError("No starting point assigned!");
            return;
        }

        currentTarget = startingPoint;
        currentPointScript = currentTarget.GetComponent<PathPoint>();

        if (currentPointScript == null)
        {
            Debug.LogError("Starting point is missing PathPoint script!");
            return;
        }

        // Initialize focus points
        SetActiveFocus();

        InitializePathVelocities();
        StartCoroutine(MoveAlongPath());
    }

    void SetActiveFocus()
    {
        if (focusPoints.Length > 0)
        {
            currentFocus = focusPoints[currentFocusIndex];  // Set the first focus as the active one
        }
    }

    void InitializePathVelocities()
    {
        if (currentPointScript == null) return;

        currentPointScript.initialVelocity = speed;
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        currentPointScript.CalculateFinalVelocity(distance);
    }

    IEnumerator MoveAlongPath()
    {
        while (currentTarget != null)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = currentTarget.position;

            speed = currentPointScript.initialVelocity;
            float finalVelocity = currentPointScript.finalVelocity;
            float acceleration = currentPointScript.acceleration;

            float travelTime = acceleration != 0 ? (finalVelocity - speed) / acceleration : Vector3.Distance(startPos, targetPos) / speed;

            float t = 0f;
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                speed += acceleration * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                t += Time.deltaTime;

                // Rotate towards the active focus during movement
                if (currentFocus != null)
                {
                    RotateTowardsFocus();
                }

                yield return null;
            }

            transform.position = targetPos;

            // If the point has the removal flag, remove it from the scene
            if (currentPointScript.shouldRemoveOnReach)
            {
                Destroy(currentTarget.gameObject);  // Remove the current target from the scene
            }

            // Choose next target dynamically
            currentTarget = ChooseNextTarget();
            if (currentTarget != null)
            {
                currentPointScript = currentTarget.GetComponent<PathPoint>();
                InitializePathVelocities();
            }
            else
            {
                Debug.Log("No more points to move towards.");
            }

            // Check if the current focus is destroyed and set the next one as active
            if (currentFocus == null || currentFocus.gameObject == null)
            {
                SwitchToNextFocus();
            }

            yield return null;
        }
    }

    Transform ChooseNextTarget()
    {
        if (currentPointScript == null || currentPointScript.nextPoints == null || currentPointScript.nextPoints.Length == 0)
        {
            Debug.Log("No next points available.");
            return null;
        }

        // Pick the closest point
        Transform bestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform next in currentPointScript.nextPoints)
        {
            float dist = Vector3.Distance(transform.position, next.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                bestTarget = next;
            }
        }

        Debug.Log("Next target: " + bestTarget.position);
        return bestTarget;
    }

void RotateTowardsFocus()
{
    if (currentFocus != null)
    {
        // Calculate the direction vector from the bullet to the focus point
        Vector3 direction = currentFocus.transform.position - transform.position;
        
        // Ignore the y-axis for rotation to keep it 2D
        direction.x = 0f;
        direction.y = 0f;

        // If the direction vector is not zero, rotate the bullet towards it
        if (direction.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}

    void SwitchToNextFocus()
    {
        currentFocusIndex++;
        if (currentFocusIndex < focusPoints.Length)
        {
            currentFocus = focusPoints[currentFocusIndex];
        }
        else
        {
            currentFocus = null;  // No more focuses
        }
    }
}
