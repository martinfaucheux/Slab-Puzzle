using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlabCreator : MonoBehaviour {

    
    public LevelFace[] FaceVector;

    public Transform slabContainer;


    private LevelInitializer _levelInitializer; // usefull to get the prefabs



    private void Start()
    {
        if(slabContainer == null)
        {
            Debug.LogWarning("no slab container found, creating one");
            GameObject newGO = new GameObject("GeneratedSlabs");
            slabContainer = newGO.transform;
        }

        _levelInitializer = LevelInitializer.instance;
        if (_levelInitializer == null)
            Debug.LogError("error : level initializer not found");
    }

    public void GenerateRectFaces()
    {
        foreach(LevelFace face in FaceVector)
        {
            GenerateRectFace(face);
        }
    }


    public void GenerateRectFace(LevelFace levelFace, int seed = 0)
    {
        if (levelFace.spreadingType != LevelFace.SpreadingType.Rect)
        {
            Debug.LogError(gameObject + ": can't generate non Rect level face with this method");
            return;
        }

        Random.InitState(seed == 0 ? (int) Time.time : seed); // init random with a seed, or go random if nothing provided

        // getting parameters
        LevelFace.FaceDimension dimension = levelFace.dimension;
        LevelFace.SlabProbabilities probabilities = levelFace.slabProbabilities;

        // overriding slabNumber with dimension values
        levelFace.slabNumber = (dimension.Xmax - dimension.Xmin) * (dimension.Ymax - dimension.Ymin);
        Debug.LogWarning(gameObject + ": overriding slabNumber with dimension values: creating " + levelFace.slabNumber + " slabs");

        // first position to create a slab
        Vector3 firstPosition = new Vector3();

        int u = (levelFace.normal + 2) % 3;
        int v = (levelFace.normal + 1) % 3;

        firstPosition[levelFace.normal] = levelFace.normalValue;
        firstPosition[u] = (-0.5f * (dimension.Xmax - dimension.Xmin) + 0.5f) * Slab.size;
        firstPosition[v] = (-0.5f * (dimension.Ymax - dimension.Ymin) + 0.5f) * Slab.size;


        // for each slab...
        for (int x = dimension.Xmin; x < dimension.Xmax; x++)
        {
            for (int y = dimension.Ymin; y < dimension.Ymax; y++)
            {
                GameObject slabModel = null;
                string modelType = levelFace.GetRandomSlabType();

                // get a random slab type
                switch (modelType)
                {
                    case "permutable":
                        slabModel = _levelInitializer.objectPrefabs.PermutableSlabPrefab;
                        break;
                    case "rotative":
                        slabModel = _levelInitializer.objectPrefabs.RotativeSlabPrefab;
                        break;
                    case "static":
                        slabModel = _levelInitializer.objectPrefabs.StaticSlabPrefab;
                        break;
                    default:
                        Debug.LogError("non critic error : unexpected slab type given by level face object");
                        slabModel = _levelInitializer.objectPrefabs.StaticSlabPrefab;
                        break;
                }

                // in case of unexpected slab type
                if (slabModel == false)
                {
                    Debug.LogError("no model found after switch slabType");
                    return;
                }

                // determines the position for the new slab
                Vector3 newPosition = firstPosition;
                newPosition[u] += x * Slab.size;
                newPosition[v] += y * Slab.size;
                
                // actual instantiation
                GameObject newSlabObj = Instantiate(slabModel, slabContainer.position + newPosition, Quaternion.identity);
                newSlabObj.transform.parent = slabContainer;
                Slab newSlabComp = newSlabObj.GetComponent<Slab>();
                newSlabComp.orientation = levelFace.normal;



                newSlabComp.id = Slab.GetNewId();

                switch (levelFace.normal)
                {
                    case 0:
                        newSlabObj.transform.Rotate(-90f, 0f, -90f);
                        break;
                    case 1: // already well oriented
                        break;
                    case 2:
                        newSlabObj.transform.Rotate(0f, 90f, 90f);
                        break;
                    default:
                        Debug.LogError("Error on instanciation : unkown orientation " + levelFace.normal);
                        return ;
                }

                // add it to the created slabs of the levelFace
                levelFace.createdFace.Add(newSlabObj);
            }
        }        
    }

    public void DeleteSlabs()
    {
        foreach(Transform elt in slabContainer)
        {
            Slab slab = elt.GetComponent<Slab>();
            if (slab != null)
            {
                Destroy(elt.gameObject);
            }
        }
    }




}
