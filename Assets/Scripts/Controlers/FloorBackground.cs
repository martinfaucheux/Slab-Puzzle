using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBackground : MonoBehaviour
{
    public Material[] usableMaterial;
    public float materialTransitionTime = 5f;

    private Renderer _renderer;
    private int _maxMaterial;

    #region Singleton
    public static FloorBackground instance;
    public void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _maxMaterial = usableMaterial.Length;
    }

    public void ChangeMaterial(int id)
    {
        if (_maxMaterial == 0)
        {
            Debug.LogError(this + ": no usable material found for background");
            return;
        }

        Material newMaterial = usableMaterial[id % _maxMaterial];
        //_renderer.material = newMaterial;
        StopCoroutine("ChangeMaterialCoroutine");
        StartCoroutine(ChangeMaterialCoroutine(newMaterial));
    }

    public void ChangeMaterial()
    {
        ChangeMaterial(GameManager.instance.levelID);
    }

    private IEnumerator ChangeMaterialCoroutine(Material newMaterial)
    {
        Material initMaterial = _renderer.material;
        float startTime = 0f;

        while (startTime < materialTransitionTime)
        {
            startTime += Time.deltaTime;
            //_renderer.material.Lerp(initMaterial, newMaterial, startTime / materialTransitionTime);
            _renderer.material.color = Color.Lerp(initMaterial.color, newMaterial.color, Mathf.Clamp(startTime / materialTransitionTime,0f,1f));
            yield return null;
        }
    }
}