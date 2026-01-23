using UnityEngine;
using UnityEngine.UIElements;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private string progressbarName = "ProgressBar";
    private VisualElement progressbarMain;
    private UIControllerBase uiController;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        progressbarMain = root.Q<VisualElement>(progressbarName);

        uiController = GetComponentInParent<UIControllerBase>();
    }

    void Start()
    {
        uiController.OnVitalRatioChanged += UpdateProgressBar;
    }

    private void UpdateProgressBar(bool isHealthChanged, float progress)
    {
        if (isHealthChanged)
        {
            progressbarMain.style.width = Length.Percent(progress * 100f);
        }
    }

    private void OnDisable()
    {
        uiController.OnVitalRatioChanged -= UpdateProgressBar;
    }

}
