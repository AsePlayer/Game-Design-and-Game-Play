using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class Deck : MonoBehaviour
{
    [Header("Set Dynamically")]
    // Create new xml reader from System.xml
    public TextAsset xmlr;

    // Create a new deck of cards
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;

    // InitDeck is called by Prospector when it is ready
    public void InitDeck(string deckXMLText)
    {
        ReadDeck(deckXMLText);
    }

    // ReadDeck parses the XML file passed to it into CardDefinitions
    public void ReadDeck(string deckXMLText)
    {
        // Parse xml with deckXMLText
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(deckXMLText);

        string s = "xml[0] decorator[0] ";
        s += "type=" + xmlDoc["xml"]["decorator"].Attributes["type"].Value;
        s += " x=" + xmlDoc["xml"]["decorator"].Attributes["x"].Value;
        s += " y=" + xmlDoc["xml"]["decorator"].Attributes["y"].Value;
        s += " scale=" + xmlDoc["xml"]["decorator"].Attributes["scale"].Value;
        // print(s);

        // Read decorators for all cards
        decorators = new List<Decorator>(); // Init the List of Decorators
        // Grab a reference to all the decorators in the XML file
        XmlNodeList decos = xmlDoc.SelectNodes("xml/decorator");

        Decorator deco;

        // Iterate through all the decorators
        for(int i = 0; i < decos.Count; i++)
        {
            // Create a new Decorator instance
            deco = new Decorator();
            deco.type = decos[i].Attributes["type"].Value;

            // bool deco.flip is true if the text of the flip attribute is "1"
            deco.flip = (decos[i].Attributes["flip"].Value == "1");

            // floats need to be parsed from the attrivute strings
            deco.scale = float.Parse(decos[i].Attributes["scale"].Value);

            // Vector3 loc takes the x, y, and z attributes
            deco.loc.x = float.Parse(decos[i].Attributes["x"].Value);
            deco.loc.y = float.Parse(decos[i].Attributes["y"].Value);
            deco.loc.z = float.Parse(decos[i].Attributes["z"].Value);

            // Add the temporary deco to the List decorators
            decorators.Add(deco);
        }

        // Read pip locations for each card number
        cardDefs = new List<CardDefinition>(); // Init the List of Cards
        // Grab a reference to all the cards in the XML file
        XmlNodeList cards = xmlDoc.SelectNodes("xml/card");

        // Iterate through all the cards
        for(int i = 0; i < cards.Count; i++)
        {
            // Create a new CardDefinition instance
            CardDefinition cDef = new CardDefinition();
            cDef.rank = int.Parse(cards[i].Attributes["rank"].Value);

            // Retrieve all the pips of this card
            XmlNodeList pips = cards[i].SelectNodes("pip");

            // If there are pips, iterate through them
            if(pips != null)
            {
                for(int j = 0; j < pips.Count; j++)
                {
                    deco = new Decorator();
                    deco.type = "pip";
                    deco.flip = (pips[j].Attributes["flip"].Value == "1");
                    deco.loc.x = float.Parse(pips[j].Attributes["x"].Value);
                    deco.loc.y = float.Parse(pips[j].Attributes["y"].Value);
                    deco.loc.z = float.Parse(pips[j].Attributes["z"].Value);
            
                    if(pips[j].Attributes["scale"] != null)
                    {
                        deco.scale = float.Parse(pips[j].Attributes["scale"].Value);
                    }

                    cDef.pips.Add(deco);
                }
            }

            // Face cards (Jack, Queen, King) have a face attribute
            if(cards[i].Attributes["face"] != null)
            {
                cDef.face = cards[i].Attributes["face"].Value;
            }
            
            // Add this cardDefinition to the cardDefs List
            cardDefs.Add(cDef);
        }
    }

}
