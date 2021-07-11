using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandle : MonoBehaviour
{
    [SerializeField] private RectTransform selectionArea = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;
    private Vector2 startPosition;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;
        //player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        // This event is raised whenever a unit despawns
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        GameoverHandler.ClientOnGameOver += ClientHandleGameover;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameoverHandler.ClientOnGameOver -= ClientHandleGameover;
    }

    private void ClientHandleGameover(string winnerName)
    {
        enabled = false;
    }

    private void Update()
    {
        // Костыль
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StarSelectionArea();
        } else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        } else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StarSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        }

        selectionArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        selectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        selectionArea.anchoredPosition = startPosition +
            new Vector2(areaWidth / 2f, areaHeight / 2f);
    }

    private void ClearSelectionArea()
    {
        selectionArea.gameObject.SetActive(false);

        // This code means that we clicked, but didn`t drag the selection area
        if(selectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }
            if (!unit.hasAuthority) { return; }

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        Vector2 min = selectionArea.anchoredPosition - (selectionArea.sizeDelta / 2f);
        Vector2 max = selectionArea.anchoredPosition + (selectionArea.sizeDelta / 2f);

        foreach(Unit unit in player.GetUnits()){
            if(SelectedUnits.Contains(unit)) { continue; } // Countinue iteration

            // Converting the world position of an object to the screen position,
            // so that we can select it by 2D dragging
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }

    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

}
