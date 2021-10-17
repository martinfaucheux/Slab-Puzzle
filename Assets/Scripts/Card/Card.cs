using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {

    // id of the card
    public int id;





    // true when the card is activated
    // to win the level, every card must be activated
    public bool isActivated;

    public Slab supportSlab;

    public int rotation;                                         // rotation of the card on its slab

    private static int _maxId = 0;


    //public static float hilightColorMultiplier = 1.5f;              // multiplier to apply to the color to display the "highlighted" state
    //public static float selectColorMultiplier = 2f;                 // multiplier to apply to the color to display the "selected" state
    //[HideInInspector] public Material selectedCardMaterial;
    //[HideInInspector] public Material unselectedCardMaterial;
    //public Renderer externalChildRenderer;

    [System.Serializable]
    public class RendererControler
    {
        public static float hilightColorMultiplier = 1.5f;              // multiplier to apply to the color to display the "highlighted" state
        public static float selectColorMultiplier = 2f;                 // multiplier to apply to the color to display the "selected" state[HideInInspector] public Material selectedCardMaterial;

        //[HideInInspector] public Material selectedCardMaterial;
        //[HideInInspector] public Material unselectedCardMaterial;

        public Renderer externalChildRenderer;

        public Color errorColor;
        
    }
    public RendererControler rendererControler;



    private Renderer _renderer;
    private Color _defaultColor;
    private Color _selectColor;
    private Color _highlightColor;


    // USELESS
    // function applied to check if the card is activated
    public abstract bool CheckActivation();

    public override string ToString()
    {
        return "card " + id;
    }

    public static int GetNewId()
    {
        _maxId++;
        return _maxId;
    }

    // set the activation to the given state
    // if there is no changing, do nothing
    public void SetActive(bool activate = true)
    {
        if(isActivated != activate)
        {
            isActivated = activate;
            StartCoroutine(ChangeIllumination(activate));
        }

    }

    // Use this for initialization
    public virtual void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            if (_renderer == null)
                _renderer = rendererControler.externalChildRenderer;
        }
        InitColors();

        // add to card vect
        GameManager.instance.CardList.Add(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.instance.CardList.Remove(this.gameObject);
    }

    // set the color of the card to "highlight"
    public void SetHighlightColor()
    {
        _renderer.material.color = _highlightColor;
        //Debug.Log(gameObject + " : card color change to highlighted; new color : "+ thisRenderer.material.color);
    }

    // set the color of the card to "selected"
    public void SetSelectColor()
    {
        _renderer.material.color = _selectColor;
        //Debug.Log(gameObject + " : card color change to highlighted; new color : " + thisRenderer.material.color);
    }

    // set the color of the card to default
    public void SetDefaultColor()
    {
        _renderer.material.color = _defaultColor;
        //Debug.Log(gameObject + " : card color change to unhighlighted; new color : " + thisRenderer.material.color);
    }

    // initialise the different colors at the beginning
    public void InitColors()
    {
        _defaultColor = _renderer.material.color;
        _highlightColor = _defaultColor * RendererControler.hilightColorMultiplier;
        _selectColor = _defaultColor * RendererControler.selectColorMultiplier;
    }


    //// DEPRECATED

    //// change the material to indicate that the card is selected
    //public void SelectCard()
    //{
    //    Debug.Log(gameObject + " selected");
    //    _renderer.material = rendererControler.selectedCardMaterial;
    //}

    //// change the material to indicate that the card is not selected
    //public void UnselectCard()
    //{
    //    Debug.Log(gameObject + " unselected");
    //    _renderer.material = rendererControler.unselectedCardMaterial;
    //}

    // if true, illulinate the card (change emission coefficient)
    // if false, set back emission to 0
    public IEnumerator ChangeIllumination(bool illuminate = true, float transitionTime = 1f)
    {
        Color minEmissionColor = Color.black;
        Color maxEmissionColor = Color.white * 1.57f; // possible via HDR

        //Color startColor = _renderer.material.GetColor("_EmissionColor");
        Color startColor = _renderer.material.GetColor("_EmissionColor");

        Color targetColor = (illuminate ? maxEmissionColor : minEmissionColor);
        float startTime = 0f;
        
        while(startTime < transitionTime)
        {
            startTime += Time.deltaTime;
            //_renderer.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, startTime / transitionTime));
            _renderer.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, startTime / transitionTime));
            yield return null;
        }

        _renderer.material.SetColor("_EmissionColor", targetColor);
    }

    public void SetColorToError()
    {
        _renderer.material.color = rendererControler.errorColor;
    }

    public void ChangeRendererLayer(int layer)
    {
        _renderer.gameObject.layer = layer;
    }

    public string GetStringType()
    {
        if (GetComponent<CircuitCard>() != null)
        {
            return "circuit";
        }
        else if (GetComponent<StaticCard>() != null)
        {
            return "static";
        }
        else
        {
            Debug.LogError(gameObject + ": no particular slabtype found, return static type");
            return "static";
        }
    }
}
