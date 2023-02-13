using System;
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
            Debug.Log(currentInteractable.name);
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
            newPos.y = 0;

            newPos.x = math.round(newPos.x);            
            newPos.z = math.round(newPos.z);

            if ((currentInteractable.transform.position - newPos).magnitude < distanceToMove) return;

            currentInteractable.transform.position = newPos;
        }
    }
}