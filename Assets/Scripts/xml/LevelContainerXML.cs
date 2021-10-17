using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


[XmlRoot("LevelCollection")]
public class LevelContainerXML
{
    [XmlArray("Levels")]
    [XmlArrayItem("Level")]
    public List<LevelXML> Levels = new List<LevelXML>();

    public LevelContainerXML()
    {
        Levels = new List<LevelXML>();
    }

    public LevelContainerXML(List<LevelXML> levelList)
    {
        Levels = levelList;
    }

    //public LevelContainerXML(Transform slabContainer, string newLevelName, string path)
    //{
    //    LevelXML currentLevel = SerializeLevel(slabContainer, newLevelName);
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

    // create a new LevelContainerXML from the current scene
    public LevelContainerXML(Transform slabContainer)
    {
        string newLevelName = "new_level";
        if(SerializerXML.instance != null)
        {
            newLevelName = SerializerXML.instance.newLevelName;
        }

        Debug.Log("Serializing the LevelContainerXML. New level name: " + newLevelName);

        LevelXML newLevel = new LevelXML(slabContainer, newLevelName);

        this.Levels = new List<LevelXML>() { newLevel };
    }

    // create a LevelContainerXML from a xml file
    public LevelContainerXML(string filePath)
    {
        LevelContainerXML tmp = LoadFromFile(filePath);
        this.Levels = tmp.Levels;
    }

    // serialize all item in the container in a xml file at the given path
    public static LevelContainerXML SerializeBoard(Transform slabContainer, string path)
    {
        LevelContainerXML levelContainer = new LevelContainerXML(slabContainer);

        WriteFile(path, levelContainer);
        Debug.LogWarning("LevelContainerXML: Serialization of all the objects of " + slabContainer);

        return levelContainer;
    }

    // add the given level to the xml file with the given path
    public static void AddLevelToLevelList(LevelXML newLevel, int index)
    {
        // add specified level
        LevelContainerXML levelContainer = LevelContainerXML.LoadFromResource();
        levelContainer.Levels.Insert(index, newLevel);

        string path = "Assets/Resources/Levels.xml";
        if(SerializerXML.instance != null)
        {
            path = SerializerXML.instance.realFilePath;
        }

        WriteFile(path, levelContainer);
        Debug.LogWarning("LevelContainerXML: Added new level at index " + index + " in file at " + path);

    }

    public static LevelContainerXML LoadFromFile(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LevelContainerXML));
        FileStream stream = new FileStream(filePath, FileMode.Open);

        LevelContainerXML container = serializer.Deserialize(stream) as LevelContainerXML;
        stream.Close();

        return container;
    }

    public static LevelContainerXML LoadFromResource(string resourceName = "Levels")
    {
        TextAsset _xml = Resources.Load<TextAsset>(resourceName);

        if (_xml == null)
            Debug.LogError("LevelContainerXML: can't find the Levels asset");

        XmlSerializer serializer = new XmlSerializer(typeof(LevelContainerXML));
        StringReader reader = new StringReader(_xml.ToString());
        LevelContainerXML levelContainer = serializer.Deserialize(reader) as LevelContainerXML;
        reader.Close();

        if(levelContainer == null)
            Debug.LogError("LevelContainerXML: Level.xml found but can't serialize it");
        else
            Debug.Log("LevelContainerXML: successfully found Levels.xml file");

        return levelContainer;
    }

    public static void WriteFile(string filePath, LevelContainerXML levelContainer)
    {
        var serializer = new XmlSerializer(typeof(LevelContainerXML));
        var stream = new FileStream(filePath, FileMode.Create);

        serializer.Serialize(stream, levelContainer);
        stream.Close();
        Debug.LogWarning("LevelContainerXML: Serialization successfully done at " + filePath);
    }

    public LevelXML GetLevel(int id)
    {
        if (id >= Levels.Count || id < 0)
            return null;

        return Levels[id];
    }

    public LevelXML GetLevel(string id)
    {
        foreach (LevelXML level in Levels)
        {
            if (level.LevelID == id)
                return level;
        }

        // default
        return null;
    }
}

