using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMission : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject _missionPanel;
    [SerializeField] Button _missionButton;
    [SerializeField] Button _closeButton;

    [Header("Mission UI")]
    [SerializeField] Transform _scrollContent;
    [SerializeField] MissionItem _missionItemPrefab;

    [Header("Mission Data")]
    [SerializeField] MissionDataSO[] _missionDataSO;
    Dictionary<CardType, string> _missionDescriptionMap = new Dictionary<CardType, string>();
    Dictionary<CardType, int> _missionTotalMap = new Dictionary<CardType, int>();
    public Dictionary<CardType, int> MissionTotal => _missionTotalMap;
    int _missionMask;
    public int MissionMask => _missionMask;

    MissionDetail[] _selectedMission;
    public MissionDetail[] SelectedMission => _selectedMission;

    public Action<CardType, int> OnMissionUpdate;
    public Action OnMissionFinished;

    private void Awake()
    {
        _selectedMission = GetRandomMission();
        SetMissionMapping(_selectedMission);
    }

    public MissionDetail[] GetRandomMission()
    {
        if (_missionDataSO == null || _missionDataSO.Length == 0) return null;

        int index = Random.Range(0, _missionDataSO.Length);
        return _missionDataSO[index].MissionDetails;
    }

    public void SetMissionMapping(MissionDetail[] missions)
    {
        if (missions == null || missions.Length == 0)
        {
            return;
        }
        _missionMask = 0;
        for (int i = 0; i < missions.Length; i++)
        {
            MissionDetail mission = missions[i];
            _missionMask |= GameUtility.GetCardTypeMask(mission.MissionCardType);

            if (_missionTotalMap.ContainsKey(mission.MissionCardType))
            {
                _missionTotalMap[mission.MissionCardType] += mission.CollectedCard;
            }
            else
            {
                _missionTotalMap.Add(mission.MissionCardType, mission.CollectedCard);
                _missionDescriptionMap.Add(mission.MissionCardType, mission.Description);
            }
        }
    }

    private void Start()
    {
        SetUI();
        SetMissionUI();

        _missionButton.onClick?.Invoke();
    }

    void SetUI()
    {
        _missionButton.onClick.AddListener(OpenMissionPanel);
        _closeButton.onClick.AddListener(CloseMissionPanel);
    }

    void OpenMissionPanel()
    {
        _missionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void CloseMissionPanel()
    {
        _missionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void SetMissionUI()
    {
        foreach (CardType cardType in _missionDescriptionMap.Keys)
        {
            MissionItem item = Instantiate(_missionItemPrefab, _scrollContent);
            item.SetDescription(_missionDescriptionMap[cardType]);

            CardType type = cardType;
            int total = _missionTotalMap[cardType];
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
