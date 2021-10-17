using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CardCreator))]
public class CardCreatorEditor : Editor
{
    CardCreator cardCreator;

    private void OnEnable()
    {
        cardCreator = (CardCreator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Initialize List"))
        {
            cardCreator.InitSlabContainer();
        }

        if (GUILayout.Button("Create new Circuit"))
        {
            cardCreator.StartNewCircuit();
        }

        if (GUILayout.Button("Create all Cards"))
        {
            cardCreator.CreateCards();
        }

        if (GUILayout.Button("Shuffle Board"))
        {
            cardCreator.ShuffleBoard();
        }
    }
}