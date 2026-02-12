using UnityEngine;
using UnityEngine.UIElements;

public class UI_BattleOverlay : MonoBehaviour
{
    private UIController_HealthBar HealthBarUIController;
    private UI_TextHealthBar healthBar;
    private VisualElement root;

    private void Awake()
    {
        HealthBarUIController = new UIController_HealthBar();
        HealthBarUIController.Init(GetComponentInParent<IAbilitySystemContext>());

        root = GetComponent<UIDocument>().rootVisualElement;

        healthBar = new UI_TextHealthBar(HealthBarUIController, root);
    }

    private void OnDisable()
    {
        healthBar?.Dispose();
    }

}
