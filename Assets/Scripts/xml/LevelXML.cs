using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class LevelXML
{

    [XmlElement("LevelID")]
    public string LevelID;     // id of the level

    public float XCameraOffset = 0f;
    public float YCameraOffset = 0f;
    public float CameraSize = 7f;

    // try hints
    public string FirstHint = "";
    public string SecondHint = "";


    [XmlArray("Blocks"), XmlArrayItem("Block")]
    public List<BlockXML> Blocks;

    public LevelXML(string levelID)
    {
        LevelID = levelID;
        XCameraOffset = 0f;
        YCameraOffset = 0f;
        CameraSize = 7f;
        Blocks = new List<BlockXML>();
    }

    public LevelXML()
    {
        LevelID = "new_serialized_level";
        XCameraOffset = 0f;
        YCameraOffset = 0f;
        CameraSize = 7f;
        Blocks = new List<BlockXML>();
    }

    public LevelXML(Transform slabContainer, string newLevelName)
    {
        Debug.Log("serializing the level " + newLevelName + "; " + slabContainer.childCount + " slabs found");

        LevelID = newLevelName;
        XCameraOffset = 0f;
        YCameraOffset = 0f;
        CameraSize = 7f;

        // List of the slabs
        List<SlabXML> slabList = new List<SlabXML>();

        // create XML object for each slab object
        foreach (Transform slabTransform in slabContainer)
        {
            slabList.Add(new SlabXML(slabTransform.gameObject));
        }

        // create one block
        BlockXML newBlock = new BlockXML(slabList);

        // associate it to the LevelXML
        this.Blocks = new List<BlockXML>() { newBlock };
    }


    //[XmlArray("Slabs"), XmlArrayItem("Slab")]
    //public List<SlabXML> Slabs;

    

}
