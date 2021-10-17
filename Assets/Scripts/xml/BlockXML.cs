using System.Xml.Serialization;
using System.Collections.Generic;

public class BlockXML {

    [XmlArray("Slabs"), XmlArrayItem("Slab")]
    public List<SlabXML> Slabs = new List<SlabXML>();


    public BlockXML()
    {
        Slabs = new List<SlabXML>();
    }

    public BlockXML(List<SlabXML> slabList)
    {
        Slabs = slabList;
    }
}
