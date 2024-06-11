using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMission : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject missionPanel;
    [SerializeField] Button missionButton;
    [SerializeField] Button closeButton;

    [Header("Mission UI")]
    [SerializeField] Transform scrollContent;
    [SerializeField] MissionItem missionItemPrefab;

    [Header("Mission Data")]
    [SerializeField] MissionDataSO[] missionDataSO;
    Dictionary<CardType, string> missionDescriptionMap = new Dictionary<CardType, string>();
    Dictionary<CardType, int> missionTotalMap = new Dictionary<CardType, int>();
    public Dictionary<CardType, int> MissionTotal => missionTotalMap;
    int missionMask;
    public int MissionMask => missionMask;

    MissionDetail[] selectedMission;
    public MissionDetail[] SelectedMission => selectedMission;

    public Action<CardType, int> OnMissionUpdate;
    public Action OnMissionFinished;

    private void Awake()
    {
        selectedMission = GetRandomMission();
        SetMissionMapping(selectedMission);
    }

    public MissionDetail[] GetRandomMission()
    {
        if (missionDataSO == null || missionDataSO.Length == 0) return null;

        int index = Random.Range(0, missionDataSO.Length);
        return missionDataSO[index].MissionDetails;
    }

    public void SetMissionMapping(MissionDetail[] missions)
    {
        if (missions == null || missions.Length == 0)
        {
            return;
        }
        missionMask = 0;
        for (int i = 0; i < missions.Length; i++)
        {
            MissionDetail mission = missions[i];
            missionMask |= GameUtility.GetCardTypeMask(mission.MissionCardType);

            if (missionTotalMap.ContainsKey(mission.MissionCardType))
            {
                missionTotalMap[mission.MissionCardType] += mission.CollectedCard;
            }
            else
            {
                missionTotalMap.Add(mission.MissionCardType, mission.CollectedCard);
                missionDescriptionMap.Add(mission.MissionCardType, mission.Description);
            }
        }
    }

    private void Start()
    {
        SetUI();
        SetMissionUI();

        missionButton.onClick?.Invoke();
    }

    void SetUI()
    {
        missionButton.onClick.AddListener(OpenMissionPanel);
        closeButton.onClick.AddListener(CloseMissionPanel);
    }

    void OpenMissionPanel()
    {
        missionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void CloseMissionPanel()
    {
        missionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void SetMissionUI()
    {
        foreach (CardType cardType in missionDescriptionMap.Keys)
        {
            MissionItem item = Instantiate(missionItemPrefab, scrollContent);
            item.SetDescription(missionDescriptionMap[cardType]);

            CardType type = cardType;
            int total = missionTotalMap[cardType];
            item.InitTotalCollected(total);

            OnMissionUpdate += (CardType updateType, int remainValue) =>
            {
                if (type == updateType)
                {
                    item.UpdateCollected(total - remainValue);
                }
            };
        }
    }
}
