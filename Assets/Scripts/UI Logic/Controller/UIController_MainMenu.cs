using UnityEngine.SceneManagement;

public class UIController_MainMenu
{
    public bool IsSaveDataExisted()
    {
        return GameManager.Instance.SaveManager.HasSaveFile();
    }
}
