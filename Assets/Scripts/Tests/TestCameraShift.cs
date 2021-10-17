using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraShift : MonoBehaviour {

    public float amplitude = 1f;
    public float animationDuration = 2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(CameraShake(amplitude, animationDuration));
        }
	}

    public IEnumerator CameraShake(float amplitude, float animationDuration)
    {
        Vector3 initialPosition = this.transform.position;
        Vector3 newPosition;

        float k = amplitude * (1 + animationDuration) * 0.01f;
        float startTime = 0f;

        while(startTime < animationDuration)
        {
            startTime += Time.deltaTime;
            newPosition = initialPosition;
            newPosition.x += Mathf.Sin(Mathf.PI * startTime) * (k / (1 + startTime));

            this.transform.position = newPosition;

            yield return null;
            
            
        }
        this.transform.position = initialPosition;

    }
}
