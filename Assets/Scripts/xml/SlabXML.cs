using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;




public class SlabXML
{
    [XmlAttribute("slabType")] public string SlabType = "static";


    public int PositionX;
    public int PositionY;
    public int PositionZ;
    public int Orientation;
    public int SlabLevel;
    public CardXML Card;

    public SlabXML()
    {
        SlabType = "static";
        PositionX = 0;
        PositionY = 0;
        PositionZ = 0;
        Orientation = 0;
        SlabLevel = 0;
        Card = null;
    }

    public SlabXML(GameObject SlabObject)
    {
        Debug.Log("Serializing a slab from " + SlabObject);

        Slab slabComp = SlabObject.GetComponent<Slab>();
        PermutableSlab permutableSlabComp = SlabObject.GetComponent<PermutableSlab>();
        if (slabComp == null)
        {
            Debug.LogError("SlabXML tried to serialize " + SlabObject.ToString() + " but this is not a slab");
            return;
        }
        Transform slabTransform = SlabObject.transform;
        SlabType = slabComp.GetSlabType();
        PositionX = (int)slabTransform.position.x;
        PositionY = (int)slabTransform.position.y;
        PositionZ = (int)slabTransform.position.z;
        Orientation = slabComp.orientation;

        if(permutableSlabComp != null)
        {
            SlabLevel = permutableSlabComp.level;
        }
        else
        {
            SlabLevel = 0;
        }

        Card assiociatedCard = slabComp.card;
        if(assiociatedCard == null)
        {
            Card = null;
        }
        else
        {
            Card = new CardXML(assiociatedCard.gameObject);
        }

    }


    public override string ToString()
    {
        string str = SlabType + " slab, pos(" + PositionX + "," + PositionY + "," + PositionZ + ") ori=" + Orientation;

        return str;
    }
}
