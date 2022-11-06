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

         // Set which cards are hiding others
         foreach (CardProspector tCP in tableau)
         {
             foreach (int hid in tCP.slotDef.hiddenBy)
             {
                 cp = FindCardByLayoutID(hid);
                 tCP.hiddenBy.Add(cp);
             }
         }

         // Set up the initial target card
         MoveToTarget(Draw());
         // ^ Arrange the initial target card
         UpdateDrawPile();
    }

    CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach (CardProspector tCP in tableau)
        {
            // Search through all cards in the tableau List<>
            if (tCP.layoutID == layoutID)
            {
                // If the card has the same ID, return it
                return (tCP);
            }
        }

        // If it's not found, return null
        return (null);
    }

    // This turns cards in the Mine face-up or face-down
    void SetTableauFaces()
    {
        foreach (CardProspector cd in tableau)
        {
            bool fup = true; // Assume the card will be face-up

            foreach (CardProspector cover in cd.hiddenBy)
            {
                // If either of the covering cards are in the tableau
                if (cover.state == eCardState.tableau)
                {
                    fup = false; // then this card is face-down
                }
            }

            cd.faceUp = fup; // Set the value on the card
        }
    }

    // Moves the current target to the discardPile
    void MoveToDiscard(CardProspector cd)
    {
        // Set the state of the card to discard
        cd.state = eCardState.discard;
        discardPile.Add(cd); // Add it to the discardPile List<>
        cd.transform.parent = layoutAnchor; // Update its transform parent

        // Position this card on the discardPile
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
        cd.faceUp = true;

        // Place it on top of the pile for depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    // Make cd the new target card
    void MoveToTarget(CardProspector cd)
    {
        // If there is currently a target card, move it to discardPile
        if (target != null) MoveToDiscard(target);
        target = cd; // cd is the new target
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;

        // Move to the target position
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);
        cd.faceUp = true; // Make it face-up

        // Set the depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }

    // Arranges all the cards of the drawPile to show how many are left
    void UpdateDrawPile()
    {
        CardProspector cd;

        // Go through all the cards of the drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            // Position it correctly with the layout.drawPile.stagger
            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x), layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), -layout.drawPile.layerID + 0.1f * i);
            cd.faceUp = false; // Make them all face-down
            cd.state = eCardState.drawpile;

            // Set depth sorting
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }

    // CardClicked is called any time a card in the game is clicked
    public void CardClicked(CardProspector cd)
    {
        // The reaction is determined by the state of the clicked card
        switch (cd.state)
        {
            case eCardState.target:
                // Clicking the target card does nothing
                break;

            case eCardState.drawpile:
                // Clicking any card in the drawPile will draw the next card
                MoveToDiscard(target); // Moves the target to the discardPile
                MoveToTarget(Draw()); // Moves the next drawn card to the target
                UpdateDrawPile(); // Restacks the drawPile
                ScoreManager.EVENT(eScoreEvent.draw);
                // FloatingScoreHandler(eScoreEvent.draw);
                break;

            case eCardState.tableau:
                // Clicking a card in the tableau will check if it's a valid play
                bool validMatch = true;
                if (!cd.faceUp)
                {
                    // If the card is face-down, it's not valid
                    validMatch = false;
                }
                if (!AdjacentRank(cd, target))
                {
                    // If it's not an adjacent rank, it's not valid
                    validMatch = false;
                }
                if (!validMatch) return; // return if not valid

                // If we got here, then the card is valid
                tableau.Remove(cd); // Remove it from the tableau List
                MoveToTarget(cd); // Make it the target card
                SetTableauFaces(); // Update tableau card face-ups
                ScoreManager.EVENT(eScoreEvent.mine);
                // FloatingScoreHandler(eScoreEvent.mine);
                break;
        }

        // Check to see whether the game is over or not
        CheckForGameOver();
    }

    // Test whether the game is over
    void CheckForGameOver()
    {
        // If the tableau is empty, the game is over
        if (tableau.Count == 0)
        {
            // Call GameOver() with a win
            GameOver(true);
            return;
        }

        // If there are still cards in the draw pile, the game's not over
        if (drawPile.Count > 0)
        {
            return;
        }

        // Check for remaining valid plays
        foreach (CardProspector cd in tableau)
        {
            if (AdjacentRank(cd, target))
            {
                // If there is a valid play, the game's not over
                return;
            }
        }

        // Since there are no valid plays, the game is over
        // Call GameOver with a loss
        GameOver(false);
    }

    // Called when the game is over. Simple for now, but expandable
    void GameOver(bool won)
    {
        if (won)
        {
            ScoreManager.EVENT(eScoreEvent.gameWin);
        }
        else
        {
            ScoreManager.EVENT(eScoreEvent.gameLoss);
        }
        
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    public bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
        // If either card is face-down, it's not adjacent.
        if (!c0.faceUp || !c1.faceUp) return (false);

        // If they are 1 apart, they are adjacent
        if (Mathf.Abs(c0.rank - c1.rank) == 1)
        {
            return (true);
        }

        // If one is Ace and the other King, they are adjacent
        if (c0.rank == 1 && c1.rank == 13) return (true);
        if (c0.rank == 13 && c1.rank == 1) return (true);

        // Otherwise, return false
        return (false);
    }

}
