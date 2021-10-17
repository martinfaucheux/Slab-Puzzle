using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour {

    public float movementAmplitude = 0.7f;
    public float movementSpeed = 0.5f;



    public bool isFloating = true;

    private float _cycleTime = 0f;
    private Vector3 _initPos;

	// Use this for initialization
	void Start () {
        _initPos = transform.position;
        _cycleTime = Random.Range(0f, 2 * Mathf.PI);
	}


    private void Update()
    {
        if (isFloating)
        {
            transform.position = _initPos + movementAmplitude * Mathf.Sin(_cycleTime + _cycleTime * movementAmplitude) * Vector3.up;
            _cycleTime += Time.deltaTime * movementSpeed;
        }
    }

    public void StopFloating()
    {
        isFloating = false;
        StartCoroutine(GoToNormalState());
    }

    private IEnumerator GoToNormalState()
    {
        while ((transform.position - _initPos).magnitude > 0.01)
        {
            Vector3 currentPos = transform.position;
            Vector3 movementDirection = (_initPos - currentPos).normalized;
            Vector3 newPosition = currentPos + movementDirection * movementSpeed * Time.deltaTime;

            // if any movement would increase the distance instead of decreasing it
            if ((currentPos - newPosition).magnitude > (currentPos - _initPos).magnitude)
            {
                transform.position = _initPos;
                break; // if short distance, break the loop
            }
            else
            {
                transform.position = newPosition; // if object is still far away, reduce the distance
            }

            yield return null;
        }
        transform.position = _initPos;

    }
}
