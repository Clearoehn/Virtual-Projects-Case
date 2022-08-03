using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gates : MonoBehaviour
{
    public GameObject player;
    private GameObject _calculationText;
    private int _calculationNumber;
    private int _calculationId;
    private int result;
    private int getNumber;

    private void Awake()
    {
        _calculationText = transform.GetChild(0).gameObject;
        _calculationNumber = (int)(Random.Range(1, 5));
        _calculationId = (int)(Random.Range(1, 4));
        switch (_calculationId)
        {
            case 1:
                _calculationText.GetComponent<TextMeshPro>().text = "+" + _calculationNumber;
                break;
            case 2:
                _calculationText.GetComponent<TextMeshPro>().text = "-" + _calculationNumber;
                break;
            case 3:
                _calculationText.GetComponent<TextMeshPro>().text = "X" + _calculationNumber;
                break;
            case 4:
                _calculationText.GetComponent<TextMeshPro>().text = "/" + _calculationNumber;
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Left Hand"))
        {
            Calculation(1);
            Destroy(GetComponent<BoxCollider>());
        }

        if (other.transform.CompareTag("Right Hand"))
        {
            Calculation(2);
            Destroy(GetComponent<BoxCollider>());
        }
    }
    
    public void Calculation(int handId)
    {
        if (handId == 1)
        {
            getNumber = player.GetComponent<Exchange>().leftCardCount;
        }
        
        if (handId == 2)
        {
            getNumber = player.GetComponent<Exchange>().rightCardCount;
        }
        
        switch (_calculationId)
        {
            case 1:
                result = getNumber + _calculationNumber;
                break;
            case 2:
                result = getNumber - _calculationNumber;
                break;
            case 3:
                result = getNumber * _calculationNumber;
                break;
            case 4:
                result = (int)(getNumber / _calculationNumber);
                break;
        }
        
        if (result < getNumber)
        {
            if(result >= 0)
            {
                if(handId == 1)
                {
                    player.GetComponent<Exchange>().leftCardCount = result;
                }
                
                if(handId == 2)
                {
                    player.GetComponent<Exchange>().rightCardCount = result;
                }
                
                player.GetComponent<Exchange>().DeleteCards(getNumber - (result), handId);
            }
            else
            {
                player.GetComponent<Exchange>().DeleteCards(getNumber, handId);
            }
        }

        if (handId == 1 && result > getNumber)
        {
            player.GetComponent<Exchange>().AddCards((result - getNumber), 1);
        }
        
        if (handId == 2 && result > getNumber)
        {
            player.GetComponent<Exchange>().AddCards((result - getNumber), 2);
        }
    }
}
