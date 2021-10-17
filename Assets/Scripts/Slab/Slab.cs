using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Slab : MonoBehaviour {

    public int id = 0;

    public Card card;
    public List<GameObject> neighborhood;
    public int orientation;                     // 0, 1, 2 for normal along x, y, z 
    public bool isInteractable = true;

    public static float size = 2;

    private static int _maxId = 0;


    // used for the neighbors
    public class Direction
    {
        public int orientation;
        public int edge;
        public Vector3 direction;

        public Direction(int orientation, int edge, Vector3 direction)
        {
            this.orientation = orientation;
            this.edge = edge;
            this.direction = direction;
        }

        public Direction() : this(0, 0, Vector3.zero) { }

        public override string ToString(){
            return "Direction (normal = "+orientation+"; edge = "+edge+"; direction = "+direction;
        }

    }

    private Collider _collider;
    private int layerMask = 1 << 8;

    public static int GetNewId()
    {
        _maxId++;
        return _maxId;
    }

    public override string ToString()
    {
        return "slab "+id;
    }

	// Use this for initialization
	public virtual void Start () {
        _collider = GetComponent<Collider>();

        if (_collider == null)
            Debug.LogError(gameObject + ": can't find a collider component");

        //InitNeighborhood();
    }

    // function that apply the corresponding movement according to the type of the slab
    // return true if this initated a movement
    // false if not (e.g. unselection of permutable slab)
    public virtual bool OnTouch()
    {
        Debug.Log(gameObject + " : OnTouch()");

        // we dis-highligth any highlighted PermutableSlab if we use the OnTouch method for other type of slab
        PermutableSlab[] permutableSlabList = GameObject.FindObjectsOfType<PermutableSlab>();
        foreach(PermutableSlab elt in permutableSlabList)
        {
            elt.isSelected = false;
        }
        return false;
    }

    public void InitNeighborhood()
    {
        List<GameObject> neighborhood = new List<GameObject>();
        List<Vector3> directionList = GetPossibleNeighborhoodDirections();
        foreach (Vector3 dirVector in directionList)
        {
            GameObject objInDirection = GetSlabInDirection(dirVector);
            if (objInDirection != null && objInDirection.GetComponent<Slab>() != null && !neighborhood.Contains(objInDirection))
            {
                neighborhood.Add(objInDirection);
                // Debug.Log(this+": added to neighborhood: " + objInDirection.GetComponent<Slab>() + " for direction "+dirVector);
            }
        }
        this.neighborhood = neighborhood;
    }

    public List<Vector3> GetPossibleNeighborhoodDirections()
    {
        Debug.Log(gameObject + ": constructing Possible Neighborhood Direction");
        List<Vector3> directionList = new List<Vector3>();
        Direction[] tmpDirections;
        int edge;

        for (edge = 0; edge < 4; edge++)
        {
            tmpDirections = DirectionForEdge(edge);
            directionList.Add(tmpDirections[0].direction);
            directionList.Add(tmpDirections[1].direction);
        }

        return directionList;
    }

    // return the adjacent slab in the given direction
    // if there is no slab, return null
    public Slab GetNeighborForEdge(int edge, out Slab.Direction usedDirection)
    {
        //Debug.Log(gameObject + ": check edge connection for edge " + edge);
        Slab tmpSlab = null;
        Slab.Direction[] dirVect = DirectionForEdge(edge);
        usedDirection = null;

        // check the slab in the first direction
        GameObject objInDirection = GetSlabInDirection(dirVect[0].direction);
        if (objInDirection != null)
        {
            tmpSlab = objInDirection.GetComponent<Slab>();

            // check if the detected object is a slab
            if (tmpSlab != null)
            {
                usedDirection = dirVect[0]; // update usedDirection
                // Debug.Log(this + ": found correct slab in the direction " + edge + "; direction[0]: " + usedDirection);
            }
        }

        // if we didn't find any slab, check the second possible location
        if (tmpSlab == null)
        {
            objInDirection = GetSlabInDirection(dirVect[1].direction);
            if (objInDirection != null)
            {
                tmpSlab = objInDirection.GetComponent<Slab>();

                if (tmpSlab != null)
                {
                    usedDirection = dirVect[1];
                    // Debug.Log(this + ": found correct slab in the direction " + edge + "; direction[1]: " + usedDirection);
                }
            }
        }

        // if we still havn't find any usable circuitCard
        if (tmpSlab == null)
        {
            // Debug.Log(this + ": no correct slab in the direction " + edge);
            usedDirection = null;
            return null;
        }

        //bool res = adjacentCircuitCard.HasCircuitAtEdge(usedDirection.edge);

        //if (res)// add the card to the adjacent list
        //    connectedNeighbours.Add(adjacentCircuitCard);

        // Debug.Log(this + ": neighbor found for edge " + edge + ": " + tmpSlab.ToString());

        return tmpSlab;
    }

    //// check if there is a valid slab to connect with adjacent to the given edge
    //// if yes return the adjacent CircuitCard comp
    //// if not return null
    //public CircuitCard GetNeighborForEdge(int edge, out Slab.Direction usedDirection)
    //{
    //    //Debug.Log(gameObject + ": check edge connection for edge " + edge);
    //    Slab tmpSlab = null;
    //    CircuitCard adjacentCircuitCard = null;
    //    Slab.Direction[] dirVect = DirectionForEdge(edge);
    //    usedDirection = null;

    //    // check the slab in the first direction
    //    GameObject objInDirection = GetSlabInDirection(dirVect[0].direction);
    //    if (objInDirection != null)
    //    {
    //        tmpSlab = objInDirection.GetComponent<Slab>();

    //        // check if the detected object is a circuitCard
    //        if (tmpSlab != null && tmpSlab.card.GetComponent<CircuitCard>() != null)
    //        {
    //            adjacentCircuitCard = tmpSlab.card.GetComponent<CircuitCard>();
    //            usedDirection = dirVect[0]; // update usedDirection
    //            //Debug.Log(gameObject + ": found correct slab in the direction " + edge + "; direction[0]: " + usedDirection);
    //        }
    //    }

    //    // if we didn't find an appropriate circuit Card, check the second possible location
    //    if (adjacentCircuitCard == null)
    //    {
    //        objInDirection = GetSlabInDirection(dirVect[1].direction);
    //        if (objInDirection != null)
    //        {
    //            tmpSlab = objInDirection.GetComponent<Slab>();

    //            if (tmpSlab != null && tmpSlab.card.GetComponent<CircuitCard>() != null)
    //            {
    //                adjacentCircuitCard = tmpSlab.card.GetComponent<CircuitCard>();
    //                usedDirection = dirVect[1];
    //                //Debug.Log(gameObject + ": found correct slab in the direction " + edge + "; direction[1]: " + usedDirection);
    //            }
    //        }
    //    }

    //    // if we still havn't find any usable circuitCard
    //    if (adjacentCircuitCard == null)
    //    {
    //        //Debug.Log(gameObject + ": no correct slab in the direction " + edge);
    //        usedDirection = null;
    //        return null;
    //    }

    //    //bool res = adjacentCircuitCard.HasCircuitAtEdge(usedDirection.edge);

    //    //if (res)// add the card to the adjacent list
    //    //    connectedNeighbours.Add(adjacentCircuitCard);

    //    return adjacentCircuitCard;
    //}

    // DEPRECATED
    public List<Vector3> GetPossibleNeighborhoodDirections_old()
    {
        Debug.Log(gameObject + ": constructing Possible Neighborhood Direction");
        List<Vector3> directionList = new List<Vector3>();
        switch (orientation)
        {
            // for a normal in x
            case 0:
                directionList.Add(new Vector3(0f, 0f, 1f));
                directionList.Add(new Vector3(0f, 0f, -1f));
                directionList.Add(new Vector3(0f, 1f, 0f));
                directionList.Add(new Vector3(0f, -1f, 0f));

                directionList.Add(new Vector3(1f, 0f, -1f));
                directionList.Add(new Vector3(-1f, 0f, 1f));
                directionList.Add(new Vector3(1f, -1f, 0f));
                directionList.Add(new Vector3(-1f, 1f, 0f));
                break;

            // for a normal in y
            case 1:
                directionList.Add(new Vector3(1f, 0f, 0f));
                directionList.Add(new Vector3(-1f, 0f, 0f));
                directionList.Add(new Vector3(0f, 0f, 1f));
                directionList.Add(new Vector3(0f, 0f, -1f));

                directionList.Add(new Vector3(0f, 1f, -1f));
                directionList.Add(new Vector3(0f, -1f, 1f));
                directionList.Add(new Vector3(1f, -1f, 0f));
                directionList.Add(new Vector3(-1f, 1f, 0f));
                break;

            // for a normal in z
            case 2:
                directionList.Add(new Vector3(1f, 0f, 0f));
                directionList.Add(new Vector3(-1f, 0f, 0f));
                directionList.Add(new Vector3(0f, -1f, 0f));
                directionList.Add(new Vector3(0f, -1f, 0f));

                directionList.Add(new Vector3(0f, -1f, 1f));
                directionList.Add(new Vector3(0f, 1f, -1f));
                directionList.Add(new Vector3(-1f, 0f, 1f));
                directionList.Add(new Vector3(1f, 0f, -1f));
                break;
        }

        return directionList;
    }

    public GameObject GetSlabInDirection(Vector3 direction)
    {
        // Debug.Log(this + ": throwing a ray in direction: " + direction);
        GameObject slabHit = null;
        float hitDistance = size * 3/2; // size * 3 / 4;
        RaycastHit hit;

        //_collider.enabled = false;
        bool isHit = Physics.Raycast(transform.position, direction, out hit, hitDistance, layerMask);
        //_collider.enabled = true;

        Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.red, 5);

        if (isHit)
        {
            GameObject collidedObj = hit.collider.gameObject;
            // Debug.Log(this + ": has hit: " + collidedObj + " in direction " + direction);
            if (collidedObj.GetComponent<Slab>() != null)
            {
                slabHit = collidedObj;
            }
        }
        return slabHit;
    }

    public Direction[] DirectionForEdge(int edge)
    {
        // DirectionVect renseigne sur les deux slab voisines possibles pour l'edge donné
        Direction[] directionVect = new Direction[2];
        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 0f, 1f));

        switch (orientation)
        {
            // normal in x
            case 0:
                switch (edge)
                {
                    case 0:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 0f, 1f));
                        directionVect[1] = new Direction(2, 1 , new Vector3(-1f, 0f, 1f));
                        break;
                    case 1:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 1f, 0f));
                        directionVect[1] = new Direction(1,0, new Vector3(-1f, 1f, 0f));
                        break;
                    case 2:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 0f, -1f));
                        directionVect[1] = new Direction(2, 3, new Vector3(1f, 0f, -1f));
                        break;
                    case 3:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, -1f, 0f));
                        directionVect[1] = new Direction(1, 2, new Vector3(1f, -1f, 0f));
                        break;
                }
                break;
           
            // normal in y
            case 1:
                switch (edge)
                {
                    case 0:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(1f, 0f, 0f));
                        directionVect[1] = new Direction (0, 1, new Vector3(1f, -1f, 0f));
                        break;
                    case 1:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 0f, 1f));
                        directionVect[1] = new Direction(2, 0, new Vector3(0f, -1f, 1f));
                        break;
                    case 2:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(-1f, 0f, 0f));
                        directionVect[1] = new Direction(0, 3, new Vector3(-1f, 1f, 0f));
                        break;
                    case 3:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 0f, -1f));
                        directionVect[1] = new Direction(2, 2, new Vector3(0f, 1f, -1f));
                        break;
                }
                break;
            
            // normal in z
            case 2:
                switch (edge)
                {
                    case 0:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, 1f, 0f));
                        directionVect[1] = new Direction(1, 1, new Vector3(0f, 1f, -1f));
                        break;
                    case 1:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(1f, 0f, 0f));
                        directionVect[1] = new Direction(0, 0, new Vector3(1f, 0f, -1f));
                        break;
                    case 2:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(0f, -1f, 0f));
                        directionVect[1] = new Direction(1, 3, new Vector3(0f, -1f, 1f));
                        break;
                    case 3:
                        directionVect[0] = new Direction(orientation, (edge + 2) % 4, new Vector3(-1f, 0f, 0f));
                        directionVect[1] = new Direction( 0, 2, new Vector3(-1f, 0f, 1f));
                        break;
                }
                break;
        }
        // Debug.Log(this + ": Direction for edge(" + edge + "): " + "(" + directionVect[0] + ", " + directionVect[1] + ")");
        return directionVect;
    }


    public string GetSlabType()
    {
        if (GetComponent<PermutableSlab>() != null)
        {
            return "permutable";
        }
        else if (GetComponent<RotativeSlab>() != null)
        {
            return "rotative";
        }
        else if (GetComponent<StaticSlab>() != null)
        {
            return "static";
        }
        else
        {
            Debug.LogError(gameObject + ": no particular slabtype found, return static type");
            return "static";
        }
    }
}
