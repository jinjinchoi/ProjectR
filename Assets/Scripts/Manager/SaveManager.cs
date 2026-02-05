using UnityEngine;

public class PrimaryAttributeData
{
    public float strength;
    public float dexterity;
    public float intelligence;
    public float vitality;
    public float currentHeath;
}

public class SaveManager
{
    PrimaryAttributeData playerData;

    public void BackupPrimaryData(PrimaryAttributeData data)
    {
        playerData = data;
    }
}
