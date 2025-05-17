using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentPreview : MonoBehaviour
{
    [SerializeField] private Color deploymentAllowedColor = Color.green;
    [SerializeField] private Color deploymentForbiddenColor = Color.red;
    [SerializeField] private Color deploymentCanceledColor = Color.blue;
    private List<RectTransform> troops = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform troop = transform.GetChild(i).GetComponent<RectTransform>();
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

    public void SetDeploymentState(DeploymentState deploymentState)
    {
        Color color;
        switch (deploymentState)
        {
            case DeploymentState.Allowed:
                color = deploymentAllowedColor;
                break;
            case DeploymentState.Forbidden:
                color = deploymentForbiddenColor;
                break;
            case DeploymentState.Canceled:
                color = deploymentCanceledColor;
                break;
            default:
                color = Color.white;
                break;
        }

        foreach (var troop in troops)
        {
            Image image = troop.GetComponent<Image>();
            if (image == null) continue;

            if (image.material != null)
            {
                image.material = Instantiate(image.material);
                image.material.SetColor("_OutlineColor", color);
            }
        }
    }
}

public enum DeploymentState
{
    Allowed,
    Forbidden,
    Canceled
}
