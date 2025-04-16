using UnityEngine;

public class GoalMarker : MonoBehaviour
{
    public Transform rover; // drag your rover GameObject into this in the Inspector
    public float goal_dx = -100f;  // meters forward (Unity units)
    public float goal_dy = 0f;     // meters sideways (left/right)

    void Start()
    {
        // Assumes rover forward is Z+, adjust if needed
        Vector3 goalOffset = new Vector3(goal_dy, 0, goal_dx);  // Note dx = Z, dy = X in Unity
        transform.position = rover.position + goalOffset;
    }
}