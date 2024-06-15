using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionCard : Card
{
    Dictionary<CardType, int> _missionTotalMap = new Dictionary<CardType, int>();

    protected override void SetStart()
    {
        base.SetStart();

        if (m_GameManager.Mission == null)
        {
            Debug.LogError("Mission Manager not found!");
            return;
        }

        _missionTotalMap = m_GameManager.Mission.MissionTotal;
        m_AcceptedCard = _missionTotalMap.Keys.ToArray();
        m_AcceptedCardMask = m_GameManager.Mission.MissionMask;
    }

    protected override bool CanTriggered => true;

    protected override void SetEndCardDragging(Card card)
    {
        base.SetEndCardDragging(card);

        ProcessMission();
    }

    void ProcessTriggerCard()
    {
        if (!HasTriggeredCard) return;

        List<Card> processedCard = new List<Card>();
        var groupTrigger = m_TriggeredCard.GroupBy(card => card.CardType);
        foreach (var group in groupTrigger)
        {
            StackGroup selectedGroup = null;
            foreach (Card card in group)
            {
                if (card == null) continue;
                if (card is not StackableCard stackableCard) continue;
                if (selectedGroup == null)
                {
                    selectedGroup = stackableCard.Group;
                    processedCard.Add(card);
                    continue;
                }

                stackableCard.MergeGroup(selectedGroup);
                selectedGroup = stackableCard.Group;
            }
        }
        m_TriggeredCard = processedCard;
    }

    void ProcessMission()
    {
        if (!HasTriggeredCard) return;
        ProcessTriggerCard();
        foreach (StackableCard card in m_TriggeredCard.ToArray().Cast<StackableCard>())
        {
            if (!_missionTotalMap.ContainsKey(card.CardType)) continue;
            if (card == null) continue;

            int needed = _missionTotalMap[card.CardType];
            int available = card.Group.TotalStack;

            int remain = 0;
            if (available < needed)
            {
                remain = needed - available;
                _missionTotalMap[card.CardType] = needed - available;
            }
            else
            {
                _missionTotalMap.Remove(card.CardType);
                m_AcceptedCardMask = GameUtility.RemoveFromMask(m_AcceptedCardMask, card.CardType);
            }

            m_GameManager.Mission.OnMissionUpdate?.Invoke(card.CardType, remain);
            card.Group.DecreaseCard(needed);
            AudioManager.Instance?.PlaySFX("MissionUpdate");

            if (_missionTotalMap.Count <= 0)
            {
                m_GameManager.Mission.OnMissionFinished?.Invoke();
            }
        }
        m_TriggeredCard.RemoveAll(card => card == null);
    }
}
