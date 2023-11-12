using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    public int width;
    public int height;

    public string category;

    // Create a dictionary to map category names to colors
    private static readonly Dictionary<string, Color> categoryColors = new Dictionary<string, Color>
    {
        { "red", Color.red },
        { "blue", Color.blue },
        { "green", Color.green }
    };
        // Add a static property to get the list of possible categories
    public static List<string> Categories
    {
        get { return new List<string>(categoryColors.Keys); }
    }

        // Add a static method to get the color of a category
    public static Color GetColorOfCategory(string category)
    {
        if (categoryColors.TryGetValue(category, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning($"No color defined for category {category}");
            return Color.white; // Return white as a default color
        }
    }

    // Add a method to set the color of a model based on its category
    public void SetColorBasedOnCategory()
    {
        if (categoryColors.TryGetValue(category, out Color color))
        {
            GetComponent<Renderer>().material.color = color;
        }
        else
        {
            Debug.LogWarning($"No color defined for category {category}");
        }
    }
}