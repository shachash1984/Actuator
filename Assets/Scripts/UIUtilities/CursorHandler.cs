#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType { Regular, Movement, Query, DoorLocked, DoorUnlocked, Speak}
public class CursorHandler : MonoBehaviour
{
    public Texture2D regCursor; // 73, 41
    public Texture2D moveCursor; // 128, 128
    public Texture2D queryCursor; //104, 98
    public Texture2D speakCursor; //128, 128
    public Texture2D doorLockedCursor; //128, 128
    public Texture2D doorUnlockedCursor; //128, 128
    [SerializeField] private LayerMask _layerMask;

    private const int GROUND_LAYER = 11;
    private const int INTER_LAYER = 18;
    private const int DOOR_LAYER = 13;


    private Vector2 regHotSpot = new Vector2(73, 41);
    private Vector2 moveHotSpot = new Vector2(128, 128);
    private Vector2 queryHotSpot = new Vector2(104, 98);
    private Vector2 speakHotSpot = new Vector2(128, 128);
    private Vector2 doorLockedHotSpot = new Vector2(128, 128);
    private Vector2 doorUnlockedHotSpot = new Vector2(128, 128);

    private CursorType _cursorType;
    private Interactable _currentInteractable;

    private Camera _camera;

    private void Start()
    {
        SetCursor(CursorType.Regular);
        _camera = Camera.main;
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Scene1Manager.CanPlayerInteract())
        {
            if (_cursorType != CursorType.Regular)
                SetCursor(CursorType.Regular);
        }
        else if (Physics.Raycast(ray, out hit, 1000f ,_layerMask))
        {
            //Debug.Log(hit.collider.gameObject.layer);
           
            switch (hit.collider.gameObject.layer)
            {
                case GROUND_LAYER:
                    if (Movement.S.IsAllowedToMove() && !Movement.S.IsMoving())
                    {
                        if (Movement.S.CanMoveToPosition(hit.point))
                        {
                            SetCursor(CursorType.Movement);
                            if (Input.GetMouseButtonDown(0))
                            {
                                Movement.S.HandleMouseClick(hit.point);
                            }
                        }
                    }
                    else
                    {
                        if (_cursorType != CursorType.Regular)
                            SetCursor(CursorType.Regular);
                    }
                    if (_currentInteractable)
                        _currentInteractable.OnCursorExit();
                    _currentInteractable = null;
                    break;
                case INTER_LAYER:
                    
                    if (!_currentInteractable)
                        _currentInteractable = hit.collider.gameObject.GetComponent<Interactable>();
                    if (_currentInteractable.tag == "Speakable")
                        SetCursor(CursorType.Speak);
                    else
                        SetCursor(CursorType.Query);
                    _currentInteractable.OnCursorOver();
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_cursorType == CursorType.Speak)
                            Movement.S.LookAt(_currentInteractable.transform.position);
                        _currentInteractable.OnCursorDown();
                    }
                    break;
                case DOOR_LAYER:
                    if (!_currentInteractable)
                        _currentInteractable = hit.collider.gameObject.GetComponent<DoorBase>();
                    DoorBase db = _currentInteractable as DoorBase;
                    if (db)
                    {
                        if ((db).Locked)
                            SetCursor(CursorType.DoorLocked);
                        else
                            SetCursor(CursorType.DoorUnlocked);
                        db.OnCursorOver();
                        if (Input.GetMouseButtonDown(0))
                        {
                            db.OnCursorDown();
                        }
                    }
                    break;
                default:
                    if (_cursorType != CursorType.Regular)
                        SetCursor(CursorType.Regular);
                    if (_currentInteractable)
                        _currentInteractable.OnCursorExit();
                    _currentInteractable = null;
                    break;
            }
        }
        else
        {
            if (_cursorType != CursorType.Regular)
                SetCursor(CursorType.Regular);
            if (_currentInteractable)
                _currentInteractable.OnCursorExit();
            _currentInteractable = null;
        }
           
    }

    private void SetCursor(CursorType ct)
    {
        switch (ct)
        {
            case CursorType.Regular:
                _cursorType = CursorType.Regular;
                Cursor.SetCursor(regCursor, regHotSpot, CursorMode.Auto);
                break;
            case CursorType.Movement:
                _cursorType = CursorType.Movement;
                Cursor.SetCursor(moveCursor, moveHotSpot, CursorMode.Auto);
                break;
            case CursorType.Query:
                _cursorType = CursorType.Query;
                Cursor.SetCursor(queryCursor, queryHotSpot, CursorMode.Auto);
                break;
            case CursorType.DoorLocked:
                _cursorType = CursorType.DoorLocked;
                Cursor.SetCursor(doorLockedCursor, doorLockedHotSpot, CursorMode.Auto);
                    break;
            case CursorType.DoorUnlocked:
                _cursorType = CursorType.DoorUnlocked;
                Cursor.SetCursor(doorUnlockedCursor, doorUnlockedHotSpot, CursorMode.Auto);
                break;
            case CursorType.Speak:
                _cursorType = CursorType.Speak;
                Cursor.SetCursor(speakCursor, speakHotSpot, CursorMode.Auto);
                break;
            default:
                break;
        }
    }
}
