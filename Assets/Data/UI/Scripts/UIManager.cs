using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<PanelInfo> panels;
    private List<PanelInfo> activePanels = new();

    private void Awake()
    {
        if (panels.Count > 0) OpenPanel(panels[0].Id);
    }

    public void OpenPanel(string openedPanelId)
    {
        PanelInfo openedPanel = panels.First(x => x.Id == openedPanelId);

        List<string> panelsToClose = new();
        foreach (var activePanel in activePanels)
        {
            if (activePanel.incompatiblePanels.Contains(openedPanelId) && activePanel.priority < openedPanel.priority)
            {
                panelsToClose.Clear();
                return;
            }

            if (openedPanel.incompatiblePanels.Contains(activePanel.Id))
            {
                if (activePanel.priority <= openedPanel.priority)
                {
                    panelsToClose.Add(activePanel.Id);
                }
                else
                {
                    panelsToClose.Clear();
                    return;
                }
            }
        }

        foreach (var panelToCloseId in panelsToClose)
        {
            ClosePanel(panelToCloseId);
        }

        openedPanel.canvasGroup.alpha = 1f;
        openedPanel.canvasGroup.interactable = true;
        openedPanel.canvasGroup.blocksRaycasts = true;
        activePanels.Add(openedPanel);

        if (openedPanel.pausesTime) Time.timeScale = 0f;
    }

    public void ClosePanel(string closedPanelId)
    {
        PanelInfo closedPanel = panels.First(x => x.Id == closedPanelId);

        closedPanel.canvasGroup.alpha = 0f;
        closedPanel.canvasGroup.interactable = false;
        closedPanel.canvasGroup.blocksRaycasts = false;
        activePanels.Remove(closedPanel);

        if (!activePanels.Any(x => x.pausesTime)) Time.timeScale = 1f;
    }
}

[Serializable]
public struct PanelInfo
{
    public string Id;
    public CanvasGroup canvasGroup;
    public bool pausesTime;
    public List<string> incompatiblePanels;
    public int priority;
}
