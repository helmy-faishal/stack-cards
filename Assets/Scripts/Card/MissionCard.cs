using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCard : Card
{
    Dictionary<CardType, int> missionTotalMap = new Dictionary<CardType, int>();

    StackGroup targetGroup;
    bool HasTarget => targetGroup != null && selectedCard != null;

    protected override void SetStart()
    {
        base.SetStart();

        if (gameManager.mission == null )
        {
            Debug.LogError("Mission Manager not found!");
            return;
        }

        missionTotalMap = gameManager.mission.MissionTotal;
        acceptedCardMask = gameManager.mission.MissionMask;
    }

    protected override void HandleTriggerEnter(Collider2D collision)
    {
        base.HandleTriggerEnter(collision);
        SetTargetGroup();
    }

    protected override void HandleTriggerStay(Collider2D collision)
    {
        base.HandleTriggerStay(collision);
        SetTargetGroup();
    }

    protected override void HandleTriggerExit(Collider2D collision)
    {
        base.HandleTriggerExit(collision);
        if (selectedCard == null)
        {
            targetGroup = null;
        }
    }

    void SetTargetGroup()
    {
        if (selectedCard is not StackableCard) return;

        StackableCard card = (StackableCard)selectedCard;

        if (targetGroup == null)
        {
            targetGroup = card.group;
        }
    }

    private void Update()
    {
        if (!HasTarget) return;
        if (selectedCard.IsDragging) return;
        ProcessMission();
    }

    void ProcessMission()
    {
        int needed = missionTotalMap[selectedCard.Type];
        int available = targetGroup.TotalStack;

        int remain = 0;
        if (available < needed)
        {
            remain = needed - available;
            missionTotalMap[selectedCard.Type] = needed - available;
        }
        else
        {
            missionTotalMap.Remove(selectedCard.Type);
            acceptedCardMask = GameUtility.RemoveFromMask(acceptedCardMask, selectedCard.Type);
        }

        gameManager.mission.OnMissionUpdate?.Invoke(selectedCard.Type, remain);
        targetGroup.DecreaseCard(needed);
        AudioManager.instance?.PlaySFX("MissionUpdate");

        if (missionTotalMap.Count <= 0)
        {
            gameManager.mission.OnMissionFinished?.Invoke();
        }

        selectedCard = null;
        targetGroup = null;

    }
}
