using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCreator : MonoBehaviour {

    [Tooltip("parameter determining the average amount of circuits created")]
    public float firstBranching = 0.3f; // float between 0 and 1. It determines how many connections will be created

    [Tooltip("parameter determining the minimum amount of circuits created")]
    public float ReBranching = 0.1f; // parameter between 0 and 1 determining the minimum number of branch created for the circuits 

    [Tooltip("maximum amount of empty card")]
    public int maxEmptyCard = 0;

    [Tooltip("amount of static card")]
    public int staticCard = 0;

    [Tooltip("seed used for mapping")]
    public int seed = 0;


    
    public Transform slabContainer;

    public List<Slab> uncheckedSlabs; // TODO set back to private when job is done


    private LevelInitializer _levelInitializer; // usefull to get the prefabs



    private void Start()
    {
        Random.InitState(seed == 0f ? (int)Time.time : seed); // init random with a seed, or go random if nothing provided

    }

    public int InitSlabContainer()
    {
        uncheckedSlabs = new List<Slab>();
        Debug.Log("looking for slabs : " + slabContainer.childCount + " objects found");
        foreach (Transform child in slabContainer)
        {
            Slab slabComp = child.GetComponent<Slab>();
            if(slabComp != null)
            {
                uncheckedSlabs.Add(slabComp);
                Debug.Log(slabComp + " found, adding it to the list");
            }
            else
            {
                Debug.Log(" no slab comp found for this object");
            }
        }

        return uncheckedSlabs.Count;
    }

    // map cards on all of the slabs in the given countainer
    public void CreateCards()
    {
        // get slabs in container
        InitSlabContainer();
        
        while(uncheckedSlabs.Count > 0)
        {
            StartNewCircuit();
        }

    }

    public void StartNewCircuit()
    {

        // get a specific slab
        int index = Random.Range(0, uncheckedSlabs.Count);
        Slab slabComp = uncheckedSlabs[index];
        if (slabComp == null)
        {
            Debug.LogWarning("start new circuit : no slab found for index " + index);
            return;
        }

        Debug.Log("Start a new circuit ! Chosen slab : " + slabComp);

        Propagate(slabComp.gameObject, true);



        //// remove it as it is "checked"
        //uncheckedSlabs.RemoveAt(index);

        //bool[] edges = new bool[4];
        //for(int edge = 0; edge <4;edge++)
        //{
        //    edges[edge] = (Random.Range(0f, 1f) < averageBranching);
        //}

        //// instanciate appropriate card
        //int rotation = 0;
        //CircuitCard.CircuitType type = CircuitCard.CircuitType.Empty;
        //CircuitCard.EdgesToType(edges,out type, out rotation);


        //GameObject newCard = LevelInitializer.instance.CreateCard(slabComp.GetSlabType(), slabComp.transform, "circuit", rotation, type);

        //// apply changes to the concerned neighbors
        //// and propagate circuit through free neighbors, removing them from the unchecked list
        //slabComp.InitNeighborhood();

        //foreach(GameObject neighborSlab in slabComp.neighborhood){
        //    Slab neighborSlabComp = neighborSlab.GetComponent<Slab>();
        //    if (neighborSlabComp != null)
        //    {
        //        Propagate(neighborSlab);
        //    }
        //}
    }

    // update the circuit to connect the slab to its neighbors then propagate the circuit
    // the game obj already has a Slab comp and a CircuitCard comp
    // if hard propagate is false, adjacente slabs will just be updated according to the current new state
    // if hard propagate is true, it will also propagate from the new bound slabs
    
    // Do the hard propagations at the end pf the current one to avoid conflicts
    private void Propagate(GameObject slabObj, bool hardPropagation = false)
    {
        bool[] edges = new bool[4];

        List<GameObject> objectsToHardPropagate = new List<GameObject>(); // list the object where we will hard propagate
        List<GameObject> objectsToSoftPropagate = new List<GameObject>(); // list the object where we will soft propagate

        Slab slabComp = slabObj.GetComponent<Slab>();
        if (slabComp == null)
        {
            Debug.LogError(gameObject + ": this object is supposed to have a slab component to add a CircuitCard component");
            return;
        }

        Debug.Log("start propagation on " + slabComp);

        bool checkRemove = uncheckedSlabs.Remove(slabComp);
        if (!checkRemove)
        {
            Debug.Log(uncheckedSlabs + ": can't find this item in the list. It was already removed");
        }

        // we check all the edges
        for (int edge = 0; edge < 4; edge++)
        {
            Debug.Log(slabComp + ": checking edge " + edge);

            // get the card in the direction of the edge
            Slab.Direction usedDirection; // to store the used direction
            Slab neighborSlab = slabComp.GetNeighborForEdge(edge, out usedDirection);

            if (neighborSlab != null) // if there is already a card
            {
                Debug.Log(slabComp + ": slab found for edge " + edge+ ": "+neighborSlab + "; used direction: "+usedDirection);

                Card neighborCard = neighborSlab.card;

                if(neighborCard != null) // if there is an attached card
                {
                    CircuitCard neighborCircuitCard = neighborSlab.card.GetComponent<CircuitCard>();
                    if (neighborCircuitCard != null) // if this card is a circuit card
                    {
                        if (neighborCircuitCard.HasCircuitAtEdge(usedDirection.edge)) // if the neighbor slab already has an edge to connect to the current one
                        {
                            Debug.Log(slabComp + ": edge " + edge + ": the neighbor slab has already a connection for this edge");
                            edges[edge] = true;
                        }
                        else // if the neighbor slab doesn't have a connection with the current one
                        {
                            // flip a coin to soft-propagate the circuit
                            float score = Random.Range(0f, 1f);
                            if (score < ReBranching) // if the random value is low enough we create a new connection
                            {
                                Debug.Log(slabComp + ": edge " + edge + ": there is already a circuit card and we connect it (score = " + score + ")");
                                edges[edge] = true; // we mark the current slab for this direction

                                // we add it to the list to propage after the instanciation of the current card
                                objectsToSoftPropagate.Add(neighborSlab.gameObject);
                            }
                            else // if random value is too hiegh, we don't connect this edge
                            {
                                Debug.Log(slabComp + ": edge " + edge + ": there is already a circuit card but we don't connect it (score = " + score + ")");
                                edges[edge] = false;
                            }
                        }
                    }
                    else // if there is already a card but its type is not Circuit
                    {
                        Debug.Log(slabComp + ": edge " + edge + ": there is already a non-circuit card");
                        edges[edge] = false; // we can't connect it to the circuit
                    }
                }
                else if(hardPropagation) // if there is no card yet but we are in hardPropagation mode
                {
                    Debug.Log(slabComp + ": there is no card in the direction " + edge +"; flip to create one...");

                    // flip the coin to connect !
                    float score = Random.Range(0f, 1f);
                    if (score < firstBranching)
                    {
                        Debug.Log(score + " is high enough to create a new connection: gonna propagate");
                        edges[edge] = true;

                        // we add it to the list to propage after the instanciation of the current card
                        objectsToHardPropagate.Add(neighborSlab.gameObject); 
                    }
                    else // the score is too low, we don't create a new card for the slab at this edge and we don't create a connection either
                    {

                        Debug.Log(score +" is too low to create a new connection" );
                        edges[edge] = false;
                    }
                }
                else // if we are in soft-propagation mode, we simply update of slab, we don't add any connection
                {
                    Debug.Log(slabComp + ": edge "+edge+": no new connection in soft-propagation mode");
                    edges[edge] = false;
                }
            }
            else // if slab is null, do nothing for this edge
            {
                Debug.Log(slabComp + ": no slab found for edge " + edge);
                edges[edge] = false;
            }
        } // end of edge checking

        // destroy the previous card if there is
        if(slabComp.card != null)
        {
            Debug.Log(slabComp + ": destroy the attached card (" + slabComp.card + ") to create an updated one");
            Destroy(slabComp.card.gameObject);
        }

        // instanciate the proper card
        int rotation = 0;
        CircuitCard.CircuitType type = CircuitCard.CircuitType.Empty;
        CircuitCard.EdgesToType(edges,out type, out rotation);
        
        GameObject newCard = LevelInitializer.instance.CreateCard(slabComp.GetSlabType(), slabComp.transform, "circuit", rotation, type);
        //Debug.Log(slabComp + ": instanciated new card: " + newCard.GetComponent<Card>());
        Debug.Log(slabComp + ": instanciated object: " + newCard + "; is object null : " + (newCard == null));
        Debug.Log(slabComp + ": shape of the card: " + printEdges(edges) + ": corresponding type: " + type);



        // hardpropagate needed slab
        foreach (GameObject go in objectsToHardPropagate)
        {
            Propagate(go, true);
        }

        // softpropagate needed slab
        foreach (GameObject go in objectsToSoftPropagate)
        {
            Propagate(go,false);
        }
    }

    public int ShuffleBoard()
    {
        int count = 0;

        foreach (RotativeSlab rotSlab in FindObjectsOfType<RotativeSlab>())
        {
            if (rotSlab.RotateCardRandom())
            {
                count++;
            }
        }

        foreach (PermutableSlab permSlab in FindObjectsOfType<PermutableSlab>())
        {
            if (permSlab.PermuteRandom())
            {
                count++;
            }
        }
        Debug.LogWarning(this + ": shuffle " + count + " cards");
        return count;
    }
    

    public void PlaceStaticCard()
    {
        // get a specific slab
        int index = Random.Range(0, uncheckedSlabs.Count);
        Slab slabComp = uncheckedSlabs[index];

        // remove it as it is "checked"
        uncheckedSlabs.RemoveAt(index);

        // instantation
        GameObject newObj = Instantiate(_levelInitializer.objectPrefabs.StaticCardPrefab, slabComp.transform);
        Card cardComp = newObj.GetComponent<Card>();
        cardComp.id = Card.GetNewId();

        // binding
        cardComp.supportSlab = slabComp;
        slabComp.card = cardComp;
        

    }


    private static string printEdges(bool[] edges)
    {
        string str = "[" + (edges[0] ? "1" : "0") + "," + (edges[1] ? "1" : "0") + "," + (edges[2] ? "1" : "0") + "," + (edges[3] ? "1" : "0") + "]";
        return str;
    }

  
}
