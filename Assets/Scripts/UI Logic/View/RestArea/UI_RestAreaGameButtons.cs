using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaGameButtons
{
    private VisualElement root;
    private readonly Button optionButton;
    private readonly Button detailButton;
    private readonly Button skillButton;

    public UI_RestAreaGameButtons(VisualElement root)
    {
        this.root = root;

        optionButton = root.Q<Button>("OptionButton");
        optionButton.clicked += OnOptionButtonClicked;

        detailButton = root.Q<Button>("DetailButton");
        detailButton.clicked += OnDetailButtonClicked;

        skillButton = root.Q<Button>("SkillButton");
        skillButton.clicked += OnSkillButtonClicked;
    }

    public void Dispose()
    {
        optionButton.clicked -= OnOptionButtonClicked;
        detailButton.clicked -= OnDetailButtonClicked;
        skillButton.clicked -= OnSkillButtonClicked;
    }

    void OnOptionButtonClicked()
    {
        EventHub.RaiseGameMenuOpen();
    }

    void OnDetailButtonClicked()
    {
        EventHub.RaiseDetailButtonClicked();
    }

    void OnSkillButtonClicked()
    {
        EventHub.RaiseSkillButtonClicked();
    }
}
