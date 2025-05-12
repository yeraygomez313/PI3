using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LocalForceAvoidance : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    private const float maxVelocityPerFrame = 100f;
    private const float maxVelocity = 1000f;
    private const float repulsionStrength = 150f;
    private const float damping = 0.8f;
    [field:SerializeField] public float Mass { get; private set; } = 1f;

    private Vector2 velocity;
    private CircleCollider2D circleCollider;
    private Vector2 colliderOrigin;
    private float colliderRadius;

    [SerializeField] private Transform target;
    [SerializeField] private bool staticUnit = false;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        colliderOrigin = new Vector2(circleCollider.offset.x * transform.lossyScale.x, circleCollider.offset.y * transform.lossyScale.y);
        colliderRadius = circleCollider.radius * transform.lossyScale.x;

        if (!circleCollider.isTrigger)
        {
            Debug.LogWarning("CircleCollider2D is not set to trigger.");
        }
    }

    public void Start()
    {
        ChunkManager.Instance.RegisterUnit(this);
    }

    private void FixedUpdate()
    {
        if (staticUnit)
        {
            return;
        }

        Vector2 totalForce = Vector2.zero;

        Vector2 targetDirection = ((Vector2)target.position - GetOrigin()).normalized;
        totalForce += targetDirection * movementSpeed;

        foreach (var other in FindNearbyUnits()) // Replace with your chunk-based spatial check
        {
            Vector2 direction = (GetOrigin() - other.GetOrigin());
            float distance = direction.magnitude;
            float desiredSpacing = GetRadius() + other.GetRadius();
            float overlap = Mathf.Max(0f, desiredSpacing - distance); // assume desired spacing = 1

            if (overlap > 0f)
            {
                float t = Mathf.Clamp01(overlap / desiredSpacing); // Normalize to [0,1]
                float ease = EaseOutCubic(t);

                direction.Normalize();
                Vector2 force = direction * ease * repulsionStrength * (other.Mass / Mass);
                totalForce += force;
            }
        }

        // Apply damping to velocity
        velocity *= damping;

        // Add clamped repulsion force
        velocity += Vector2.ClampMagnitude(totalForce, maxVelocityPerFrame) * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxVelocity);

        transform.position += (Vector3)velocity * Time.fixedDeltaTime;
    }

    public Vector2 GetOrigin()
    {
        return (Vector2)transform.position + colliderOrigin;
    }

    public float GetRadius()
    {
        return colliderRadius;
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private IEnumerable<LocalForceAvoidance> FindNearbyUnits()
    {
        foreach (var unit in ChunkManager.Instance.GetNearbyUnits(this))
        {
            if (unit != this) // Avoid self-collision
            {
                yield return unit;
            }
        }
        yield break;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }
}
