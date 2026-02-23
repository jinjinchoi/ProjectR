using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameStartMode
{
    None,
    NewGame,          // 완전 새 시작
    LoadGame,         // 세이브 데이터 로드
    SceneTransition   // 다른 씬에서 넘어옴
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> DayChanged;
    public event Func<Task> SceneChangingAsync;
    public event Action GameClear;


    private EventManager eventManager;
    private DialogueManager dialogueManager;
    private RuntimeGameState runtimeGameState;
    private SaveManager saveManager;

    private int day = 0;
    private int tomorrow = 1;
    private bool isLoading = false;
    private EGameStartMode startMode;

    [SerializeField] private string restAreaSceneName = "RestArea";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string saveFileName = "SaveFile";


    #region Scriptable Object
    [Header("Event System")]
    [SerializeField] private ScenarioEventSO scenarioEventSO;
    [SerializeField] private NormalEventSO normalEventSO;
    [SerializeField] private BattleInfoSO battleEventInfoSO;
    [Header("Dialogue System")]
    [SerializeField] private NormalDialogueSO normalDialogueSO;
    [SerializeField] private ChoiceDialogueSO choiceDialogueSO;
    [SerializeField] private RewardDialogueSO rewardDialogueSO;
    #endregion
    #region Getter
    public NormalDialogueSO NormalDialogueSO => normalDialogueSO;
    public ChoiceDialogueSO ChoiceDialogueSO => choiceDialogueSO;
    public RewardDialogueSO RewardDialogueSO => rewardDialogueSO;
    public BattleInfoSO BattleInfoSO => battleEventInfoSO;
    public EventManager EventManager => eventManager;
    public DialogueManager DialogueManager => dialogueManager;
    public RuntimeGameState RuntimeGameState => runtimeGameState;
    public SaveManager SaveManager => saveManager;
    public int CurrentDay => day;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        battleEventInfoSO.Init();
        CreateManagerClassess();

        DontDestroyOnLoad(gameObject);
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

        runtimeGameState ??= new RuntimeGameState();
        saveManager ??= new SaveManager(saveFileName);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        eventManager.EventFinished += OnEventFinished;
    }

    private void OnDisable()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        eventManager.EventFinished -= OnEventFinished;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
    }

    public void ResetManager()
    {
        day = 0;
        tomorrow = 1;

        runtimeGameState?.Reset();
        eventManager?.Reset();
    }

    public void SaveGame()
    {
        SaveData data = new()
        {
            Day = day,
            GrowthData = runtimeGameState.CurrentGrowthData,
            PrimaryAttributeData = runtimeGameState.PlayerData,
            UnlokcedAbilityIds = runtimeGameState.UnlokcedAbilityIds,
            TriggeredEvent = eventManager.TriggeredEventSet.ToList()
        };

        SaveManager.SaveDataToDisk(data);
    }

    public void NewGame()
    {
        day = 0;
        startMode = EGameStartMode.NewGame;
        TravelToRestArea();
    }

    public void LoadGame()
    {
        SaveData data = SaveManager.LoadDataFromDisk();
        if (data == null)
        {
            Debug.LogWarning("No save data found");
            return;
        }

        tomorrow = data.Day;
        startMode = EGameStartMode.LoadGame;

        runtimeGameState.UpdatePlayerData(data.PrimaryAttributeData, data.UnlokcedAbilityIds);
        runtimeGameState.LoadGrowthData(data.GrowthData);
        eventManager.RestoreTriggeredEvent(data.TriggeredEvent.ToHashSet());
        TravelToRestArea();
    }

    public void OnLoadingFinished()
    {
        switch (startMode)
        {
            case EGameStartMode.NewGame:
            case EGameStartMode.SceneTransition:
                ProcessDay();
                break;

            case EGameStartMode.LoadGame:
                day = tomorrow;
                DayChanged?.Invoke(day);
                tomorrow++;
                break;
        }

        startMode = EGameStartMode.None;
    }

    public void ProcessDay()
    {
        day = tomorrow;
        DayChanged?.Invoke(day);

        if (day == scenarioEventSO.lastDay)
        {
            GameClear?.Invoke();
        }
        else if (eventManager.IsScenarioExist(day))
        {
            eventManager.ExecuteScenarioEvent(day);
        }
        else if (eventManager.CanTriggerNormalEvent(day))
        {
            eventManager.ExecuteNormalEvent(day);
        }
        else
        {
            OnEventFinished();
        }
    }

    public void OnEventFinished()
    {
        tomorrow++;
    }

    public BattleEventInfo GetCurrentEventEnemyInfo()
    {
        BattleEventInfo battleInfo = battleEventInfoSO.GetBattleInfo(eventManager.CurrentBattleInfoId);

        if (battleInfo == null)
        {
            DebugHelper.LogWarning($"Battle info is missing for ID: [{eventManager.CurrentBattleInfoId}]");
        }

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

    public void TravelToRestAreaWithSave()
    {
        startMode = EGameStartMode.SceneTransition;
        _ = LoadSceneAsync(restAreaSceneName);
    }

    public void TravelToRestArea()
    {
        SceneManager.LoadSceneAsync(restAreaSceneName);

    }

    public void TravelToMainMenu()
    {
        ResetManager();
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }

    public string GetBattleSceneNameBy(string battleId)
    {
        return BattleInfoSO.GetBattleInfo(battleId).sceneName;
    }
}
