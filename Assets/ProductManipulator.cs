using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProductManipulator : MonoBehaviour
{
    private GameObject selectedProduct;
    List<GameObject> previousActiveSlots = null;

    private GameObject objectBeingDragged = null;

    GameObject panel;

    void Start()
    {
        // Get the panel
        panel = GameObject.Find("OccupiedPanel");
    }

void Update()
{
    HandleMouseClick();
    MoveSelectedProductWithMouse();
    UpdateOccupiedPanel();

    if (selectedProduct != null)
        {
            HighlightActiveSlot();
        }
}

private void HandleMouseClick()
{
    if (Input.GetMouseButtonDown(0))
    {
        if (selectedProduct == null)
        {
            SelectProduct();
        }
        else
        {
            PlaceProductOnShelves();
        }
    }
}

    private void UpdateOccupiedPanel()
    {
       PlacementRules.Instance.DrawIsOccupiedTable(panel);  
    }

private void MoveSelectedProductWithMouse()
{
    if (selectedProduct != null)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(selectedProduct.transform.position).z;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        selectedProduct.transform.position = newPosition;
    }
}
private void HighlightActiveSlot()
{
    // Get all shelves
    List<GameObject> shelves = ShelfManager.Instance.GetShelves();

    // Unhighlight all previously active slots
    UnhighlightPreviousActiveSlots();

    foreach (GameObject shelf in shelves)
    {
        // Get the Shelf component from the shelf GameObject
        Shelf shelfComponent = shelf.GetComponent<Shelf>();

        // Get the number of active slots based on the selected product's width
        int numberOfActiveSlots = selectedProduct.GetComponent<Product>().width;

        // Get the active slots
        List<GameObject> activeSlots = shelfComponent.GetActiveSlots(selectedProduct.GetComponent<Product>(), numberOfActiveSlots);

        if (activeSlots.Count > 0)
        {
            // Highlight the active slots
            foreach (GameObject activeSlot in activeSlots)
            {
                activeSlot.GetComponent<Slot>().Highlight();
            }

            // Update the previous active slots
            previousActiveSlots = activeSlots;
        }
    }
}

private void UnhighlightPreviousActiveSlots()
{
    if (previousActiveSlots != null)
    {
        foreach (GameObject slot in previousActiveSlots)
        {
            slot.GetComponent<Slot>().Unhighlight();
        }
    }
}

    private void PlaceProductOnShelves()
    {
        if (selectedProduct != null)
        {
            // Get all shelves
            List<GameObject> shelves = ShelfManager.Instance.GetShelves();

            foreach (GameObject shelf in shelves)
            {

                // Get the Shelf component from the shelf GameObject
                Shelf shelfComponent = shelf.GetComponent<Shelf>();

                // Check if the product can be placed on the shelf
                if (shelfComponent.CanPlaceProduct(selectedProduct.GetComponent<Product>()))
                {
                    // Place the product on the shelf
                    shelfComponent.PlaceProduct(selectedProduct.GetComponent<Product>());

                    // Parent the product model to the shelf to keep the hierarchy organized
                    selectedProduct.transform.parent = shelf.transform;

                    // snap the product to the position of the active slots include the product width offset
                    // and the slot height & width offset
                    selectedProduct.transform.position = previousActiveSlots[0].transform.position - new Vector3(0.05f, 0.05f, 0);

                    // Unhighlight the previous active slots
                    UnhighlightPreviousActiveSlots();


                    // Deselect the product
                    selectedProduct = null;

                    // Break the loop as the product has been placed
                    break;
                }
            }
        }
    }


    private void SelectProduct()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Product hitProduct = hit.transform.gameObject.GetComponent<Product>();
            if (hitProduct != null)
            {
                selectedProduct = hitProduct.gameObject;

                // Get the Shelf component from the parent of the product
                Shelf shelf = selectedProduct.transform.parent.GetComponent<Shelf>();
                if (shelf != null)
                {
                    //free up the slots
                    shelf.FreeUpSlots(selectedProduct.GetComponent<Product>());
                }
            }
        }
    }


}