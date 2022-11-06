using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

// The SlotDef class is not a subclass of MonoBehaviour, so it doesn't need a separate C# file.
[System.Serializable] // This makes SlotDefs visible in the Unity Inspector pane.

public class SlotDef {
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
    public int player;
}

public class Layout : MonoBehaviour
{
    // Parse xml with deckXMLText
    public XmlDocument xmlr;

    public Vector2 multiplier; // The offset of the next card in the layout

    // SlotDef references
    public List<SlotDef> slotDefs;
    public SlotDef drawPile;
    public SlotDef discardPile;

    // This holds all of the possible names for the layers that the cards will be on
    public string[] sortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };

    // This function is called to read in the LayoutXML.xml file

    public void ReadLayout(string xmlText)
    {
        xmlr = new XmlDocument();
        xmlr.LoadXml(xmlText); // The XML is parsed

        // Read in the multiplier, which sets card spacing
        multiplier.x = float.Parse(xmlr.SelectSingleNode("/xml/multiplier").Attributes["x"].Value);
        multiplier.y = float.Parse(xmlr.SelectSingleNode("/xml/multiplier").Attributes["y"].Value);

        // Read in the slots
        SlotDef tSD;
        // slotsX is used as a shortcut to all the <slot>s
        XmlNodeList slotsX = xmlr.SelectNodes("/xml/slot");

        for(int i = 0; i < slotsX.Count; i++)
        {
            tSD = new SlotDef(); // Create a new SlotDef instance
            if(slotsX[i].Attributes["type"] != null)
            {
                tSD.type = slotsX[i].Attributes["type"].Value;
            }
            else
            {
                // If not, set its type to "slot"l it's a card in the rows
                tSD.type = "slot";
            }

            // Various attributes are parsed into numerical values
            tSD.x = float.Parse(slotsX[i].Attributes["x"].Value);
            tSD.y = float.Parse(slotsX[i].Attributes["y"].Value);
            tSD.layerID = int.Parse(slotsX[i].Attributes["layer"].Value);
            
            // This converts the number of the sorting layer into a text layerName
            tSD.layerName = sortingLayerNames[tSD.layerID];

            switch(tSD.type)
            {
                // Pull additional attributes based on the type of this <slot>
                case "slot":
                    // Ignore slots that are face down
                    tSD.faceUp = (slotsX[i].Attributes["faceup"] != null && slotsX[i].Attributes["faceup"].Value == "1");
                    tSD.id = int.Parse(slotsX[i].Attributes["id"].Value);
                    if(slotsX[i].Attributes["hiddenby"] != null)
                    {
                        // If this card is hidden by another, set the id of the slot hiding it
                        string[] hiding = slotsX[i].Attributes["hiddenby"].Value.Split(',');
                        foreach(string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;

                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].Attributes["xstagger"].Value);
                    drawPile = tSD;
                    break;
                
                case "discardpile":
                    discardPile = tSD;
                    break;
            }
        }

    }

}
