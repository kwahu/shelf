using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
    // Reverse the order of the rows
    foreach (List<GameObject> row in Enumerable.Reverse(slotsTable))
    {
        // Set the constraint to FixedColumnCount and the constraintCount to the number of slots in the row
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = row.Count;

        int columnNumber = 0;
        // Reverse the order of the columns
        foreach (GameObject slot in Enumerable.Reverse(row))
        {
            // Get the Category of the slot
            Slot slotComponent = slot.GetComponent<Slot>();
            string category = slotComponent.Category;

            // Get or create the text component
            Text text = GetOrCreateTextComponent(panel, rowNumber, columnNumber);

            // Set the text of the UI text element to the Category
            text.text = category;

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
        gridLayout.spacing = new Vector2(10, 20); // Set the spacing to 20px
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
public void CheckAllColors()
{
    HashSet<string> uniqueCategories = new HashSet<string>();

    foreach (var row in slotsTable)
    {
        foreach (var slotObject in row)
        {
            var slot = slotObject.GetComponent<Slot>();
            var category = slot.Category;

            if (category != null)
            {
                uniqueCategories.Add(category);
            }
        }
    }

    foreach (var category in uniqueCategories)
    {
        var isAdjacent = AreSameCategoryProductsAdjacent(category);
        Debug.Log($"Category {category} is {(isAdjacent ? "" : "not ")}adjacent.");
    }
}
public bool AreSameCategoryProductsAdjacent(string category)
{
    // Iterate over the slots table
    for (int i = 0; i < slotsTable.Count; i++)
    {
        for (int j = 0; j < slotsTable[i].Count; j++)
        {
            // Get the Slot component from the GameObject at the current coordinates
            Slot slot = slotsTable[i][j].GetComponent<Slot>();

            // If the slot's category matches the given category
            if (slot.Category == category)
            {
                // If there is any slot of the given category that doesn't have an adjacent slot of the same category, return false
                if (!IsAdjacentSlotSameCategory((i, j), category))
                {
                    Debug.Log($"Slot at ({i}, {j}) is not adjacent to a slot of the same category.");
                    return false;
                }
            }
        }
    }

    // If all slots of the given category have at least one adjacent slot of the same category, return true
    return true;
}

private bool IsAdjacentSlotSameCategory((int, int) coord, string category)
{
    // Define the directions for checking adjacent slots
    // -1, 0 represents the slot above
    // 0, 1 represents the slot to the right
    // 1, 0 represents the slot below
    // 0, -1 represents the slot to the left
    int[] dx = { -1, 0, 1, 0 };
    int[] dy = { 0, 1, 0, -1 };

    // Loop through each direction
    for (int direction = 0; direction < 4; direction++)
    {
        // Calculate the coordinates of the adjacent slot
        int newX = coord.Item1 + dx[direction];
        int newY = coord.Item2 + dy[direction];

        // Check if the coordinates are within the bounds of the slots table
        if (newX >= 0 && newX < slotsTable.Count && newY >= 0 && newY < slotsTable[0].Count)
        {
            // Get the Slot component from the GameObject at the adjacent coordinates
            Slot slot = slotsTable[newX][newY].GetComponent<Slot>();

            // If the slot's category matches the given category, return true
            if (slot.Category == category)
            {
                return true;
            }
        }
    }

    // If no adjacent slot of the same category is found, return false
    return false;
}
}