using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// The premise of Prospector is that the player is digging down for gold, whereas the premise of Tri-Peaks is that the player is trying to climb three mountains.
// The objective of Tri-Peaks is just to clear all the cards. The objective of Prospector is to earn points by having long chains of cards, and each gold card in the chain doubles the value of the whole chain.
public class Prospector : MonoBehaviour
{
    static public Prospector s;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;

    public float xOffset = 3;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;

    public Transform layoutAnchor;

    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;

    void Awake()
    {
        s = this;
    }

    void Start()
    {
        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); // Pass DeckXML to it

        Deck.Shuffle(ref deck.cards); // Shuffle the deck

        // Card c;
        // for (int cNum = 0; cNum < deck.cards.Count; cNum++)
        // {
        //     c = deck.cards[cNum];
        //     c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        // }

        layout = GetComponent<Layout>(); // Get the Layout component
        layout.ReadLayout(layoutXML.text); // Pass DeckXML to it

        drawPile = ConvertListCardsToListCardProspectors(deck.cards);

        LayoutGame();
    }

    List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
    {
        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;

        foreach (Card tCD in lCD)
        {
            tCP = tCD as CardProspector;

            lCP.Add(tCP);
        }
        
        return (lCP);
    }

    CardProspector Draw()
    {
        CardProspector cd = drawPile[0]; // Pull the 0th CardProspector
        drawPile.RemoveAt(0); // Then remove it from List<> drawPile
        return (cd); // And return it
    }

    void LayoutGame()
    {
        // Create an empty GameObject to serve as an anchor for the tableau
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            // ^ Create an empty GameObject named _LayoutAnchor in the Hierarchy
            layoutAnchor = tGO.transform; // Grab its Transform
            layoutAnchor.transform.position = layoutCenter; // Position it
        }

        CardProspector cp;
        // Follow the layout
        foreach (SlotDef tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            // ^ Set its parent to _LayoutAnchor this will keep the tableau centered
            cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
            // ^ Set the localPosition of the card based on slotDef
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            // ^ Set layoutID and slotDef of this card
            cp.state = eCardState.tableau;
            // ^ CardProspector in the tableau
            
            cp.SetSortingLayerName(tSD.layerName); // Set the sorting layers

            tableau.Add(cp); // Add this CardProspector to the List<> tableau
        }

    //     // Set which cards are hiding others
    //     foreach (CardProspector tCP in tableau)
    //     {
    //         foreach (int hid in tCP.slotDef.hiddenBy)
    //         {
    //             cp = FindCardByLayoutID(hid);
    //             tCP.hiddenBy.Add(cp);
    //         }
    //     }

    //     // Set up the initial target card
    //     MoveToTarget(Draw());
    //     // ^ Arrange the initial target card
    //     UpdateDrawPile();
    }

}
