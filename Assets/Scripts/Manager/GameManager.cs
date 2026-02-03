using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> DayChanged;
    

    [Header("Event System")]
    [SerializeField] private ScenarioEventSO scenarioEventSO;
    [SerializeField] private NormalEventSO normalEventSO;
    [Header("Dialogue System")]
    [SerializeField] private NormalDialogueSO normalDialogueSO;
    [SerializeField] private ChoiceDialogueSO choiceDialogueSO;
    [SerializeField] private RewardDialogueSO rewardDialogueSO;

    public EventManager eventManager;
    private DialogueManager dialogueManager;

    private int day = 0;
    private int lastActivatedEventDay = 1;

    public NormalDialogueSO NormalDialogueSO => normalDialogueSO;
    public ChoiceDialogueSO ChoiceDialogueSO => choiceDialogueSO;
    public RewardDialogueSO RewardDialogueSO => rewardDialogueSO;
    public EventManager EventManager => eventManager;
    public DialogueManager DialogueManager => dialogueManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateManagerClassess();
    }

    private void CreateManagerClassess()
    {
        if (eventManager == null)
        {
            eventManager = new EventManager();
            eventManager.Init(scenarioEventSO.scenarioEvent, normalEventSO.NormalEvent);
        }

        if (dialogueManager == null)
        {
            dialogueManager = new DialogueManager();
            dialogueManager.Init();
        }
    }

    public void ProcessDay()
    {
        day++;
        DayChanged?.Invoke(day);

        if (day == scenarioEventSO.lastDay)
        {
            // game end

        }
        else if (eventManager.IsScenarioExist(day))
        {
            eventManager.ExecuteScenarioEvent(day);
        }
        else if (CanTriggerNormalEvent(eventManager))
        {
            Debug.Log("NormalEvent Execute");
            eventManager.ExecuteNormalEvent();
            lastActivatedEventDay = day;
        }
    }

    private bool CanTriggerNormalEvent(EventManager eventManager)
    {
        int gap = day - lastActivatedEventDay;

        // 하루당 확률 증가량
        float increasePerDay = 0.2f; // 하루당 +10%
        float chance = gap * increasePerDay;

        // 최대 확률 제한
        chance = Mathf.Clamp01(chance);

        return UnityEngine.Random.value < chance;
    }

}
