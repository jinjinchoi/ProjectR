using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> DayChanged;
    public event Func<Task> SceneChangingAsync;

    private EventManager eventManager;
    private DialogueManager dialogueManager;
    private SaveManager saveManager;

    private int day = 0;
    private bool isLoading = false;

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
    public SaveManager SaveManager => saveManager;
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

        SceneManager.sceneLoaded += OnSceneLoaded;
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

        saveManager ??= new SaveManager();

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ResetManager()
    {
        saveManager?.Reset();
        eventManager?.Reset();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
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

    public async Task LoadSceneAsync(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        if (SceneChangingAsync != null)
        {
            foreach (Func<Task> handler in SceneChangingAsync.GetInvocationList().Cast<Func<Task>>())
            {
                try
                {
                    await handler();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        await SceneManager.LoadSceneAsync(sceneName);
    }

    public string GetBattleSceneNameBy(string battleId)
    {
        return BattleInfoSO.GetBattleInfo(battleId).sceneName;
    }
}
