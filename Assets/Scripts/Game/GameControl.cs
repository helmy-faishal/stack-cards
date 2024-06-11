using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script yang mengatur kontrol dalam permainan
public class GameControl : MonoBehaviour
{
    [Tooltip("Waktu minimal yang diperlukan untuk membuat kondisi kartu dari ditekan menjadi dapat digeser")]
    [SerializeField] float pressThreshold = 0.2f;
    float pressTime = 0f;

    public Action<Card> OnCardPressed;
    public Action<Card> OnCardStartDrag;
    public Action<Card> OnCardEndDrag;

    Camera cam;

    bool isPressed;
    bool isDragging;
    Card target;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        ButtonPressed();
        ButtonReleased();
    }

    void ButtonPressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
            {
                if (hit.transform.TryGetComponent(out Card card))
                {
                    isPressed = true;
                    target = card;
                    AudioManager.instance?.PlaySFX("Click");
                    break;
                }
            }
        }
    }

    void ButtonReleased()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (target == null) return;

            if (isPressed)
            {
                isPressed = false;
                pressTime = 0f;
                OnCardPressed?.Invoke(target);
            }

            if (isDragging)
            {
                OnCardEndDrag?.Invoke(target);
                target.OnCardDestroyed -= (_) =>
                {
                    target = null;
                    isDragging = false;
                };

                isDragging = false;
                target.IsDragging = false;
            }

            target = null;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            if (isDragging || isPressed)
            {
                isPressed = false;
                isDragging = false;
            }
        }

        if (isPressed)
        {
            pressTime += Time.deltaTime;

            if (pressTime > pressThreshold)
            {
                isDragging = true;
                target.IsDragging = true;
                OnCardStartDrag?.Invoke(target);

                target.OnCardDestroyed += (_) =>
                {
                    target = null;
                    isDragging = false;
                };

                pressTime = 0f;
                isPressed = false;
            }
        }

        if (isDragging)
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            target.MainTransform.position = GameUtility.ClampPointInBounds(GameManager.PlayableBounds, pos);
        }
    }


}
