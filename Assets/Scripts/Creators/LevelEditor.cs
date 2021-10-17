using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour {


    public int slabSize = 2;

    // this is the "preview" of the object that the user is about to create
    public GameObject ghostObject;

    // boolean that indicates if the ghost object is visible
    [HideInInspector] public bool isGhostObjectVisible = false;

    public GameObject positionningPlane;

    private Renderer _ghostObjectRenderer;

    #region Singleton

    public static LevelEditor instance;


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
    #endregion

    private void Awake()
    {
        CheckSingleton();

    }

    private void Start()
    {
        _ghostObjectRenderer = ghostObject.GetComponent<Renderer>();
        if(_ghostObjectRenderer == null){
            Debug.LogError(this + ": Error : no renderer found for the ghost object");
        }
    }

    // set the Ghost Object invisible
    public void MakeGhostObjectInvisible()
    {
        isGhostObjectVisible = false;
        _ghostObjectRenderer.enabled = false;
    }

    // set the Ghost Object visble
    public void MakeGhostObjectVisible()
    {
        isGhostObjectVisible = true;
        _ghostObjectRenderer.enabled = true;
    }



    // change the ghost object to a slab of the specified type
    public void ChangeGhostObjectToSlab(string slabType)
    {
        MakeGhostObjectVisible();

        // save the state of the object
        Vector3 ghostPosition = ghostObject.transform.position;
        Quaternion ghostRotation = ghostObject.transform.rotation;

        // find the right model to instanciate it as the new ghostObject
        GameObject slabModel = LevelInitializer.instance.objectPrefabs.StaticSlabPrefab;


        // actual instantiation
        ghostObject = Instantiate(slabModel, ghostPosition, ghostRotation);


    }

    private Vector3 RepositionGhostObject()
    {
        Vector3 newPos = ghostObject.transform.position;

        GameObject hitGO = null;
        float _rayMaxDistance = 200f;

        // cast ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // if ray touched
        if (Physics.Raycast(ray, out hit, _rayMaxDistance, positionningPlane.layer))
        {
            hitGO = hit.collider.gameObject;
            Debug.Log(this + ": found positionning plane");
            Vector3 hitPos = hit.point;
            newPos = NearestPointOnGrid(hitPos);


        }
        //else
        //    Debug.Log(gameObject + ": casting ray; ray hit nothing");

        



        return newPos;
    }


    private static Vector3 NearestPointOnGrid(Vector3 hitPos)
    {
        Vector3 newPos = hitPos;




        return newPos;
    }
}
