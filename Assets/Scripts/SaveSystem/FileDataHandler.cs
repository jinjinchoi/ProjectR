using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string fullPath;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
    }

    public void SaveData(SaveData gameData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToSave = JsonUtility.ToJson(gameData, true);

            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter write = new(stream);
            write.Write(dataToSave);

            DebugHelper.Log("Save success");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public SaveData LoadData()
    {
        SaveData LoadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }

                LoadData = JsonUtility.FromJson<SaveData>(dataToLoad);
                DebugHelper.Log("Load success");

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        return LoadData;
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public bool SaveFileExists()
    {
        return File.Exists(fullPath);
    }
}
