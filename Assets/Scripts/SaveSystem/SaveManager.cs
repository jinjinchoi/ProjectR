using UnityEngine;

public class SaveManager
{
    private readonly FileDataHandler dataHandler;

    public SaveManager(string fileName)
    {
        dataHandler ??= new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void SaveDataToDisk(SaveData data)
    {
        dataHandler.SaveData(data);
    }

    public SaveData LoadDataFromDisk()
    {
       return dataHandler.LoadData();
    }

    public bool HasSaveFile()
    {
        return dataHandler.SaveFileExists();
    }
}
