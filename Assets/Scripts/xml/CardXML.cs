using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;




public class CardXML
{
    [XmlAttribute("cardType")]
    public string CardType;

    public int Rotation;
    public string CircuitType;

    public CardXML()
    {
        CardType = "static";
        Rotation = 0;
        CircuitType = "Empty";
    }

    public CardXML(GameObject cardObject)
    {
        Card cardComp = cardObject.GetComponent<Card>();
        if(cardComp == null)
        {
            CardType = "static";
            Rotation = 0;
            CircuitType = "Empty";
            return;
        }

        CardType = cardComp.GetStringType();
        Rotation = cardComp.rotation;

        CircuitCard circuitCardComp = cardObject.GetComponent<CircuitCard>();
        if(circuitCardComp == null)
        {
            CircuitType = "Empty";
        }
        else
        {
            CircuitType = circuitCardComp.circuitType.ToString();
        }
    }


}
