using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shelf : MonoBehaviour
{
    public float shelfWidth;

    [SerializeField] private List<GameObject> slots;


    public List<GameObject> GetSlots()
    {
        return slots;
    }
    
    public void InitializeSlots()
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
        return (int)(shelfWidth / 0.1f);
    }

    private GameObject CreateSlot(int index)
    {
        GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slot.name = "Slot " + index;
        slot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        slot.transform.position = transform.position + new Vector3(index * 0.1f, 0.1f, 0);
        slot.transform.parent = transform;
        SetSlotMaterial(slot);

        // Add a Slot component to the slot
        slot.AddComponent<Slot>();

        return slot;
    }

    private void SetSlotMaterial(GameObject slot)
    {
        Renderer slotRenderer = slot.GetComponent<Renderer>();
        slotRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        slotRenderer.material.color = new Color(1, 1, 1, 0.5f);
    }

    public bool CanPlaceProduct(Product product)
    {
        // Calculate the number of slots based on the product's width
        int numberOfSlots = product.width;

        for (int i = 0; i < slots.Count; i++)
        {
            // Check if the product can be placed in the current slot and the right amount of adjacent slots
            if (CanPlaceInSlot(product, i, numberOfSlots))
            {
                return true;
            }
        }

        // If the product is not colliding with any slots, it can't be placed
        return false;
    }

    private bool CanPlaceInSlot(Product product, int slotIndex, int numberOfSlots)
    {
        GameObject slot = slots[slotIndex];

        // Check if the product's collider is colliding with the slot's collider
        if (product.GetComponent<Collider>().bounds.Intersects(slot.GetComponent<Collider>().bounds))
        {
            // Get the Slot component from the slot GameObject
            Slot slotComponent = slot.GetComponent<Slot>();

            // If the slot is occupied, the product can't be placed
            if (slotComponent.IsOccupied)
            {
                return false;
            }

            // Check the right amount of adjacent slots
            for (int j = 1; j < numberOfSlots; j++)
            {
                // If there are not enough adjacent slots, the product can't be placed
                if (slotIndex + j >= slots.Count)
                {
                    return false;
                }

                // Get the Slot component from the adjacent slot GameObject
                Slot adjacentSlotComponent = slots[slotIndex + j].GetComponent<Slot>();

                // If the adjacent slot is occupied, the product can't be placed
                if (adjacentSlotComponent.IsOccupied)
                {
                    return false;
                }
            }

            // If the slot and the right amount of adjacent slots are free, the product can be placed
            return true;
        }

        return false;
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
            slotComponent.Occupy();
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
            slotComponent.Free();
        }
    }
}

}