using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCubeColor : MonoBehaviour
{
    public float colorMultiplier = 0.5f;

    //private Color customGreen = new Color(25 / 255f, 150 / 255f, 97 / 255f);
    private Color defaultColor;
    private bool isTriggered = false;
    private Renderer rend;

    // Use this for initialization
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        Debug.Log("cube renderer : " + rend);
        defaultColor = rend.material.color;
        Debug.Log("default color : " + defaultColor);
    }

    // When the mouse goes on the object
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // change material
            //ChangeColor(isTriggered ? defaultColor : customGreen);

            if (!isTriggered)
            {
                SetClearer();
            }
            else
            {

            }
            isTriggered = isTriggered ? false : true;
            
            Debug.Log(gameObject + " : color set via mouse : " + rend.material.color);
        }
    }

    private void ChangeColor(Color color)
    {
        
        rend.material.color = color;
        //rend.material.shader = Shader.Find("Specular");
    }

    private void SetClearer()
    {
        Color newColor = defaultColor /*+ new Color(1f,1f,1f) */ * colorMultiplier;
        Debug.Log(gameObject + " : newly created color : " + newColor);
        rend = GetComponent<Renderer>();
        rend.material.color = newColor;
    }
}
