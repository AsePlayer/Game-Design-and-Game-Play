using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Suits
    public bool startFaceUp = false;
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;

    // Prefabs
    public GameObject prefabCard;
    public GameObject prefabSprite;

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
        // This creates an anchor for all the Card GameObjects in the Hierarchy
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        // Initialize the Dictionary of SuitSprites with necessary Sprites
        dictSuits = new Dictionary<string, Sprite>()
        {
            {"C", suitClub},
            {"D", suitDiamond},
            {"H", suitHeart},
            {"S", suitSpade}
        };
        
        ReadDeck(deckXMLText);

        MakeCards();
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

    // Get the proper CardDefinition for a card of type cardRank and cardSuit
    public CardDefinition GetCardDefinitionByRank(int rnk)
    {
        foreach(CardDefinition cd in cardDefs)
        {
            if(cd.rank == rnk)
            {
                return(cd);
            }
        }

        return(null);
    }
    
    // Make the Card GameObjects
    public void MakeCards()
    {
        // cardNames will be the names of cards to build
        // Each suit goes from 1 to 14 (e.g., C1 to C14 for Clubs)

        cardNames = new List<string>();
        string[] letters = new string[] {"C", "D", "H", "S"};

        foreach (string s in letters)
        {
            for(int i = 0; i < 13; i++)
            {
                cardNames.Add(s + (i + 1));
            }
        }

        cards = new List<Card>(); // Init the List of Cards

        // Iterate through all of the card names that were just made
        for (int i = 0; i < cardNames.Count; i++)
        {
            // Make the card and add it to the cards Deck
            cards.Add(MakeCard(i));
        }
    }

    private Card MakeCard(int cNum) 
    {
        // Create a new Card GameObject
        GameObject cgo = Instantiate(prefabCard) as GameObject;

        // Set the transform parent of the new card to deckAnchor
        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>(); // Get the Card component

        // This line stacks the cards so that they're all in nice rows
        cgo.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);

        // Assign basic values to the Card
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));

        if(card.suit == "D" || card.suit == "H")
        {
            card.colS = "Red";
            card.color = Color.red;
        }

        // Pull the metadata for this card
        card.def = GetCardDefinitionByRank(card.rank);
        
        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);
        
        return(card);
    }

    // These private variables will be reused several times in helper methods
    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    private void AddDecorators(Card card)
    {
        // Add Decorators
        foreach(Decorator deco in decorators)
        {
            if(deco.type == "suit")
            {
                // Instantiate a Sprite GameObject
                _tGO = Instantiate(prefabSprite) as GameObject;

                // Get the SpriteRenderer Component
                _tSR = _tGO.GetComponent<SpriteRenderer>();

                // Set the SpriteRenderer's sprite to the proper suit
                _tSR.sprite = dictSuits[card.suit];
            }
            else
            {
                // Instantiate a Sprite GameObject
                _tGO = Instantiate(prefabSprite) as GameObject;

                // Get the SpriteRenderer Component
                _tSR = _tGO.GetComponent<SpriteRenderer>();

                // Get the proper Sprite to show this rank
                _tSp = rankSprites[card.rank];

                // Assign this rank Sprite to the SpriteRenderer
                _tSR.sprite = _tSp;

                // Set the color of the rank
                _tSR.color = card.color;
            }

            // Make the deco Sprites render above the Card
            _tSR.sortingOrder = 1;

            // Make the decorator Sprite a child of the Card
            _tGO.transform.parent = card.transform;

            // Set the localPosition of the decorator
            _tGO.transform.localPosition = deco.loc;

            // Flip the decorator if needed
            if(deco.flip)
            {
                // An Euler rotation of 180° around the Z-axis will flip it 
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            // Set the scale of the decorator
            if(deco.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }

            // Name this GameObject so it's easy to see
            _tGO.name = deco.type;

            // Add this GameObject to the List of card.decoGOs
            card.decoGOs.Add(_tGO);
        }
    }

    private void AddPips(Card card)
    {
        // For each of the pips in the definition
        foreach(Decorator pip in card.def.pips)
        {
            // Instantiate a Sprite GameObject
            _tGO = Instantiate(prefabSprite) as GameObject;

            // Set the parent to be the card GameObject
            _tGO.transform.parent = card.transform;

            // Set the position to that specified in the XML
            _tGO.transform.localPosition = pip.loc;

            // Flip it if necessary
            if(pip.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            // Scale it if necessary (only for the Ace)
            if(pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }

            // Give this GameObject a name
            _tGO.name = "pip";

            // Get the SpriteRenderer Component
            _tSR = _tGO.GetComponent<SpriteRenderer>();

            // Set the Sprite to the proper suit
            _tSR.sprite = dictSuits[card.suit];

            // Set sortingOrder to 1 so it renders above the card
            _tSR.sortingOrder = 1;

            // Add this GameObject to the List of pips
            card.pipGOs.Add(_tGO);
        }
    }

    private void AddFace(Card card)
    {
        if (card.def.face == "")
        {
            return; // No need to run if this isn't a face card
        }

        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();

        // Generate the right name and pass it to GetFace()
        _tSp = GetFace(card.def.face + card.suit);
        _tSR.sprite = _tSp;
        _tSR.sortingOrder = 1;
        _tGO.transform.parent = card.transform;
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }

    private Sprite GetFace(string faceS)
    {
        foreach(Sprite tS in faceSprites)
        {
            if(tS.name == faceS)
            {
                return(tS);
            }
        }

        return(null);
    }

    private void AddBack(Card card)
    {
        // Add Card Back
        // The Card_Back will be able to cover everything else on the Card
        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = cardBack;
        _tGO.transform.parent = card.transform;
        _tGO.transform.localPosition = Vector3.zero;

        // This is a high sortingOrder than anything else
        _tSR.sortingOrder = 2;
        _tGO.name = "back";
        card.back = _tGO;

        // Default to face-up
        card.faceUp = startFaceUp; // Use the property faceUp of Card
    }
}
