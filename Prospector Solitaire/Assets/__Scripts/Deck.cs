using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class Deck : MonoBehaviour
{
    [Header("Set Dynamically")]
    // Create new xml reader from System.xml
    public TextAsset xmlr;   

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
        print(s);
    }

}
