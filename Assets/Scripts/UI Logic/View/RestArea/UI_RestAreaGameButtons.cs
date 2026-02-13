using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaGameButtons
{
    private VisualElement root;
    private readonly Button optionButton;
    private readonly Button detailButton;

    public UI_RestAreaGameButtons(VisualElement root)
    {
        this.root = root;

        optionButton = root.Q<Button>("OptionButton");
        optionButton.clicked += OnOptionButtonClicked;

        detailButton = root.Q<Button>("DetailButton");
        detailButton.clicked += OnDetailButtonClicked;
    }

    public void Dispose()
    {
        optionButton.clicked -= OnOptionButtonClicked;
        detailButton.clicked -= OnDetailButtonClicked;
    }

    void OnOptionButtonClicked()
    {
        EventHub.RaiseGameMenuOpen();
    }

    void OnDetailButtonClicked()
    {
        EventHub.RaiseDetailButtonClicked();
    }
}
