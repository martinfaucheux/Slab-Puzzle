using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotSlabComponent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slab slab = GetComponent<Slab>();
            string tmp = (slab == null) ? "" : "not ";
            Debug.Log(gameObject + "; slab is " + tmp + "null: " + slab);
        }
    }

}
