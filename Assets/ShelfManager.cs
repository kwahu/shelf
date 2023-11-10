using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager Instance { get; private set; }

    [SerializeField] public int numShelves;
    [SerializeField] public float shelfWidth;
    [SerializeField] public float shelfDepth;
    [SerializeField] public float shelfHeight;

    [SerializeField] public int minProductsPerShelf;
    [SerializeField] public int maxProductsPerShelf;

    private List<GameObject> shelves = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

void Start()
{
    InstantiateShelves();

    ProductManager.Instance.GenerateProducts();
    
    ProductManager.Instance.PopulateShelves(minProductsPerShelf,maxProductsPerShelf);

    PlacementRules.Instance.GetSlotsTable();

    

    
}

private void InstantiateShelves()
{
    for (int i = 0; i < numShelves; i++)
    {
        Vector3 position = transform.position + new Vector3(0, i * shelfHeight, 0);
        GameObject shelf = CreateShelf(position, "Shelf " + i);
        shelves.Add(shelf);
    }
}

    private GameObject CreateShelf(Vector3 position, string name)
    {
        GameObject shelf = new GameObject(name);
        shelf.transform.position = position;

        // Add the Shelf component to the shelf GameObject
        Shelf shelfComponent = shelf.AddComponent<Shelf>();
        shelfComponent.shelfWidth = shelfWidth; // Set the shelf width
        shelfComponent.InitializeSlots();

        // Create shelf geometry
        CreateShelfPart(shelf, new Vector3(0+shelfWidth/2, 0, 0));

        return shelf;
    }

    private void CreateShelfPart(GameObject parent, Vector3 localPosition)
    {
        GameObject shelfPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shelfPart.transform.parent = parent.transform;
        shelfPart.transform.localPosition = localPosition;
        shelfPart.transform.localScale = new Vector3(shelfWidth, 0.1f, shelfDepth);
    }

    public List<GameObject> GetShelves()
    {
        return shelves;
    }
}