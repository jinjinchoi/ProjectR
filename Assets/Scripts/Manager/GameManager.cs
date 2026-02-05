using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> DayChanged;

    private EventManager eventManager;
    private DialogueManager dialogueManager;

    private int day = 0;

    #region Scriptable Object
    [Header("Event System")]
    [SerializeField] private ScenarioEventSO scenarioEventSO;
    [SerializeField] private NormalEventSO normalEventSO;
    [SerializeField] private BattleInfoSO battleEventInfoSO;
    [SerializeField] private EnemyAttribtueSO enemyAttribtueSO;
    [Header("Dialogue System")]
    [SerializeField] private NormalDialogueSO normalDialogueSO;
    [SerializeField] private ChoiceDialogueSO choiceDialogueSO;
    [SerializeField] private RewardDialogueSO rewardDialogueSO;
    #endregion
    #region Getter
    public NormalDialogueSO NormalDialogueSO => normalDialogueSO;
    public ChoiceDialogueSO ChoiceDialogueSO => choiceDialogueSO;
    public RewardDialogueSO RewardDialogueSO => rewardDialogueSO;
    public EnemyAttribtueSO EnemyAttribtueSO => enemyAttribtueSO;
    public BattleInfoSO BattleInfoSO => battleEventInfoSO;
    public EventManager EventManager => eventManager;
    public DialogueManager DialogueManager => dialogueManager;
    #endregion

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
        enemyAttribtueSO.Init();
        battleEventInfoSO.Init();
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
        else if (eventManager.CanTriggerNormalEvent(day))
        {
            eventManager.ExecuteNormalEvent(day);
        }
    }

    public BattleEventInfo GetCurrentEventEnemyInfo()
    {
        BattleEventInfo battleInfo = battleEventInfoSO.GetBattleInfo(eventManager.CurrentBattleInfoId);

        if (battleInfo == null)
        {
            DebugHelper.LogWarning($"Battle Info not exist. id: {eventManager.CurrentBattleInfoId}");
        } // null check

        return battleInfo;
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public string GetBattleSceneNameBy(string battleId)
    {
        return BattleInfoSO.GetBattleInfo(battleId).sceneName;
    }
}
