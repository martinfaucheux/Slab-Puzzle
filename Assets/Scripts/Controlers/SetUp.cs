using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : MonoBehaviour {

    public class ShaderProperties
    {
        public readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
    }

    public ShaderProperties shaderProp;

    #region Singleton

    public static SetUp instance;


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

    private void Start()
    {
        shaderProp = new ShaderProperties();

    }
}
