using System;
using System.Collections.Generic;
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

        // TODO: execute dialgue, battle. etc...

    }

    public void ExecuteNormalEvent()
    {
        NormalEventInfo normalEvent = GetRandomNormalEvent();
        if (normalEvent == null)
        {
            Debug.LogWarning("Event not exist");
            return;
        }

        // TODO: execute event...
    }


    private NormalEventInfo GetRandomNormalEvent()
    {
        NormalEventInfo evnetInfo = normalEvents[UnityEngine.Random.Range(0, normalEvents.Count)];
        if (eventState.triggeredEventSet.Contains(evnetInfo.eventId))
        {
            return null;
        }

        return evnetInfo;
    }
}
