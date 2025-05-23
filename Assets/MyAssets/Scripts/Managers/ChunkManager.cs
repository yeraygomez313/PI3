using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    private Dictionary<Vector2Int, List<LocalForceAvoidance>> chunks = new();

    [SerializeField] private float chunkSize = 2f;
    private List<LocalForceAvoidance> units = new();
    private const float repulsionStrength = 100f;

    private static readonly Vector2Int[] NeighborOffsets = new Vector2Int[]
{
    new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
    new Vector2Int(-1,  0), new Vector2Int(0,  0), new Vector2Int(1,  0),
    new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1)
};

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        ClearChunks();
        foreach (var unit in units)
        {
            UpdateChunkPosition(unit);
        }

        foreach (var unit in units)
        {
            unit.CalculateForce();
        }

        //foreach (var unit in units)
        //{
        //    unit.SetTarget(GetClosestTarget(unit));
        //}
    }

    public Vector2Int WorldToChunkPos(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / chunkSize),
                              Mathf.FloorToInt(position.y / chunkSize));
    }

    public Vector2Int RegisterUnit(LocalForceAvoidance unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }

        return UpdateChunkPosition(unit);
    }

    private Vector2Int UpdateChunkPosition(LocalForceAvoidance unit)
    {
        Vector2Int chunk = WorldToChunkPos(unit.Origin);

        if (!chunks.ContainsKey(chunk))
        {
            chunks[chunk] = new List<LocalForceAvoidance>();
        }

        chunks[chunk].Add(unit);
        unit.SetChunkCoordinates(chunk);
        return chunk;
    }

    public void UnregisterUnit(LocalForceAvoidance unit, Vector2Int chunk)
    {
        if (chunks.ContainsKey(chunk))
        {
            chunks[chunk].Remove(unit);
        }

        if (units.Contains(unit))
        {
            units.Remove(unit);
        }
    }

    public void ClearChunks()
    {
        chunks.Clear();
    }

    public Vector2 GetRepulsionForce(LocalForceAvoidance unit)
    {
        Vector2 avoidanceForce = Vector2.zero;
        Vector2 origin = unit.Origin;
        float radius = unit.ColliderRadius;
        Vector2Int originChunk = unit.ChunkCoordinates;
        Vector2Int chunk = new Vector2Int();

        foreach (var offset in NeighborOffsets)
        {
            chunk = originChunk + new Vector2Int(offset.x, offset.y);

            if (!chunks.ContainsKey(chunk)) continue;

            foreach (LocalForceAvoidance other in chunks[chunk])
            {
                if (other == unit) continue;

                Vector2 direction = origin - other.Origin;
                float sqrDistance = direction.sqrMagnitude;
                float desiredSpacing = radius + other.ColliderRadius;
                float sqrSpacing = desiredSpacing * desiredSpacing;

                if (sqrDistance < sqrSpacing)
                {
                    float overlap = desiredSpacing - Mathf.Sqrt(sqrDistance); // only do sqrt if really overlapping
                    if (overlap < 0.01f) continue;
                    float ease = EaseOutCubic(overlap / desiredSpacing);

                    direction.Normalize();
                    Vector2 force = direction * ease * repulsionStrength * (other.Mass / unit.Mass);
                    avoidanceForce += force;
                }
            }
        }

        return avoidanceForce;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }

    public LocalForceAvoidance GetClosestTarget(LocalForceAvoidance unit)
    {
        return GetClosestTargetInRange(unit, float.MaxValue);
    }

    public LocalForceAvoidance GetClosestTargetInRange(LocalForceAvoidance unit, float range)
    {
        LocalForceAvoidance bestTarget = null;
        float bestDistance = range;
        float currentChunkDistance = 0f;

        Vector2 origin = unit.Origin;
        float radius = unit.ColliderRadius;
        Vector2Int originChunk = unit.ChunkCoordinates;
        Vector2Int chunk = new Vector2Int();

        var sortedChunkInfo = chunks.Keys
            .Select(coord => new
            {
                Coord = coord,
                Distance = GetNearestCornerDistance(coord, originChunk)
            })
            .OrderBy(x => x.Distance)
            .ToList();

        for (int i = 0; i < sortedChunkInfo.Count; i++)
        {
            var sortedChunk = sortedChunkInfo[i];
            if (bestTarget != null && bestDistance < sortedChunk.Distance) return bestTarget;
            currentChunkDistance = sortedChunk.Distance;

            chunk = sortedChunk.Coord;
            if (!chunks.ContainsKey(chunk)) continue;
            foreach (LocalForceAvoidance other in chunks[chunk])
            {
                if (unit.IsMonster == other.IsMonster) continue;

                Vector2 direction = origin - other.Origin;
                float distance = direction.magnitude;

                if (distance < bestDistance)
                {
                    bestTarget = other;
                    bestDistance = distance;
                }
            }
        }

        return bestTarget;
    }

    public float GetNearestCornerDistance(Vector2Int a, Vector2Int b)
    {
        // Convert chunk coordinates to world-space bounds
        float minX1 = a.x * chunkSize;
        float maxX1 = minX1 + chunkSize;
        float minY1 = a.y * chunkSize;
        float maxY1 = minY1 + chunkSize;

        float minX2 = b.x * chunkSize;
        float maxX2 = minX2 + chunkSize;
        float minY2 = b.y * chunkSize;
        float maxY2 = minY2 + chunkSize;

        // Compute clamped distance on X axis
        float dx = Mathf.Max(0f, Mathf.Max(minX1 - maxX2, minX2 - maxX1));

        // Compute clamped distance on Y axis
        float dy = Mathf.Max(0f, Mathf.Max(minY1 - maxY2, minY2 - maxY1));

        // Pythagorean distance between nearest corners
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
