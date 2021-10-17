using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSortingLayer : MonoBehaviour {

    public string layerName = "Default";
    public int sortingOrder = 0;

    private Renderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.sortingOrder = sortingOrder;
        rend.sortingLayerName = layerName;
	}
	

}
