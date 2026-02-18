using System;
using UnityEngine;

public class UIController_BattleOverlay : BaseCharacterUIController
{
    public event Action<BuffUIData> BuffStart;
    public event Action<EAbilityId> BuffEnd;

    public override void Init(IAbilitySystemContext asc)
    {
        base.Init(asc);

        abilitySystem.OnBuffActivated += OnBuffActivated;
        abilitySystem.OnBuffDeactivated += OnBuffDeactivated;
    }

    private void OnBuffActivated(BuffUIData data)
    {
        if (data.Icon == null) return;
        BuffStart?.Invoke(data);
    }

    private void OnBuffDeactivated(EAbilityId id)
    {
        BuffEnd?.Invoke(id);
    }

    public override void Dispose()
    {
        base.Dispose();

        abilitySystem.OnBuffActivated -= OnBuffActivated;
        abilitySystem.OnBuffDeactivated -= OnBuffDeactivated;
    }

    public int GetCurrentDay()
    {
        return GameManager.Instance.CurrentDay;
    }
}
