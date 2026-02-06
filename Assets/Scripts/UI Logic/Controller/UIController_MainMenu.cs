using UnityEngine.SceneManagement;

public class UIController_MainMenu
{
    public void Init()
    {
        GameManager.Instance.ResetManager();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
