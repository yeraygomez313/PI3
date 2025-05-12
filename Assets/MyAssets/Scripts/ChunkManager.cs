using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    private Dictionary<Vector2Int, List<LocalForceAvoidance>> chunks = new();
    [SerializeField] private float chunkSize = 2f;
    private List<LocalForceAvoidance> units = new();

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
            RegisterUnit(unit);
        }
    }

    public Vector2Int WorldToChunkPos(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / chunkSize),
                              Mathf.FloorToInt(position.y / chunkSize));
    }

    public void RegisterUnit(LocalForceAvoidance unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }

        Vector2Int chunk = WorldToChunkPos(unit.GetOrigin());

        if (!chunks.ContainsKey(chunk))
        {
            chunks[chunk] = new List<LocalForceAvoidance>();
        }

        chunks[chunk].Add(unit);
    }

    public void ClearChunks()
    {
        chunks.Clear();
    }

    public List<LocalForceAvoidance> GetNearbyUnits(LocalForceAvoidance unit)
    {
        List<LocalForceAvoidance> nearby = new();
        Vector2Int centerChunk = WorldToChunkPos(unit.GetOrigin());

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int chunk = centerChunk + new Vector2Int(x, y);
                if (chunks.TryGetValue(chunk, out var list))
                {
                    nearby.AddRange(list);
                }
            }
        }

        return nearby;
    }
}
