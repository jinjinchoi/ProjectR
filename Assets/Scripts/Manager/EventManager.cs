using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager
{
    public string CurrentBattleInfoId { get; private set; }

    private Dictionary<int, ScenarioEventInfo> scenarioEventByDay;
    private List<NormalEventInfo> normalEvents;
    private HashSet<string> triggeredEventSet = new();

    private int lastNormalEventDay = 1;
    private bool canTriggerNormalEvent = true;

    public void Init(List<ScenarioEventInfo> scenarioEvent, List<NormalEventInfo> normalEvent)
    {
        scenarioEventByDay = new Dictionary<int, ScenarioEventInfo>();
        foreach (ScenarioEventInfo eventInfo in scenarioEvent)
        {
            if (!scenarioEventByDay.ContainsKey(eventInfo.day))
            {
                scenarioEventByDay.Add(eventInfo.day, eventInfo);
            }
        }

        normalEvents = normalEvent;
    }
    public bool IsScenarioExist(int day)
    {
        return scenarioEventByDay.ContainsKey(day);
    }

    public ScenarioEventInfo GetScenarioEvents(int day)
    {
        if (IsScenarioExist(day))
        {
            return scenarioEventByDay[day];
        }

        return null;
    }

    public void ExecuteScenarioEvent(int day)
    {
        ScenarioEventInfo scenerioEvent = GetScenarioEvents(day);
        if (scenerioEvent == null)
        {
            DebugHelper.LogWarning("scenerio event not exist");
            return;
        }

        if (scenerioEvent.type == EScenarioEventType.Dialogue)
        {
            EventHub.RaiseDialogueRequested(scenerioEvent.dialogueId);
            return;
        }

        if (scenerioEvent.type == EScenarioEventType.Battle)
        {
            CurrentBattleInfoId = scenerioEvent.battleInfoId;
            string battleSceneName = GameManager.Instance.GetBattleSceneNameBy(CurrentBattleInfoId);
            GameManager.LoadScene(battleSceneName);
        }
    }
    public bool CanTriggerNormalEvent(int day)
    {
        if (!canTriggerNormalEvent) return false;

        int gap = day - lastNormalEventDay;

        // 하루당 확률 증가량
        float increasePerDay = 0.05f; // 하루당 +5%
        float chance = gap * increasePerDay;

        // 최대 확률 제한
        chance = Mathf.Clamp01(chance);

        return UnityEngine.Random.value < chance;
    }
    public void ExecuteNormalEvent(int day)
    {
        NormalEventInfo normalEvent = GetRandomNormalEvent();
        if (normalEvent == null)
        {
            DebugHelper.Log("Available event does not exist.");
            canTriggerNormalEvent = false;
            return;
        }

        EventHub.RaiseDialogueRequested(normalEvent.dialogueId);
        if (normalEvent.isOnce)
        {
            triggeredEventSet.Add(normalEvent.eventId);
        }
        lastNormalEventDay = day;
    }

    private NormalEventInfo GetRandomNormalEvent()
    {
        List<NormalEventInfo> availableEvents = normalEvents
        .Where(e => !triggeredEventSet.Contains(e.eventId))
        .ToList();

        if (availableEvents.Count == 0)
            return null;

        return availableEvents[UnityEngine.Random.Range(0, availableEvents.Count)];
    }

    
}
