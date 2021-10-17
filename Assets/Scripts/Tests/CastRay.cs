using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastRay : MonoBehaviour {

    public Vector3 direction;
    public float length;
    public LayerMask layerMask;

    public GameObject Touch()
    {
        GameObject res = null;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.TransformDirection( direction), out hit, length,layerMask);
        Debug.DrawRay(transform.position, direction, Color.green, 5);


        if (isHit)
        {
            Debug.Log("has hit: "+hit.collider.gameObject);
            res = hit.collider.gameObject;

        }

        return res;
    }
}
