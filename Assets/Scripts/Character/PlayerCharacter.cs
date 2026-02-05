using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.SceneChangingAsync += SaveBeforeSceneChange;
        // TODO: Data Load
    }
    private void OnDisable()
    {
        GameManager.Instance.SceneChangingAsync -= SaveBeforeSceneChange;
    }

    private Task SaveBeforeSceneChange()
    {
        PrimaryAttributeData data = new()
        {
            strength = ASC.AttributeSet.GetAttributeValue(EAttributeType.strength),
            dexterity = ASC.AttributeSet.GetAttributeValue(EAttributeType.dexterity),
            intelligence = ASC.AttributeSet.GetAttributeValue(EAttributeType.intelligence),
            vitality = ASC.AttributeSet.GetAttributeValue(EAttributeType.vitality),
            currentHeath = ASC.AttributeSet.GetAttributeValue(EAttributeType.currentHealth),
        };

        GameManager.Instance.SaveManager.BackupPrimaryData(data);
        return Task.CompletedTask;
    }
}
