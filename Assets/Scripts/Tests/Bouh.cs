using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouh : MonoBehaviour {

	public void SayBouh()
    {
        Debug.Log("booooh it's me !");

        bool gotRend = (GetComponent<Renderer>() == null);

        Debug.Log("got rend ? " + gotRend);

    }
}
