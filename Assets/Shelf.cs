using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private float shelfWidth;
    public float ShelfWidth 
    { 
        get { return shelfWidth; } 
        set { shelfWidth = value; } 
    }

    [SerializeField] private List<GameObject> slots;

    private const float SlotWidth = 0.1f;
    private const float SlotScale = 0.1f;
    private const float SlotPositionAdjustment = 0.05f;

      private readonly Color slotColor = new Color(1, 1, 1, 0.5f); // Semi-transparent white


    public List<GameObject> GetSlots()
    {
        return slots;
    }
    
    public void CreateAndInitializeSlots()
    {
        int slotCount = CalculateSlotCount();
        slots = new List<GameObject>(slotCount);

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = CreateSlot(i);
            slots.Add(slot);
        }
    }

    private int CalculateSlotCount()
    {
        return (int)(shelfWidth / SlotWidth);
    }


    private GameObject CreateSlot(int index)
    {
        GameObject newSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newSlot.name = "Slot " + index;
        newSlot.transform.localScale = new Vector3(SlotScale, SlotScale, SlotScale);
        // Set the position of the slot and adjust it by half the slot's width
        newSlot.transform.position = transform.position + new Vector3(index * SlotScale + SlotPositionAdjustment, SlotScale, 0);
        newSlot.transform.parent = transform;
        SetSlotMaterial(newSlot);

        // Add a Slot component to the slot
        newSlot.AddComponent<Slot>();

        return newSlot;
    }
  

    private void SetSlotMaterial(GameObject slot)
    {
        Renderer slotRenderer = slot.GetComponent<Renderer>();
        slotRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        slotRenderer.material.color = slotColor;
    }

    int GetCollidingSlotIndex(Product product)
    {
        // Get the product's collider
        Collider productCollider = product.GetComponent<Collider>();

        foreach (GameObject slot in slots)
        {
            // Get the Slot and Collider components from the slot GameObject
            Slot slotComponent = slot.GetComponent<Slot>();
            Collider slotCollider = slot.GetComponent<Collider>();

            // If the product's collider is intersecting with the slot's collider, return the slot's index
            if (productCollider.bounds.Intersects(slotCollider.bounds))
            {
                return slots.IndexOf(slot);
            }
        }

        // If the product is not colliding with any slot, return -1
        return -1;
    }

    public bool CanPlaceProduct(Product product)
    {
        // Get the index of the slot that the product is colliding with
        int index = GetCollidingSlotIndex(product);

        if (index == -1)
        {
            // If the product is not colliding with any slot, it can't be placed
            return false;
        }

        // Check the adjacent slots to see if they are occupied
        {
            for (int i = 0; i < product.width; i++)
            {
                // Check if the slot at the current index plus the iteration is within the shelf's slots
                if (index + i >= slots.Count)
                {
                    return false;
                }

                // Get the Slot script attached to the slot
                Slot slotScript = slots[index + i].GetComponent<Slot>();

                // Check if the slot at the current index plus the iteration is occupied
                if (slotScript.IsOccupied)
                {
                    return false;
                }
            }
        }

        // If none of the slots are occupied, the product can be placed
        return true;
    }



    public void PlaceProduct(Product product)
    {
        // Calculate the number of slots based on the product's width
        int numberOfSlots = product.width;

        // Get the active slots
        List<GameObject> activeSlots = GetActiveSlots(product, numberOfSlots);

        foreach (GameObject slot in activeSlots)
        {
            // If the product is colliding with a slot, occupy the slot
            slot.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);

            // Get the Slot component from the slot GameObject and call its Occupy method
            Slot slotComponent = slot.GetComponent<Slot>();
            slotComponent.MarkAsOccupied();
        }
    }

    public List<GameObject> GetActiveSlots(Product product, int numberOfSlots)
    {
        
        // Get the product's collider
        Collider productCollider = product.GetComponent<Collider>();

        // Create a list to store the active slots
        List<GameObject> activeSlots = new List<GameObject>();

        foreach (GameObject slot in slots)
        {
            Slot slotComponent = slot.GetComponent<Slot>();

            // Check if the slot is unoccupied and the product's collider is colliding with the slot's collider
            if (!slotComponent.IsOccupied && productCollider.bounds.Intersects(slot.GetComponent<Collider>().bounds))
            {
                // If the slot is unoccupied and the product is colliding with it, add the slot to the active slots list
                activeSlots.Add(slot);

                // If the number of active slots is equal to the number of slots, break the loop
                if (activeSlots.Count == numberOfSlots)
                {
                    break;
                }
            }
        }
        // Return the active slots
        return activeSlots;
    }

    public GameObject GetSlotAtPosition(int position)
    {
        if (position >= 0 && position < slots.Count)
        {
            return slots[position];
        }
        else
        {
            return null;
        }
    }
public void FreeUpSlots(Product product)
{
    // Get the Collider component from the product GameObject
    Collider productCollider = product.GetComponent<Collider>();

    foreach (GameObject slot in slots)
    {
        // Get the Slot and Collider components from the slot GameObject
        Slot slotComponent = slot.GetComponent<Slot>();
        Collider slotCollider = slot.GetComponent<Collider>();

        // If the product's collider is intersecting with the slot's collider, call the Free method of the Slot component
        if (productCollider.bounds.Intersects(slotCollider.bounds))
        {
            slotComponent.MarkAsUnoccupied();
        }
    }
}

}