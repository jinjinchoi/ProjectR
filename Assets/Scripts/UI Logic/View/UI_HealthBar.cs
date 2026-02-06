using UnityEngine;
using UnityEngine.UIElements;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private string progressbarName = "ProgressBar";
    private VisualElement progressbarMain;
    private UIController_Character uiController;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        progressbarMain = root.Q<VisualElement>(progressbarName);

        uiController = GetComponentInParent<UIController_Character>();
    }

    private void OnEnable()
    {
        uiController.OnVitalRatioChanged += UpdateProgressBar;
    }

    private void OnDisable()
    {
        uiController.OnVitalRatioChanged -= UpdateProgressBar;
    }

    void Start()
    {
        UpdateProgressBar(true, uiController.GetHealthRatio());
        DebugHelper.Log("Update Healthbar");
    }

    private void LateUpdate()
    {
        if (Quaternion.Angle(transform.rotation, Quaternion.identity) > 0.01f)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateProgressBar(bool isHealthChanged, float progress)
    {
        if (isHealthChanged)
        {
            progressbarMain.style.width = Length.Percent(progress * 100f);
        }
    }



}
