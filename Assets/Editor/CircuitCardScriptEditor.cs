using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CircuitCard))]
public class CircuitCardcriptEditor : Editor
{
    CircuitCard currentCard;

    private void OnEnable()
    {
        currentCard = (CircuitCard)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Get circuit ID"))
        {
            int id = currentCard.GetCircuitID();
            if (id == -1)
            {
                Debug.Log("no bound circuit for this card");
            }
            else
            {
                Debug.Log("bound circuit id: " + id);

            }
        }
    }
}