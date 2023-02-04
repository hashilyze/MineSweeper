using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MSInputControler : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private MSCore _core;
    private Vector2 _lastDargPos;

    public void OnOpen (InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId))
        {
            return;
        }

        switch (ctx.phase)
        {
        case InputActionPhase.Started:
            if (CursorToCellPos(out Vector2Int offset))
            {
                _core.OpenTile(offset.x, offset.y);
            }
            break;
        }
    }

    public void OnMark (InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseRightId))
        {
            return;
        }   

        switch (ctx.phase)
        {
        case InputActionPhase.Started:
            if (CursorToCellPos(out Vector2Int offset))
            {
                _core.ToggleFlagTile(offset.x, offset.y);
            }
            break;
        }
    }

    public void OnMove (InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseRightId))
        {
            return;
        }
        Debug.Log($"({ctx.ReadValue<Vector2>().x}, {ctx.ReadValue<Vector2>().y})");

        Vector2 pos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        switch (ctx.phase)
        {
        case InputActionPhase.Started:
            _lastDargPos = pos;
            break;
        case InputActionPhase.Performed:
            _camera.transform.position += -new Vector3((pos - _lastDargPos).x * 0.5f, (pos - _lastDargPos).y * 0.5f, 0.0f);
            _lastDargPos = pos;
            break;
        }
    }

    private bool CursorToCellPos (out Vector2Int offset)
    {
        Vector2 pos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        offset = new Vector2Int(Mathf.FloorToInt(pos.x + _core.Board.Size.x * 0.5f), Mathf.FloorToInt(pos.y + _core.Board.Size.y * 0.5f));
        Debug.Log($"{pos}: {offset}");

        return !_core.Board.IsOutOfRange(offset.x, offset.y);
    }
}
