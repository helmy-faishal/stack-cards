using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GameControl : MonoBehaviour
{
    PlayerInputAction _action;
    InputAction _pressInput;
    InputAction _moveInput;

    Card _target;
    public bool HasTarget => _target != null;

    public Action<Card> OnSingleTapPressed;
    public Action<Card> OnMultiTapPressed;
    public Action<Card> OnStartDrag;
    public Action<Card> OnEndDrag;

    private void OnEnable()
    {
        _action = new PlayerInputAction();
        _action.Player.Enable();

        _pressInput = _action.Player.Press;
        _moveInput = _action.Player.Move;
    }

    private void OnDisable()
    {
        _action.Player.Disable();
    }

    private void Start()
    {
        _pressInput.performed += InputPressPerformed;
        _pressInput.canceled += InputPressCanceled;
    }

    void InputPressPerformed(InputAction.CallbackContext context)
    {
        SetPressedTarget();

        if (!HasTarget) return;

        if (context.interaction is TapInteraction)
        {
            OnSingleTapPressed?.Invoke(_target);
            _target = null;
        }

        if (context.interaction is MultiTapInteraction)
        {
            OnMultiTapPressed?.Invoke(_target);
            _target = null;
        }

        if (context.interaction is HoldInteraction)
        {
            OnStartDrag?.Invoke(_target);
        }
    }

    void InputPressCanceled(InputAction.CallbackContext context)
    {
        if (!HasTarget) return;

        if (context.interaction is HoldInteraction)
        {
            OnEndDrag?.Invoke(_target);
        }

        _target = null;
    }

    void SetPressedTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(_moveInput.ReadValue<Vector2>());

        float renderValue = Mathf.NegativeInfinity;
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
        {
            if (!hit.transform.TryGetComponent(out Card card)) continue;
            int layerValue = SortingLayer.GetLayerValueFromName(card.Render.sortingLayerName);

            if (layerValue > renderValue)
            {
                renderValue = layerValue;
                _target = card;
            }
        }
    }

    private void LateUpdate()
    {
        MoveControl();
    }

    void MoveControl()
    {
        if (!HasTarget) return;

        Vector2 position = Camera.main.ScreenToWorldPoint(_moveInput.ReadValue<Vector2>());
        _target.ChangeCardPosition(position);
    }
}
