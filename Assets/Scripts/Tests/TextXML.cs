using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextXML : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Start looking for fucking XML");
        LevelContainerXML levelContainer = LevelContainerXML.LoadFromFile(Application.dataPath + "/LevelData/Levels.xml");

        // Debugger la variable
        Debug.Log(levelContainer.Levels[0].LevelID);
        //foreach(SlabXML currSlab in levelContainer.Levels[0].Slabs)
        //{
        //    Debug.Log(currSlab);
        //}


    }
}
