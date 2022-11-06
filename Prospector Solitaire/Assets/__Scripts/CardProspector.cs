using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum defines a variable type with a few prenamed values
public enum eCardState {
    drawpile, // The drawpile
    tableau, // The tableau of cards
    target, // The target location
    discard // The discard pile
}

public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]

    // This is how you use the enum eCardState
    public eCardState state = eCardState.drawpile;

    // The hidden-by-list is a list of the other cards that are hiding this card
    public List<CardProspector> hiddenBy = new List<CardProspector>();

    // The layoutID matches this card to the tableau XML if it's a tableau card
    public int layoutID;

    // The SlotDef class stores information pulled in from the LayoutXML <slot>
    public SlotDef slotDef;

    // This allows the card to react to being clicked
    override public void OnMouseUpAsButton()
    {
        // Call the CardClicked method on the Prospector singleton
        Prospector.s.CardClicked(this);

        // Also call the base class (Card.cs) version of this method
        base.OnMouseUpAsButton();
    }
}
