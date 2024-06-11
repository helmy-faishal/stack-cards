using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                AudioManager.instance?.PlaySFX("Click");
            });
        }
    }
}
