using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    [SerializeField] private ScenarioEventSO scenarioEventSO;
    [SerializeField] private NormalEventSO normalEventSO;

    private Dictionary<int, ScenarioEventInfo> eventByDay;
    private List<NormalEventInfo> normalEvents;
    public int lastDay { get; private set; }
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

        eventByDay = new Dictionary<int, ScenarioEventInfo>();

        foreach (ScenarioEventInfo eventInfo in scenarioEventSO.scenarioEvent)
        {
            if (!eventByDay.ContainsKey(eventInfo.day))
            {
                eventByDay.Add(eventInfo.day, eventInfo);
            }
        }

        lastDay = scenarioEventSO.lastDay;

        normalEvents = new List<NormalEventInfo>();

        foreach (var eventInfo in normalEventSO.NormalEvent)
        {
            normalEvents.Add(eventInfo);
        }

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


}
