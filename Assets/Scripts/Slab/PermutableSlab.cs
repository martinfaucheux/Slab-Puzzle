using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermutableSlab : Slab
{
    // time of the transition when permuting 2 cards
    //public static float slidingTransitionTime = 0.2f;
    public static float defaultTransitionSpeed = 10f;
    public static float maxTransitionTime = 0.7f;


    public bool isSelected = false;             // true when the Slab is Selected
    public bool isHighligthed;                  // true when a PermutableSlab of the same level is selected
    public int level;                           // Determines the possible PermutableSlabs to interect with

    public static PermutableSlab currentSelectedSlab = null;            // currentSelectedSlab to be switch if we select an other eligible Slab



    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    public override bool OnTouch()
    {
        Debug.Log(card.supportSlab + ": OnTouch()");

        bool res = false;

        if (currentSelectedSlab == null)
        {
            // if nothing is select we select this one
            //Debug.Log(gameObject + " : no previous Slab selected : we select this one");
            this.Select();
        }
        else if(level != currentSelectedSlab.level || currentSelectedSlab == this)
        {
            //Debug.Log(gameObject + " : Cancel selected");
            // if we try to permute two Slabs from different level, or a slab with itself we unselect the first selected
            currentSelectedSlab.UnSelect();

            // TODO trigger error camera shift
        }
        else
        {
            //Debug.Log(gameObject + " : 2 valid slab chosen; start permutation");
            // permutation
            PermutableSlab slab1 = this;
            PermutableSlab slab2 = currentSelectedSlab;

            // we unselect the currently selected slab
            currentSelectedSlab.UnSelect();

            PermuteSlabs(slab1, slab2, true); // add true for sliding transition

            // UnSelection
            currentSelectedSlab = null;

            // change result
            res = true;
        }
        return res;
    }

    // get a random permutable slab and permute this with it
    // Return true if something actually happened
    public bool PermuteRandom()
    {
        PermutableSlab[] permutableSlabs = FindObjectsOfType<PermutableSlab>();
        int randInt = Random.Range(0, permutableSlabs.Length);
        PermutableSlab chosenSlab = permutableSlabs[randInt];

        PermuteSlabs(this, chosenSlab, false);

        return (this != chosenSlab);
    }

    public static void PermuteSlabs(PermutableSlab slab1, PermutableSlab slab2, bool slidingTransition = false)
    {
        if(slab1 == slab2) // if slab 1 & 2 are the same, we do nothing
        {
            return;
        }


        //Debug.Log("Start permutation. slab1 : " + slab1.gameObject + " ; slab2 : " + slab2.gameObject);
        Card card1 = slab1.card;
        Card card2 = slab2.card;

        // dependance permutation
        slab1.card = card2;
        slab2.card = card1;
        card1.supportSlab = slab2;
        card2.supportSlab = slab1;

        // physical permutation
        if (!slidingTransition) // for instant transition
        {
            Vector3 tmpPos = card1.transform.position;
            card1.transform.position = card2.transform.position;
            card2.transform.position = tmpPos;
        }
        else // Sliding transition
        {
            Transform targetTransform1 = card1.supportSlab.transform;
            Transform targetTransform2 = card2.supportSlab.transform;
            slab1.StartCoroutine(PermutableSlab.SlidingTransition(card1, targetTransform1,"Card_2")); // 1 goes with 1 and 2 with 2 because parent will be exchanged
            slab2.StartCoroutine(PermutableSlab.SlidingTransition(card2, targetTransform2, "Card_3"));
        }

        // parency permutation
        card1.transform.parent = slab2.transform;
        card2.transform.parent = slab1.transform;
    }

    private static IEnumerator SlidingTransition(Card card, Transform targetTransform, string layerName = "Card")
    {

        Transform cardTransform = card.transform;
        Vector3 initPosition = cardTransform.position;

        card.supportSlab.isInteractable = false;

        // change the renderer container layer so the two cards overlap correctly
        card.ChangeRendererLayer(LayerMask.NameToLayer(layerName));

        //float startTime = 0f;
        //while (startTime < slidingTransitionTime)
        //{
        //    cardTransform.position = Vector3.Lerp(initPosition, targetTransform.position, startTime / slidingTransitionTime);
        //    startTime += Time.deltaTime;
        //    yield return null;
        //}
        //card.transform.position = targetTransform.position;

        float transitionSpeed = defaultTransitionSpeed; // default speed
        float transitionDistance = (cardTransform.position - targetTransform.position).magnitude;

        // if transition time exceed the maximum, we increase the speed
        float transitionTime =  transitionDistance / defaultTransitionSpeed;
        if (transitionTime > maxTransitionTime)
        {
            //Debug.LogWarning("transition time = " + transitionTime + " over " + maxTransitionTime);
            transitionSpeed = (cardTransform.position - targetTransform.position).magnitude / maxTransitionTime; // increased speed
        }

        while ((cardTransform.position - targetTransform.position).magnitude > 0.1)
        {
            Vector3 movementDirection = (targetTransform.position - cardTransform.position).normalized;
            Vector3 newPosition = cardTransform.position + movementDirection * transitionSpeed * Time.deltaTime;

            // if any movement would increase the distance instead of decreasing it
            if((targetTransform.position - newPosition).magnitude > (cardTransform.position - targetTransform.position).magnitude)
            {
                break; // if short distance, break the loop
            }
            else
            {
                cardTransform.position = newPosition; // if object is still far away, reduce the distance
            }

            yield return null;
        }
        card.transform.position = targetTransform.position;

        card.ChangeRendererLayer(LayerMask.NameToLayer("Card"));
        card.supportSlab.isInteractable = true;

    }

    //private static IEnumerator SlidingTransition(Card card, Vector3 endPosition, string layerName = "Card")
    //{
    //    Transform cardTransform = card.transform;
    //    Vector3 initPosition = cardTransform.position;

    //    card.supportSlab.isInteractable = false;

    //    // change the renderer container layer so the two cards overlap correctly
    //    card.ChangeRendererLayer(LayerMask.NameToLayer(layerName));

    //    float startTime = 0f;
    //    while (startTime < slidingTransitionTime)
    //    {
    //        cardTransform.position = Vector3.Lerp(initPosition, endPosition, startTime / slidingTransitionTime);
    //        startTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    card.transform.position = endPosition;

    //    card.ChangeRendererLayer(LayerMask.NameToLayer("Card"));
    //    card.supportSlab.isInteractable = true;

    //}




    // Select the Slab to permute it later
    private void Select()
    {
        //Debug.Log(gameObject + " : selected");
        isSelected = true;
        PermutableSlab.currentSelectedSlab = this;

        currentSelectedSlab.card.SetSelectColor();

        HighlightLevel(level);
    }

    // Unselect the slab if OnTouch was operated on a non autorised object
    private void UnSelect()
    {
        //Debug.Log(gameObject + " : unselected");
        isSelected = false;

        currentSelectedSlab.card.SetDefaultColor();

        currentSelectedSlab = null;
        CancelHighlight();
    }

    // unselect the current selected slab if there is one
    public static void ResetSelected()
    {
        if(currentSelectedSlab != null)
        {
            currentSelectedSlab.UnSelect();
        }
    }

    // highlight the Slab to indicate that it is selectionable
    // mostly estethical
    private void Highlight()
    {
        //Debug.Log(gameObject + " : highlighted");
        isHighligthed = true;

        // Change the color
        card.SetHighlightColor();
        
        
    }

    // Highlight all the PermutableSlabs of a given level
    public static void HighlightLevel(int level)
    {
        //Debug.Log("Hilight of level " + level);
        PermutableSlab[] permutableSlabList = GameObject.FindObjectsOfType<PermutableSlab>();
        foreach (PermutableSlab elt in permutableSlabList)
        {
            if (!elt.isSelected && elt.level == level)
            {
                elt.Highlight();
            }
        }
    }

    // Revoke Highlight on all the PermutableSlabs
    public static void CancelHighlight()
    {
        PermutableSlab[] permutableSlabList = GameObject.FindObjectsOfType<PermutableSlab>();
        foreach (PermutableSlab elt in permutableSlabList)
        {
            if(elt.isHighligthed)
            {
                //Debug.Log(elt.gameObject + " : unhighlighted");
                elt.isHighligthed = false;

                // Rreset the card color
                elt.card.SetDefaultColor();
            }
        }
    }

}