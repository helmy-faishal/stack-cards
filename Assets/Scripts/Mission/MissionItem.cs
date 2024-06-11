using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionItem : MonoBehaviour
{
    [SerializeField] TMP_Text descriptionText;
    string description = "Collect X Card";

    int total;
    int current;

    public void SetDescription(string desc)
    {
        description = desc;
        ShowMissionDescription();
    }

    public void InitTotalCollected(int value)
    {
        total = value;
        ShowMissionDescription();
    }

    public void UpdateCollected(int value)
    {
        current = value;
        ShowMissionDescription();
    }

    public void ShowMissionDescription()
    {
        string text = $"{description} {current}/{total}";

        if (current >= total)
        {
            text = $"{text} [DONE]";
            transform.SetAsLastSibling();
        }
        descriptionText.text = text;
    }
}
