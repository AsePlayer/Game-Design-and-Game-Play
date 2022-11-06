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
}
