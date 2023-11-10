using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public bool IsOccupied { get; private set; } = false;
    private Renderer slotRenderer;

    private void Awake()
    {
        slotRenderer = GetComponent<Renderer>();
    }

    public void Occupy()
    {
        IsOccupied = true;
    }

    public void Free()
    {
        IsOccupied = false;
    }
public void Highlight()
{
    slotRenderer.material.color = new Color(0, 1, 0, 0.5f); // Semi-transparent green
}

public void Unhighlight()
{
    slotRenderer.material.color = new Color(1, 1, 1, 0.5f); // Semi-transparent white
}

}