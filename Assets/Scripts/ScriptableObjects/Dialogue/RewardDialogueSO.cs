using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RewardNodeInfo
{
    public string nodeId;
    public EAttributeType attribute;
    public int reward;
    public string nextNodeId;
}

[CreateAssetMenu(fileName = "RewardDialogue", menuName = "Dialogue/Reward")]
public class RewardDialogueSO : ScriptableObject
{
    public List<RewardNodeInfo> rewardNodeInfo;
}
