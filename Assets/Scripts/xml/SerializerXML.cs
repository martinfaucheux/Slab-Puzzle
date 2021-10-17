using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SerializerXML : MonoBehaviour {


    public static SerializerXML instance;

    public Transform slabContainer;

    public string newLevelName = "new_serialized_level";
    public int newLevelIndex = 0;

    public string testFilePath = "Assets/Resources/level_test.xml";
    public string realFilePath = "Assets/Resources/Levels.xml";

    private void Awake()
    {
        CheckSingleton();
    }

    private void CheckSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //public void SerializeBoard(string path = "Assets/Ressources/level_test.xml")
    //{
    //    LevelXML currentLevel = SerializeLevel();
    //    LevelContainerXML levelContainer = new LevelContainerXML();
    //    levelContainer.Levels = new List<LevelXML>() { currentLevel };

    //    var serializer = new XmlSerializer(typeof(LevelContainerXML));
    //    var stream = new FileStream(path, FileMode.Create);
    //    //serializer.Serialize(stream, this);
    //    serializer.Serialize(stream, levelContainer);
    //    stream.Close();
    //    Debug.LogWarning(this + ": Serialization successful at " + path);
    //}



    //private LevelXML SerializeLevel()
    //{
    //    // create the LevelXML object
    //    LevelXML newLevelXML = new LevelXML(newLevelName);

    //    // List of the slabs
    //    List<SlabXML> slabList = new List<SlabXML>();
        
    //    // create XML object for each slab object
    //    foreach(Transform slabTransform in slabContainer)
    //    {
    //        slabList.Add(new SlabXML(slabTransform.gameObject));
    //    }

    //    // create one block
    //    BlockXML newBlock = new BlockXML(slabList);

    //    // associate it to the LevelXML
    //    newLevelXML.Blocks = new List<BlockXML>() { newBlock };

    //    return newLevelXML;
    //}
    

    
}
