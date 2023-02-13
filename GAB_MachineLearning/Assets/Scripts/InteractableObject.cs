using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Outline outline;
    [SerializeField] private Color hoveredColor;
    [SerializeField] private Color selectedColor;

    [ContextMenu("Init")]
    public void Init()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Hover()
    {
        outline.OutlineColor = hoveredColor;
        outline.enabled = true;
    }

    public void Release()
    {
        outline.enabled = false;
    }

    public void Select()
    {
        outline.OutlineColor = selectedColor;
    }

    public void Unselected()
    {
        outline.OutlineColor = hoveredColor;
    }
}