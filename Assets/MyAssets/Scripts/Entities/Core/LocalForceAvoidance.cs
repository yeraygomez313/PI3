using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CircleCollider2D))]
public class LocalForceAvoidance : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    private const float maxVelocityPerFrame = 50f;
    private const float damping = 0.7f;
    [field: SerializeField] public float Mass { get; private set; } = 1f;
    public Vector2 Origin { get; private set; }

    private Transform tf;
    private Vector2 velocity;
    private float velocityMultiplier = 1f;
    private Tween slowdownTween;
    private CircleCollider2D circleCollider;
    private Vector2 colliderOrigin;
    public float ColliderRadius { get; private set; }
    public Vector2Int ChunkCoordinates { get; private set; }

    [field: SerializeField] public bool IsMonster { get; private set; } = false;
    [SerializeField] private LocalForceAvoidance target;
    [SerializeField] private bool staticUnit = false;
    private Tween stunTween;
    private bool inFear = false;
    private Tween fearTween;

    private void Awake()
    {
        //if (GetComponent<MonsterAI>()) IsMonster = true;
        //else IsMonster = false;

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

        if (target != null)
        {
            Vector2 targetDirection = (target.Origin - Origin).normalized;
            if (inFear) targetDirection *= -1f;
            totalForce += targetDirection * movementSpeed * velocityMultiplier;
        }

        totalForce += ChunkManager.Instance.GetRepulsionForce(this);

        // Apply damping to velocity
        velocity *= damping;

        // Add clamped repulsion force
        velocity += Vector2.ClampMagnitude(totalForce, maxVelocityPerFrame) * Time.fixedDeltaTime;

        tf.position += (Vector3)velocity * Time.fixedDeltaTime;
        Origin = (Vector2)tf.position + colliderOrigin;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void SetTarget(LocalForceAvoidance newTarget)
    {
        target = newTarget;
    }

    public void SetStatic(bool isStatic)
    {
        staticUnit = isStatic;
    }

    public void SetChunkCoordinates(Vector2Int chunk)
    {
        ChunkCoordinates = chunk;
    }

    private void OnDisable()
    {
        ChunkManager.Instance.UnregisterUnit(this, ChunkCoordinates);
    }

    public bool IsOverlapingTarget()
    {
        if (target == null)
        {
            return false;
        }
        float distance = Vector2.Distance(Origin, target.Origin);
        return distance < ColliderRadius + target.ColliderRadius + 0.01f;
    }

    public bool IsTargetInRange(float range)
    {
        if (target == null)
        {
            return false;
        }
        float distance = Vector2.Distance(Origin, target.Origin);
        return distance < range;
    }

    public void ApplyPushForce(Vector3 pushOrigin, float pushStrength)
    {
        Vector2 direction = (transform.position - pushOrigin).normalized;
        Vector2 force = direction * pushStrength;
        velocity += force;
    }

    public void ApplySlow(float slowAmount, float slowDuration)
    {
        if (slowdownTween != null && slowdownTween.IsActive())
        {
            slowdownTween.Kill();
        }

        velocityMultiplier = 1f - (slowAmount / 100f);

        slowdownTween = DOVirtual.DelayedCall(slowDuration, () =>
        {
            if (stunTween == null || stunTween.IsComplete())
            {
                velocityMultiplier = 1f;
            }
        }, false);
    }

    public void ApplyStun(float stunDuration)
    {
        if (stunTween != null && stunTween.IsActive())
        {
            stunTween.Kill();
        }

        velocityMultiplier = 0f;

        stunTween = DOVirtual.DelayedCall(stunDuration, () =>
        {
            if (slowdownTween == null || slowdownTween.IsComplete())
            {
                velocityMultiplier = 1f;
            }
        }, false);
    }

    public void ApplyFear(float duration)
    {
        if (fearTween != null && fearTween.IsActive())
        {
            fearTween.Kill();
        }

        inFear = true;

        fearTween = DOVirtual.DelayedCall(duration, () =>
        {
            inFear = false;
        }, false);
    }

    private void OnDestroy()
    {
        ChunkManager.Instance.UnregisterUnit(this, ChunkCoordinates);
    }
}