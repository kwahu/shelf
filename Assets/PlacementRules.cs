using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlacementRules : MonoBehaviour
{
    // Static instance of PlacementRules
    private static PlacementRules instance;

    
    // Field to store the slots table
    [SerializeField] private List<List<GameObject>> slotsTable;

    // Method to get the instance of PlacementRules
    public static PlacementRules Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlacementRules>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    
    public void GetSlotsTable()
    {
        // Create a new 2-dimensional list to store the slots
        slotsTable = new List<List<GameObject>>();

        // Get all shelves
        List<GameObject> shelves = ShelfManager.Instance.GetShelves();

        foreach (GameObject shelf in shelves)
        {
            // Get the Shelf component from the shelf GameObject
            Shelf shelfComponent = shelf.GetComponent<Shelf>();

            // Get all slots on the shelf
            List<GameObject> slots = shelfComponent.GetSlots();
         

            // Add the slots to the 2-dimensional list
            slotsTable.Add(slots);
        }
    }
public void DrawIsOccupiedTable(GameObject panel)
{
    // Get the GridLayoutGroup component from the panel
    GridLayoutGroup gridLayout = GetOrCreateGridLayout(panel);

    int rowNumber = 0;
    foreach (List<GameObject> row in slotsTable)
    {
        // Set the constraint to FixedColumnCount and the constraintCount to the number of slots in the row
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = row.Count;

        int columnNumber = 0;
        foreach (GameObject slot in row)
        {
            // Get the IsOccupied value of the slot
            Slot slotComponent = slot.GetComponent<Slot>();
            bool isOccupied = slotComponent.IsOccupied;

            // Get or create the text component
            Text text = GetOrCreateTextComponent(panel, rowNumber, columnNumber);

            // Set the text of the UI text element to the IsOccupied value
            text.text = isOccupied.ToString();

            columnNumber++;
        }
        rowNumber++;
    }
}

private GridLayoutGroup GetOrCreateGridLayout(GameObject panel)
{
    GridLayoutGroup gridLayout = panel.GetComponent<GridLayoutGroup>();
    if (gridLayout == null)
    {
        // Add a GridLayoutGroup component to the panel if it doesn't exist
        gridLayout = panel.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(20, 20); // Set the cell size to 20px by 20px
        gridLayout.spacing = new Vector2(20, 20); // Set the spacing to 20px
    }
    return gridLayout;
}

private Text GetOrCreateTextComponent(GameObject panel, int rowNumber, int columnNumber)
{
    // Find the existing text component
    string textObjectName = $"Text_Row{rowNumber}_Column{columnNumber}";
    Transform textObjectTransform = panel.transform.Find(textObjectName);
    Text text;
    if (textObjectTransform != null)
    {
        // If the text component exists, get it
        text = textObjectTransform.GetComponent<Text>();
    }
    else
    {
        // If the text component doesn't exist, create a new one
        GameObject textObject = new GameObject(textObjectName);
        text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        // Add the UI text element to the panel
        textObject.transform.SetParent(panel.transform, false);
    }
    return text;
}
}