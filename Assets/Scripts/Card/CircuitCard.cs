using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitCard : Card
{

    // true if every connectable edge is correctly connected to another card
    public bool isConnected;

    //// useful to during the activation checking phase
    //public bool isChecked = false;

    // type of circuit card
    public enum CircuitType { End, Line, Turn, T, Cross, Empty };
    public CircuitType circuitType = CircuitType.Empty;

    public List<CircuitCard> connectedNeighbours;

    // list of circuit cards which are in the same circuit
    public Circuit boundCircuit;

    // to be activated, the card must be connected and all its connected neighbours must be connected as well
    // when a circuit is fully connected, all of its cards are set as active
    public override bool CheckActivation()
    {
        if (circuitType == CircuitType.Empty)
            return true;


        // TODO implement this shit
        return false;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        connectedNeighbours = new List<CircuitCard>();
    }

    public int GetCircuitID()
    {
        if (boundCircuit == null)
            return -1;

        return boundCircuit.id;
    }



    // Update isConnected for all CircuitCard of the game
    public static void UpdateCircuitConnections()
    {
        Debug.Log("Start checking circuits connection");

        Circuit.ResetCircuits();

        CircuitCard[] circuitCardVect = GameObject.FindObjectsOfType<CircuitCard>();
        foreach (CircuitCard circuitCard in circuitCardVect)
        {
            // reset the bound circuit
            circuitCard.boundCircuit = null;
            circuitCard.SetActive(false);

            // update connection state
            circuitCard.UpdateCircuitConnection();
        }
    }

    // Update isConnected
    private void UpdateCircuitConnection()
    {
        Debug.Log("card " +id+ ": checking connection");
        bool tempBool = true;

        // reset neighbour list
        connectedNeighbours = new List<CircuitCard>();

        for (int edge = 0; edge < 4; edge++)
        {
            //Debug.Log(gameObject + ": HasCircuitAtEdge(" + edge + ") = " + HasCircuitAtEdge(edge));
            if (HasCircuitAtEdge(edge)) // if we need to connect this edge
            {
                Slab.Direction usedDirection = new Slab.Direction();
                Slab adjacentSlab = supportSlab.GetNeighborForEdge(edge, out usedDirection);  // get the circuit card to connect and its direction from this


                if(adjacentSlab != null && adjacentSlab.card != null) // if there is an adjacent slab for this direction
                {
                    Debug.Log(this + ": adjacent slab is not null");
                    CircuitCard adjacentCircuitCard = adjacentSlab.card.GetComponent<CircuitCard>(); // get the attached circuit card if there is
                    if (adjacentCircuitCard != null && adjacentCircuitCard.HasCircuitAtEdge(usedDirection.edge)) // if there is actually a circuit card and it is connected
                    {
                        // tempBool = tempBool & true;
                        connectedNeighbours.Add(adjacentCircuitCard); // add it to the neighbor list
                    }
                    else
                    {
                        tempBool = false; // else this card is not totally connected
                    }

                }
                else
                {
                    tempBool = false; // else this card is not totally connected
                }

                //tempBool = tempBool && checkConnection;
                //Debug.Log(gameObject + ": check circuit for edge: " + edge + "; res = " + tempBool);

            }
        }
        isConnected = tempBool;
        //Debug.Log(gameObject + ": new connection state: " + isConnected);
    }

    // check if there is a valid slab to connect with adjacent to the given edge
    // if there is, add them to the neighbour list
    private bool CheckEdgeConnection_old(int edge)
    {
        //Debug.Log(gameObject + ": check edge connection for edge " + edge);
        Slab tmpSlab = null;
        CircuitCard adjacentCircuitCard = null;
        Slab.Direction[] dirVect = supportSlab.DirectionForEdge(edge);
        Slab.Direction usedDirection = null;

        // check the slab in the first direction
        GameObject objInDirection = supportSlab.GetSlabInDirection(dirVect[0].direction);
        if (objInDirection != null)
        {
            tmpSlab = objInDirection.GetComponent<Slab>();

            // check if the detected object is a circuitCard
            if (tmpSlab != null && tmpSlab.card.GetComponent<CircuitCard>() != null)
            {
                adjacentCircuitCard = tmpSlab.card.GetComponent<CircuitCard>();
                usedDirection = dirVect[0]; // update usedDirection
                //Debug.Log(gameObject + ": found correct slab in the direction " + edge + "; direction[0]: " + usedDirection);
            }
        }

        // if we didn't find an appropriate circuit Card, check the second possible location
        if (adjacentCircuitCard == null)
        {
            objInDirection = supportSlab.GetSlabInDirection(dirVect[1].direction);
            if (objInDirection != null)
            {
                tmpSlab = objInDirection.GetComponent<Slab>();

                if (tmpSlab != null && tmpSlab.card.GetComponent<CircuitCard>() != null)
                {
                    adjacentCircuitCard = tmpSlab.card.GetComponent<CircuitCard>();
                    usedDirection = dirVect[1];
                    //Debug.Log(gameObject + ": found correct slab in the direction " + edge + "; direction[1]: " + usedDirection);
                }
            }
        }

        // if we still havn't find any usable circuitCard
        if (adjacentCircuitCard == null)
        {
            //Debug.Log(gameObject + ": no correct slab in the direction " + edge);
            return false;
        }

        bool res = adjacentCircuitCard.HasCircuitAtEdge(usedDirection.edge);

        if(res)// add the card to the adjacent list
            connectedNeighbours.Add(adjacentCircuitCard);

        return res;
    }

    // Does the Card have a circuit to connect at the given Edge ?
    public  bool HasCircuitAtEdge(int edge)
    {
        int turnedEdge = (edge - rotation + 4) % 4;
        //Debug.Log("turnedEdge: " + turnedEdge);
        switch (circuitType)
        {
            case CircuitType.Empty:
                return false;
            case CircuitType.End:
                return turnedEdge == 0;
            case CircuitType.Line:
                return (turnedEdge % 2 == 0);
            case CircuitType.Turn:
                return (turnedEdge == 3 || turnedEdge == 0);
            case CircuitType.T:
                return (turnedEdge != 2);
            case CircuitType.Cross:
                return true;
            default:
                return false;
        }
    }

    public static CircuitType StringToType(string typeName)
    {
        CircuitType circuitType;
        switch (typeName)
        {
            case "Empty":
                circuitType = CircuitCard.CircuitType.Empty;
                break;
            case "Line":
                circuitType = CircuitCard.CircuitType.Line;
                break;
            case "End":
                circuitType = CircuitCard.CircuitType.End;
                break;
            case "Turn":
                circuitType = CircuitCard.CircuitType.Turn;
                break;
            case "T":
                circuitType = CircuitCard.CircuitType.T;
                break;
            case "Cross":
                circuitType = CircuitCard.CircuitType.Cross;
                break;
            default:
                Debug.LogError("Error on card instanciation : no model found for circuit card type \"" + typeName+ "\"");
                return CircuitType.Empty;
        }
        return circuitType;
    }

    // set the type and rotation given an array of edge
    // e.g. [0,1,0,1] gives [Line,1]
    public static void EdgesToType(bool[] edges, out CircuitType type, out int rotation)
    {
        int c = 0; // number of connected edges
        type = CircuitType.Empty;
        rotation = 0;

        for(int i = 0; i < 4; i++)
        {
            if (edges[i])
            {
                c++;
            }
        }

        // number of edge
        switch (c)
        {
            case 0:
                type = CircuitType.Empty;
                rotation = 0;
                break;
            case 1:
                type = CircuitType.End;
                for (int i = 0; i < 4; i++)
                {
                    if (edges[i])
                        rotation = i;
                }
                break;
            case 2:
                rotation = 0;
                for (int i = 1; i < 5; i++)
                {
                    if (edges[(i - 1) % 4] && edges[i%4])
                    {
                        type = CircuitType.Turn;
                        rotation = (i%4);
                    }
                    else if (edges[i%4] && edges[(i + 2) % 4])
                    {
                        type = CircuitType.Line;
                        rotation = (i % 4);
                    }

                }
                break;

            case 3:
                type = CircuitType.T;
                rotation = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!edges[i])
                        rotation = ((i + 2) % 4);
                }
                break;
            case 4:
                type = CircuitType.Cross;
                rotation = 0;
                break;

        }
    }
}