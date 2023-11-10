using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance { get; private set; }

    public List<GameObject> productPrefabs;

    //number of products to generate
    [SerializeField] public int numProducts;

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


public void GenerateProducts()
{
    Color[] colors = { Color.red, Color.green, Color.blue };

    for (int i = 0; i < numProducts; i++)
    {
        // Create a new cube primitive for each product
        GameObject productModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        productModel.name = "Product " + i;

        // Add the Product component to the GameObject
        Product product = productModel.AddComponent<Product>();

        product.productColor = colors[i % colors.Length]; // Use modulus to cycle through colors
        product.width = UnityEngine.Random.Range(1, 3); // Random value between 1 and 3
        product.height = UnityEngine.Random.Range(1, 3); // Random value between 1 and 2

        // Set the color of the primitive's material to a semi-transparent color
        productModel.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));
        productModel.GetComponent<Renderer>().material.color = new Color(product.productColor.r, product.productColor.g, product.productColor.b, 0.5f);

        // Set the scale of the primitive to match the product's width and height
        productModel.transform.localScale = new Vector3(product.width/10.0f, product.height/10.0f, 0.3f);

        // Add a BoxCollider to the product
        productModel.AddComponent<BoxCollider>();
        // Add a Rigidbody to the product
        //Rigidbody rb = productModel.AddComponent<Rigidbody>();
        //rb.useGravity = false; // Disable gravity if you don't want the product to fall

        productPrefabs.Add(productModel);
    }
}

private GameObject GetRandomProduct(List<GameObject> productPrefabsCopy)
{
    // Get a random index
    int index = UnityEngine.Random.Range(0, productPrefabsCopy.Count);

    // Get the product at the random index
    GameObject product = productPrefabsCopy[index];

    // Remove the product from the list so it can't be selected again
    productPrefabsCopy.RemoveAt(index);

    return product;
}
public void PopulateShelves(int minProducts, int maxProducts)
{
    List<GameObject> shelves = ShelfManager.Instance.GetShelves();

    // Create a copy of the productPrefabs list
    List<GameObject> productPrefabsCopy = new List<GameObject>(productPrefabs);

    foreach (GameObject shelf in shelves)
    {
        // Get a random number of products to place on the shelf
        int numProducts = UnityEngine.Random.Range(minProducts, System.Math.Min(maxProducts + 1, productPrefabsCopy.Count));

        // Initialize the x position for the first product
        int xPos = 0;

        for (int i = 0; i < numProducts; i++)
        {
            // Check if there are any products left to place
            if (productPrefabsCopy.Count == 0)
            {
                break;
            }

            // Get a random product from productPrefabsCopy
            GameObject randomProduct = GetRandomProduct(productPrefabsCopy);

            // Position and parent the product model
            PositionProductModelOnShelf(randomProduct, shelf, ref xPos);
        }
    }
}

private void PositionProductModelOnShelf(GameObject productModel, GameObject shelf, ref int xPos)
{
    // Get the Product component from the product model GameObject
    Product product = productModel.GetComponent<Product>();

    // Adjust the product's width and height by a factor of 10
    float adjustedWidth = product.width / 10.0f;
    float adjustedHeight = product.height / 10.0f;

    // Calculate the offset for the x and y positions based on the adjusted product's width and height
    float xOffset = adjustedWidth / 2.0f;
    float yOffset = adjustedHeight / 2.0f;

    // Set the position of the product model to the position of the shelf
    // Offset the x position by xPos and xOffset
    // Offset the y position by yOffset
    productModel.transform.position = shelf.transform.position + new Vector3(xPos * 0.1f + xOffset, yOffset, 0);

    // Parent the product model to the shelf to keep the hierarchy organized
    productModel.transform.parent = shelf.transform;

    // Get the Shelf component from the shelf GameObject
    Shelf shelfComponent = shelf.GetComponent<Shelf>();

    // Calculate the starting slot index based on the product's position relative to the shelf
    int startSlot = xPos;

    // Increase xPos by the adjusted width of the product
    xPos += productModel.GetComponent<Product>().width;

       // Mark slots as occupied
    for (int i = startSlot; i < xPos; i++)
    {
        Slot slot = shelfComponent.GetSlotAtPosition(i).GetComponent<Slot>();
        if (slot != null)
        {
            slot.Occupy();
        }
    }
}

}
