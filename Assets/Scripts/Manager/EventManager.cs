using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventState
{
    // 한번 발동된 노멀 이벤트를 저장하는 Set. 세이브 로드 적용해야함.
    public HashSet<string> triggeredEventSet = new();
}

public class EventManager
{
    private EventState eventState;

    private Dictionary<int, ScenarioEventInfo> scenarioEventByDay;
    private List<NormalEventInfo> normalEvents;

    public string CurrentBattleInfoId { get; private set; }

    public void Init(List<ScenarioEventInfo> scenarioEvent, List<NormalEventInfo> normalEvent)
    {
        eventState = new EventState();

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
            Debug.LogWarning("Event not exist");
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
        }
    }

    public void ExecuteNormalEvent()
    {
        NormalEventInfo normalEvent = GetRandomNormalEvent();
        if (normalEvent == null)
        {
            Debug.Log("Available event does not exist.");
            return;
        }

        EventHub.RaiseDialogueRequested(normalEvent.dialogueId);
        if (normalEvent.isOnce)
        {
            eventState.triggeredEventSet.Add(normalEvent.eventId);
        }
        
    }

    private NormalEventInfo GetRandomNormalEvent()
    {
        List<NormalEventInfo> availableEvents = normalEvents
        .Where(e => !eventState.triggeredEventSet.Contains(e.eventId))
        .ToList();

        if (availableEvents.Count == 0)
            return null;

        return availableEvents[UnityEngine.Random.Range(0, availableEvents.Count)];
    }

    
}
