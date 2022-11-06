using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit; // Suit of this card
    public int rank; // Rank of this card
    public Color color = Color.black; // Color to tint pips
    public string colS = "Black"; // or "Red"

    // This List holds all of the Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    // This List holds all of the Pip GameObjects
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back; // The GameObject of the back of the card
    public CardDefinition def; // Parsed from DeckXML.xml

    // List of the SpriteRenderer Components of this GameObject and its children
    public SpriteRenderer[] spriteRenderers;

    void Start()
    {
        SetSortOrder(0); // Ensures that the card starts properly depth sorted
    }

    public void PopulateSpriteRenderers()
    {
        // If spriteRenderers is null or empty
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            // Get SpriteRenderer Components of this GameObject and its children
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void SetSortingLayerName(string tSLN)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }

    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderers();

        // The white background of the card is on bottom (sOrd)
        // Then the back is next (sOrd+1)
        // Then the face is next (sOrd+2)
        // Then the pips are on top (sOrd+3)

        // Iterate through all the spriteRenderers as tSR
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == this.gameObject)
            {
                // If the gameObject is this.gameObject, it's the background
                tSR.sortingOrder = sOrd; // Set it's order to sOrd
                continue; // And continue to the next iteration of the loop
            }

            // Each of the children of this GameObject are named
            // switch based on the names
            switch (tSR.gameObject.name)
            {
                case "back":
                    // If the name is "back", it's the back
                    tSR.sortingOrder = sOrd + 2;
                    break;

                case "face": // If the name is "face"

                default:     // or if it's anything else
                    // Set it to the middle layer to be above the background but below the pips
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }

    public bool faceUp // Is this card face-up?
    {
        get
        {
            return (!back.activeSelf);
        }
        set
        {
            back.SetActive(!value);
        }
    }
}

[System.Serializable] // A Serializable class is able to be edited in the Inspector
public class Decorator {
    // This class stores information about each decorator or pip from DeckXML
    public string type; // The type of decorator, i.e. "pip"
    public Vector3 loc; // The location of the sprite on the card
    public bool flip = false; // Whether to flip the sprite vertically
    public float scale = 1f; // The scale of the sprite
}

[System.Serializable]
public class CardDefinition {
    // This class stores information for each rank of card
    public string face; // Sprite to use for each face card
    public int rank; // The rank of this card, 1 = Ace, 11 = Jack, 12 = Queen, 13 = King
    public List<Decorator> pips = new List<Decorator>(); // Pips used
}
