using System;
using System.Collections.Generic;
using UnityEngine;

public class EventState
{
    // 한번 발동된 노멀 이벤트를 저장하는 Map. 세이브 로드 적용해야함.
    public Dictionary<string, bool> triggeredEventMap = new();
}

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }
    public event Action EventFinished;

    public int lastDay { get; private set; }

    [SerializeField] private ScenarioEventSO scenarioEventSO;
    [SerializeField] private NormalEventSO normalEventSO;

    private EventState eventState;

    private Dictionary<int, ScenarioEventInfo> eventByDay;
    private List<NormalEventInfo> normalEvents;
    private bool isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        isInitialized = true;

        eventState = new EventState();

        eventByDay = new Dictionary<int, ScenarioEventInfo>();
        lastDay = scenarioEventSO.lastDay;
        foreach (ScenarioEventInfo eventInfo in scenarioEventSO.scenarioEvent)
        {
            if (!eventByDay.ContainsKey(eventInfo.day))
            {
                eventByDay.Add(eventInfo.day, eventInfo);
            }
        }

        normalEvents = normalEventSO.NormalEvent;

    }
    public bool IsScenarioExist(int day)
    {
        return eventByDay.ContainsKey(day);
    }

    public ScenarioEventInfo GetScenarioEvents(int day)
    {
        if (IsScenarioExist(day))
        {
            return eventByDay[day];
        }

        return null;
    }

    public void ExecuteScenarioEvent(int day)
    {
        ScenarioEventInfo scenerioEvent = GetScenarioEvents(day);
        if (scenerioEvent == null)
        {
            EventFinished?.Invoke();
            return;
        }

        // TODO: execute dialgue, battle. etc...

    }

    public void ExecuteNormalEvent()
    {
        NormalEventInfo normalEvent = GetRandomNormalEvent();
        if (normalEvent == null)
        {
            EventFinished?.Invoke();
            return;
        }

        // TODO: execute event...
    }


    private NormalEventInfo GetRandomNormalEvent()
    {
        NormalEventInfo evnetInfo = normalEvents[UnityEngine.Random.Range(0, normalEvents.Count)];
        if (eventState.triggeredEventMap.ContainsKey(evnetInfo.eventId))
        {
            return null;
        }

        return evnetInfo;
    }
}
