using System.Collections.Generic;
using UnityEngine;

public class DeploymentPreview : MonoBehaviour
{
    [SerializeField] private Color deploymentAllowedColor = Color.green;
    [SerializeField] private Color deploymentForbiddenColor = Color.red;
    private List<Transform> troops = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform troop = transform.GetChild(i);
            troops.Add(troop);
        }

        // set sprites and scale
    }

    private void Update()
    {
        foreach (var troop in troops)
        {
            troop.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z);
        }
    }

    public List<Vector3> GetSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        foreach (var troop in troops)
        {
            spawnPoints.Add(troop.position);
        }
        return spawnPoints;
    }

    public void SetDeploymentAllowed(bool isAllowed)
    {
        Color color = isAllowed ? deploymentAllowedColor : deploymentForbiddenColor;
        foreach (var troop in troops)
        {
            MaterialPropertyBlock spritePB = new MaterialPropertyBlock();
            Renderer spriteRenderer = troop.GetComponent<SpriteRenderer>();
            spriteRenderer.GetPropertyBlock(spritePB);
            spritePB.SetColor("_OutlineColor", color);
            spriteRenderer.SetPropertyBlock(spritePB);
        }
    }
}
