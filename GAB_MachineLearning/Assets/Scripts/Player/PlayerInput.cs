using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask interactable;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Camera camera;
    private Ray ray;

    private InteractableObject currentInteractable;
    private bool currentInteractableSelected;

    [SerializeField] private float distanceToMove;

    private void Update()
    {
        CastRay();
        CheckInteractable();
        CheckInput();
        MoveInteractable();
        MoveInteractable();
        SetInteractableAltitude();
    }

    private void CastRay()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
    }

    private void CheckInteractable()
    {
        if (currentInteractableSelected) return;

        if (Physics.Raycast(ray.origin, ray.direction, out var hit, float.PositiveInfinity, interactable))
        {
            var newInteractable = hit.transform.parent.GetComponent<InteractableObject>();
            if (currentInteractable != null && newInteractable != currentInteractable) currentInteractable.Release();
            currentInteractable = newInteractable;
            currentInteractable.Hover();
        }
        else
        {
            if (currentInteractable) currentInteractable.Release();
            currentInteractable = null;
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentInteractableSelected)
            {
                currentInteractableSelected = false;
                currentInteractable.Unselected();
                return;
            }

            if (currentInteractable != null)
            {
                currentInteractableSelected = true;
                currentInteractable.Select();
            }
        }
    }

    private void MoveInteractable()
    {
        if (!currentInteractableSelected) return;

        if (Physics.Raycast(ray.origin, ray.direction, out var hit, float.PositiveInfinity, ground))
        {
            var newPos = hit.point;
            newPos.y = currentInteractable.transform.position.y;

            newPos.x = math.round(newPos.x);            
            newPos.z = math.round(newPos.z);

            if ((currentInteractable.transform.position - newPos).magnitude < distanceToMove) return;

            currentInteractable.transform.DOMove(newPos, .1f);
        }
    }

    private void SetInteractableAltitude()
    {
        if (!currentInteractableSelected) return;

        var deltaY = Input.mouseScrollDelta.y;
        if (deltaY == 0) return;
        var pos = currentInteractable.transform.position;
        pos.y += deltaY;
        pos.y = math.clamp(pos.y, -7, 0);
        currentInteractable.transform.DOMove(pos, .1f);
    }
}