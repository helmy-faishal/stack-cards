
using UnityEngine;

[System.Serializable]
public class MissionDetail
{
    [SerializeField] string description;
    [SerializeField] CardType missionCardType;
    [SerializeField][Min(1)] int collectedCard = 1;

    public string Description
    {
        get
        {
            if (string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
            {
                return $"Collect {missionCardType} cards";
            }
            else
            {
                return description ;
            }
        }
    }
    public CardType MissionCardType => missionCardType;
    public int CollectedCard => collectedCard;
}
