using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BombTrajectory : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    [Header("Trajectory Settings")]
    [SerializeField] private float lineWidth = 0.3f; // Độ dày nét vẽ (ngắn/mỏng lại)
    [SerializeField] private int resolution = 30;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private LayerMask collisionMask;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void ShowTrajectory(Vector3 startPoint, Vector3 initialVelocity)
    {
        lineRenderer.positionCount = resolution;
        Vector3 currentPosition = startPoint;
        Vector3 currentVelocity = initialVelocity;

        for (int i = 0; i < resolution; i++)
        {
            lineRenderer.SetPosition(i, currentPosition);

            Vector3 nextPosition = currentPosition + currentVelocity * timeStep + 0.5f * Physics.gravity * (timeStep * timeStep);
            
            currentVelocity += Physics.gravity * timeStep;

            if (Physics.Linecast(currentPosition, nextPosition, out RaycastHit hit, collisionMask))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;
                break;
            }

            currentPosition = nextPosition;
        }
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
