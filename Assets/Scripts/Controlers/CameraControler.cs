using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{



    public float zoomRatio = 0.9f;
    public float zoomTime = 1f;

    public Camera[] slaveCameras;

    //private Camera _cameraComp;
    private float _defaultSize;
    private Vector3 _defaultPosition;

    #region Singleton

    public static CameraControler instance;
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



    // Use this for initialization
    void Start()
    {
        _defaultSize = slaveCameras[0].orthographicSize;
        _defaultPosition = slaveCameras[0].transform.parent.position;
        Debug.Log(this + ": my default position is " + _defaultPosition);
    }

    public void TriggerSmoothZooming(bool setBack = false)
    {
        StartCoroutine(SmoothZooming(setBack));
    }


    private IEnumerator SmoothZooming(bool setBack = false)
    {
        Debug.Log("start zooming coroutine");
        _defaultSize = slaveCameras[0].orthographicSize;


        float startTime = 0f;
        float targetSize = _defaultSize * zoomRatio;

        while (startTime < zoomTime)
        {
            startTime += Time.deltaTime;
            float t = (startTime / zoomTime);

            foreach (Camera camera in slaveCameras)
            {
                camera.orthographicSize = _defaultSize * (1 - t) + t * targetSize;
                yield return null;
            }
        }

        foreach (Camera camera in slaveCameras)
        {
            camera.orthographicSize = (setBack ? _defaultSize : targetSize);
            //yield return null;
        }
    }

    // DEPRECATED
    public void SetParameters(float XOffset, float YOffset, float size) // default 0f, 0f, 7f
    {
        foreach (Camera camera in slaveCameras)
        {
            Vector3 cameraTranslation = new Vector3(XOffset, YOffset, 0f);
            camera.transform.Translate(cameraTranslation);

            camera.orthographicSize = size;
        }
    }

    public void SetParameters(float XOffset, float YOffset, float size, float transitionTime) // default 0f, 0f, 7f
    {
        //StopCoroutine("SmoothZooming");
        //StopCoroutine("SmoothChange");
        StopAllCoroutines();
        StartCoroutine(SmoothChange(XOffset,YOffset,size,transitionTime));
    }

    public IEnumerator SmoothChange(float xOffset, float yOffset, float newSize, float transitionTime) // default 0f, 0f, 7f
    {
        Debug.Log(this + ": smooth changement with arguments " + xOffset + ", " + yOffset + ", " + newSize + ", " + transitionTime);
        yield return new WaitForEndOfFrame(); // wait for everything to get initialised
        float initialSize = slaveCameras[0].orthographicSize;
        Vector3 initialPosition = slaveCameras[0].transform.parent.position;
        Vector3 targetPosition = _defaultPosition + new Vector3(xOffset, yOffset, 0f);
        Debug.Log(this + ": initial size: " + initialSize);
        Debug.Log(this + ": initial position: " + initialPosition);
        Debug.Log(this + ": target position: " + targetPosition);
        Debug.Log(this + ": default position: " + _defaultPosition);
        float startTime = 0f;

        while (startTime < transitionTime)
        {
            startTime += Time.deltaTime;

            foreach (Camera camera in slaveCameras)
            {
                camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, Mathf.Clamp(startTime / transitionTime, 0f, 1f));
                camera.orthographicSize = Mathf.Lerp(initialSize, newSize, Mathf.Clamp(startTime / transitionTime, 0f, 1f));

            }

            yield return null;
        }

        Debug.Log(this + ": final position: " + slaveCameras[0].transform.position);
    }
}
