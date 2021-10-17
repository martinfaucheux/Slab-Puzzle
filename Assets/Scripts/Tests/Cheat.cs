using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour {

	
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A))
        {
            Reload();
        }


    }

    public void Reload()
    {
        PlayerPrefManager.ResetPlayerState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
