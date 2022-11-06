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
