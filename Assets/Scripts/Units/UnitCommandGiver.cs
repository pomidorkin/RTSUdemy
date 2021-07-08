using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandle unitSelectionHandle = null;
    [SerializeField] private LayerMask layerMask = new LayerMask(); // LayerMask is a struct, that wy we don`t initialize it as a null
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.hasAuthority) {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
        }

        TryMove(hit.point);
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandle.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarger(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandle.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
