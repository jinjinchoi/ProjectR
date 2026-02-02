using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RestAreaManager : MonoBehaviour
{
    public static RestAreaManager Instance { get; private set; }

    private int day = 1;
    private int lastEventDay = 1;

    private VisualElement overlayPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // TODO: Load porcess

    }

    public void SetOverlayPannel(VisualElement panel)
    {
        overlayPanel = panel;
    }

    public void ProcessDay()
    {
        SetOverlayVisible(false);

        GameEventManager eventManager = GameEventManager.Instance;

        if (day == eventManager.lastDay)
        {
            // game end

        }
        else if (eventManager.IsScenarioExist(day))
        {
            eventManager.ExecuteScenarioEvent(day);

        }
        else if (CanTriggerNormalEvent(eventManager))
        {
            eventManager.ExecuteNormalEvent();
        }

        day++;
        SetOverlayVisible(true);

    }

    private bool CanTriggerNormalEvent(GameEventManager eventManager)
    {
        int gap = day - lastEventDay;

        // 하루당 확률 증가량
        float increasePerDay = 0.05f; // 하루당 +5%
        float chance = gap * increasePerDay;

        // 최대 확률 제한
        chance = Mathf.Clamp01(chance);

        return UnityEngine.Random.value < chance;
    }

    private void SetOverlayVisible(bool isVisible)
    {
        if (overlayPanel == null) return;

        if (isVisible)
        {
            overlayPanel.style.display = DisplayStyle.Flex;
            overlayPanel.pickingMode = PickingMode.Position;
        }
        else
        {
            overlayPanel.style.display = DisplayStyle.None;
            overlayPanel.pickingMode = PickingMode.Ignore;
        }
    }

}
