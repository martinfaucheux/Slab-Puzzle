using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SerializerXML))]
public class SerializerXMLEditor : Editor
{
    SerializerXML currentSerializerXML;

    private void OnEnable()
    {
        currentSerializerXML = (SerializerXML)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Serialize Board"))
        {
            LevelContainerXML.SerializeBoard(currentSerializerXML.slabContainer, currentSerializerXML.testFilePath);
        }

        if (GUILayout.Button("Add Level to list"))
        {
            LevelXML newLevel = new LevelXML(currentSerializerXML.slabContainer, currentSerializerXML.newLevelName);
            LevelContainerXML.AddLevelToLevelList(newLevel, currentSerializerXML.newLevelIndex);
        }
    }
}