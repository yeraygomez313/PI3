using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LocalForceAvoidance : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    private const float maxVelocityPerFrame = 100f;
    private const float maxVelocity = 1000f;
    private const float damping = 0.8f;
    [field:SerializeField] public float Mass { get; private set; } = 1f;
    public Vector2 Origin { get; private set; }

    private Transform tf;
    private Vector2 velocity;
    private CircleCollider2D circleCollider;
    private Vector2 colliderOrigin;
    public float ColliderRadius { get; private set; }
    public Vector2Int ChunkCoordinates { get; private set; }

    [SerializeField] private Transform target;
    [SerializeField] private bool staticUnit = false;

    private void Awake()
    {
        tf = transform;
        circleCollider = GetComponent<CircleCollider2D>();
        colliderOrigin = new Vector2(circleCollider.offset.x * tf.lossyScale.x, circleCollider.offset.y * tf.lossyScale.y);
        ColliderRadius = circleCollider.radius * tf.lossyScale.x;

        if (!circleCollider.isTrigger)
        {
            Debug.LogWarning("CircleCollider2D is not set to trigger.");
        }
    }

    private void Start()
    {
        ChunkCoordinates = ChunkManager.Instance.RegisterUnit(this);
    }

    public void CalculateForce()
    {
        if (staticUnit)
        {
            Origin = (Vector2)tf.position + colliderOrigin;
            return;
        }

        Vector2 totalForce = Vector2.zero;

        Vector2 targetDirection = ((Vector2)target.position - Origin).normalized;
        totalForce += targetDirection * movementSpeed;

        totalForce += ChunkManager.Instance.GetRepulsionForce(this);

        // Apply damping to velocity
        velocity *= damping;

        // Add clamped repulsion force
        velocity += Vector2.ClampMagnitude(totalForce, maxVelocityPerFrame) * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxVelocity);

        tf.position += (Vector3)velocity * Time.fixedDeltaTime;
        Origin = (Vector2)tf.position + colliderOrigin;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetChunkCoordinates(Vector2Int chunk)
    {
        ChunkCoordinates = chunk;
    }

    private void OnDisable()
    {
        ChunkManager.Instance.UnregisterUnit(this, ChunkCoordinates);
    }
}
