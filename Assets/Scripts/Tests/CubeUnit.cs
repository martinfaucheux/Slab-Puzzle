using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUnit : MonoBehaviour {

    [System.Serializable]
    public class Faces
    {
        public GameObject TopFace;
        public GameObject BotFace;
        public GameObject LeftFace;
        public GameObject RightFace;
        public GameObject FrontFace;
        public GameObject BackFace;
    }

    public Faces CubeFaces;

    private float size = 1;

    private Collider thisCollider;

    // Use this for initialization
    void Start () {
        thisCollider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private GameObject GetNeighbourCubeUnit(Vector3 direction)
    {
        // first we disable this object's collider
        thisCollider.enabled = false;

        Vector3 targetPosition = this.transform.position + direction.normalized * size;

        RaycastHit hit;
        bool isHit = Physics.Linecast(this.transform.position, targetPosition, out hit);

        thisCollider.enabled = true;

        if (isHit)
        {
            GameObject collidedObj = hit.collider.gameObject;
            if(collidedObj.GetComponent<CubeUnit>() != null)
            {
                return collidedObj;
            }
        }


        // on cast un ray de la taille du cube


        return null;
    }
}
