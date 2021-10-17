using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorTest : MonoBehaviour {

    public List<Material> materialList;
    public float transitionTime = 3f;
    private int counter;
    private int maxMat;
    private bool isTriggered = false;
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        maxMat = materialList.Count;
        counter = 0;
    }

    private void OnMouseOver()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(this + ": you clicked");
            counter++;
            StopCoroutine("ChangeMaterial");
            StartCoroutine(ChangeMaterial());
        }
    }

    private IEnumerator ChangeMaterial()
    {
        Material initMaterial = _renderer.material;
        Material finalMaterial = materialList[counter % maxMat];
        float startTime = 0f;

        isTriggered = (isTriggered ? false : true);

        while (startTime < transitionTime)
        {
            startTime += Time.deltaTime;
            _renderer.material.Lerp(initMaterial, finalMaterial, Mathf.Clamp(startTime / transitionTime, 0f, 1f));
            yield return null;
        }
    }
}
