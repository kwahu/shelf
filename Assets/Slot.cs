using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private bool isOccupied = false;
    private Renderer slotRenderer;

    private readonly Color highlightColor = new Color(0, 1, 0, 0.5f); // Semi-transparent green
    private readonly Color unhighlightColor = new Color(1, 1, 1, 0.5f); // Semi-transparent white

    public bool IsOccupied 
    { 
        get { return isOccupied; } 
    }

    private void Awake()
    {
        slotRenderer = GetComponent<Renderer>();
    }

    public void MarkAsOccupied()
    {
        isOccupied = true;
    }

    public void MarkAsUnoccupied()
    {
        isOccupied = false;
    }

    public void Highlight()
    {
        slotRenderer.material.color = highlightColor;
    }

    public void Unhighlight()
    {
        slotRenderer.material.color = unhighlightColor;
    }
}