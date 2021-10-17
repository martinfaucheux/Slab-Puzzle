using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeSlab : Slab
{
    public static float rotationTime = 0.3f;


    private Transform _cardTransform;


    public override bool OnTouch()
    {
        StartCoroutine(RotateCardSmooth());
        //RotateCardInstant();
        return true;
    }

    public override void Start()
    {
        base.Start();

        if (card != null)
        {
            _cardTransform = card.transform;
        }
        else
        {
            //Debug.Log(gameObject + ": this slab has no card on");
        }
    }

    private void RotateCardInstant()
    {
        card.transform.Rotate(Vector3.up, 90f);
        card.rotation = (card.rotation + 1) % 4;
    }

    // randomly rotate the card. Return true if something actually happened
    public bool RotateCardRandom()
    {
        Random rand = new Random();
        int nRot = Random.Range(0, 4);
        for (int i = 0; i < nRot; i++)
        {
            RotateCardInstant();
        }

        return nRot != 0;
    }


    private IEnumerator RotateCardSmooth()
    {
        float startTime = 0f;
        Quaternion initRotationState = card.transform.rotation;
        Quaternion finalRotationState = card.transform.rotation * Quaternion.AngleAxis(90f, Vector3.up);
        card.rotation = (card.rotation + 1) % 4;


        // disable itereaction
        isInteractable = false;


        while (startTime < rotationTime)
        {
            startTime += Time.deltaTime;
            //_cardTransform.Rotate(Vector3.up, 90f * (startTime / rotationTime));
            float angle = 90f * (startTime / rotationTime);
            card.transform.rotation = initRotationState * Quaternion.AngleAxis(angle, Vector3.up);
            yield return null;
        }

        card.transform.rotation = finalRotationState;


        // enable interaction
        isInteractable = true;


    }
}
