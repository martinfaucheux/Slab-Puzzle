using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit
{
    // false if there is an unconnected card in the circuit
    bool isValid = true;

    // id of the circuit
    public int id;

    // last attributed id
    static int maxId = 0;

    List<CircuitCard> cardList;



    // default constructor
    public Circuit()
    {
        maxId++;
        this.id = maxId;
        isValid = true;
        cardList = new List<CircuitCard>();
    }

    // build a circuit given an initial card
    public Circuit(CircuitCard card) {

        maxId++;
        this.id = maxId;

        isValid = true;
        cardList = new List<CircuitCard>() { card };
        foreach (CircuitCard neighbourCard in card.connectedNeighbours)
        {
            isValid = isValid && this.Add(neighbourCard);
        }

        // update activation of the circuit
        this.Activate(isValid);

        Debug.Log("new circuit created; id: " + id + "; size: " + cardList.Count + "; isValid: " + isValid);
    }

    // add a card to the circuit
    // return true if the circuit is valid
    public bool Add(CircuitCard card)
    {
        Debug.Log("circuit " + id + ": adding card " + card.id);

        bool res = card.isConnected;

        // if the card is not connected we un-valid the circuit
        if (!res)
        {
            // if we add an unconnected card to the circuit then it is not valid
            Debug.Log("card " + card.id + ": unconnected");
            isValid = false;
        }

        // if the card is not already in a circuit (it should be the case
        if (card.boundCircuit == null)
        {
            Debug.Log("card " + card.id + " successfully added to circuit " + id + ". Now adding its connected neighbors");
            cardList.Add(card);
            card.boundCircuit = this;
            foreach(CircuitCard neighbourCard in card.connectedNeighbours)
            {
                this.Add(neighbourCard);
            }
        }
        else if( card.boundCircuit != this)
        {
            Debug.LogWarning("Error : card "+card.id+" is already in circuit "+card.boundCircuit.id);
            //card.SetColorToError();
        }
        else if (card.isActivated)
        {
            Debug.LogWarning("Error : card "+card.id+" is already activated");
            //card.SetColorToError();
        }

        return isValid;
    }

    // activate every card of the circuit
    public void Activate(bool activation = true)
    {
        foreach(CircuitCard card in cardList)
        {
            card.SetActive(activation);
        }
    }

    // build all the circuits
    // return true if all the circuit are valid
    // ie all the cards are activated
    public static bool BuildCircuits()
    {
        Debug.Log("building circuits");
        bool res = true;

        foreach (GameObject card in GameManager.instance.CardList)
        {
            // get next non-checked circuit card
            CircuitCard circuitCard = card.GetComponent<CircuitCard>();
            if(circuitCard != null && circuitCard.boundCircuit == null)
            {
                Debug.Log("new circut id: " + (maxId+1));
                Circuit newCircuit = new Circuit();



                newCircuit.Add(circuitCard);
                newCircuit.Activate(newCircuit.isValid);
                res = res && newCircuit.isValid;

                //if (circuitCard.isConnected) // if the card is connected
                //{
                //    // we choose an unchecked card
                //    if(circuitCard.boundCircuit == null)
                //    {
                //        Circuit newCircuit = new Circuit(circuitCard);
                //        res = res && newCircuit.isValid;
                //    }
                //}
                //else // if not connected
                //{
                //    // desactivate the card 
                //    circuitCard.SetActive(false);

                //    // all circuits are not valid
                //    res = false;

                //}
            }
        }
        return res;
    }

    // reset circuit counter
    public static void ResetCircuits()
    {
        maxId = 0;
    }
}
