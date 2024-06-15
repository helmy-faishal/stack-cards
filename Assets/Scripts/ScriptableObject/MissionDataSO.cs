using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Mission Data", menuName ="Mission Data")]
public class MissionDataSO : ScriptableObject
{
    [SerializeField] MissionDetail[] missionDetails;

    public MissionDetail[] MissionDetails => missionDetails;
    public CardType[] MissionCardType
    {
        get
        {
            if (missionDetails == null || missionDetails.Length == 0)
            {
                return null;
            }

            CardType[] type = new CardType[missionDetails.Length];
            for (int i = 0; i < missionDetails.Length; i++)
            {
                type[i] = missionDetails[i].MissionCardType;
            }

            return type;
        }
    }
}
