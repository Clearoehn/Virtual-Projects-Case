
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Exchange : MonoBehaviour
{
    #region Definitons
    
    [Header("Objects")]
    public GameObject rightHand;
    public GameObject leftHand;
    public TextMeshPro leftCardsCountText;
    public TextMeshPro rightCardsCountText;
    public GameObject leftCardsFolder;
    public GameObject rightCardsFolder;
    [HideInInspector] public GameObject spawnedCard;
    public GameObject cardPrefab;
    private GameObject _cardTransactPos;
    public TextMeshPro finalScore;
    private int _finalScore;
    
    [Header("Variables")]
    public int totalCardCount;

    [HideInInspector] public int rightCardCount;
    [HideInInspector] public int leftCardCount;
    private int _transactionWay; // 1 For Left To Right, 2 For Right To Left, 0 For Nothing
    private bool _cardMove;
    [SerializeField] private float _InitialVelocity;
    [SerializeField] private float _Angle;
    private bool _isTouched;
    private bool _isStarted;
    private bool _isFinished;

    #endregion

    #region Default Functions

    private void Start()
    {
        StartCoroutine(AddCardsToDeck(5, 1));
    }

    private void Update()
    {
        if (leftCardCount > leftCardsFolder.transform.childCount) //It Depends to the Gates' scripts result
        {
            StartCoroutine(AddCardsToDeck(leftCardCount - leftCardsFolder.transform.childCount, 1));
        }
        
        if (rightCardCount > rightCardsFolder.transform.childCount) //It Depends to the Gates' scripts result
        {
            StartCoroutine(AddCardsToDeck(rightCardCount - rightCardsFolder.transform.childCount, 2));
        }
        
        leftCardsCountText.text = leftCardCount.ToString();
        rightCardsCountText.text = rightCardCount.ToString();

        if (_isStarted && !_isFinished)
        {
            var transform1 = transform;
            var position = transform1.position;
            position = new Vector3(position.x, position.y,
                position.z + Time.deltaTime * 10);
            transform1.position = position;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 defaultTouchPos = touch.deltaPosition;

            if (touch.phase == TouchPhase.Began && !_isStarted)
            {
                _isStarted = true;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _isTouched = true;
                if (touch.position.x - defaultTouchPos.x > 5f || touch.position.x - defaultTouchPos.x < -5f)
                {
                    Vector2 finalTouchPos = touch.position;
                    if (finalTouchPos.x - defaultTouchPos.x > 5f)
                    {
                        _transactionWay = 2;
                    }
                    else if (finalTouchPos.x - defaultTouchPos.x < 5f)
                    {
                        _transactionWay = 1;
                    }
                }

                StartCoroutine(CardTransaction(_transactionWay));
            }

            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                _transactionWay = 0;
                _isTouched = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Finish"))
        {
            Destroy(other.GetComponent<BoxCollider>());
            _isFinished = true;
            finalScore.text = (rightCardCount + leftCardCount).ToString();
        }
    }

    #endregion

    #region Custom Functions

    #region Static Functions

    public void AddCards(int addCount, int whichHand)
    {
        StartCoroutine(AddCardsToDeck(addCount, whichHand));
    }

    public void DeleteCards(int deleteCount, int whichHand)
    {
        StartCoroutine(DeleteCardsFromDeck(deleteCount, whichHand));
    }
    
    #endregion

    #region Loop Functions

    public IEnumerator AddCardsToDeck(int addCount, int whichHand)
    {
        if(whichHand == 1)
        {
            int i = 0;
            while (i < addCount)
            {
                i++;
                leftCardCount = leftCardCount + 1;
                var position = leftHand.transform.position;
                spawnedCard = Instantiate(cardPrefab, new Vector3(position.x, position.y + leftCardCount * 0.05f, position.z),
                    new Quaternion(0.71f, 0, 0, -0.71f));
                spawnedCard.transform.parent = leftCardsFolder.transform;
                spawnedCard.transform.name = "left " + leftCardCount;
                leftCardsCountText.text = leftCardCount.ToString();
                yield return new WaitForSeconds(0.15f);
            }
            
            if(leftCardCount <= leftCardsFolder.transform.childCount)
            {
                yield break;
            }
        }
        
        if(whichHand == 2)
        {
            int i = 0;
            while (i < addCount)
            {
                i++;
                rightCardCount = rightCardCount + 1;
                var position = rightHand.transform.position;
                spawnedCard = Instantiate(cardPrefab, new Vector3(position.x, position.y + rightCardCount * 0.05f, position.z),
                    new Quaternion(0.71f, 0, 0, -0.71f));
                spawnedCard.transform.parent = rightCardsFolder.transform;
                spawnedCard.transform.name = "right " + rightCardCount;
                rightCardsCountText.text = rightCardCount.ToString();
                yield return new WaitForSeconds(0.15f);
            }
            
            if(rightCardCount <= rightCardsFolder.transform.childCount)
            {
                yield break;
            }
        }
    }

    public IEnumerator DeleteCardsFromDeck(int deleteCount, int whichHand)
    {
        if (whichHand == 1)
        {
            int i = 0;
            while (i < deleteCount)
            {
                i++;
                string cardName = leftCardsFolder.transform.GetChild(leftCardsFolder.transform.childCount - 1).name;
                GameObject cardToDelete = GameObject.Find(cardName);
                Destroy(cardToDelete);
                if(leftCardCount >= leftCardsFolder.transform.childCount)
                {
                    yield break;
                }
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        if (whichHand == 2)
        {
            int i = 0;
            while (i < deleteCount)
            {
                i++;
                string cardName = rightCardsFolder.transform.GetChild(rightCardsFolder.transform.childCount - 1).name;
                GameObject cardToDelete = GameObject.Find(cardName);
                Destroy(cardToDelete);
                if(rightCardCount >= rightCardsFolder.transform.childCount)
                {
                    yield break;
                }
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    #endregion

    #region Card Transaction
    
    private IEnumerator CardTransaction(int whichWay)
    {
        if (whichWay == 2) // To Right
        {
            for (int i = 0; _isTouched && leftCardsFolder.transform.childCount > 0; i++)
            {
                string cardName = leftCardsFolder.transform.GetChild(leftCardsFolder.transform.childCount - 1).name;
                GameObject cardToTransact = GameObject.Find(cardName);

                string cardName2;
                Transform cardTransactPos;

                if (rightCardCount > 0)
                {
                    cardName2 = rightCardsFolder.transform.GetChild(rightCardsFolder.transform.childCount - 1).name;
                    cardTransactPos = GameObject.Find(cardName2).transform;
                    cardToTransact.transform.position = Vector3.Lerp(cardToTransact.transform.position,
                        new Vector3(cardTransactPos.transform.position.x,
                            cardTransactPos.position.y + i * 0.05f,
                            cardTransactPos.transform.position.z), 0.1f);
                }
                else
                {
                    cardTransactPos = rightHand.transform;
                    cardToTransact.transform.position = Vector3.Lerp(cardToTransact.transform.position,
                        new Vector3(cardTransactPos.transform.position.x,
                            cardTransactPos.transform.position.y + i * 0.05f,
                            cardTransactPos.transform.position.z), 0.1f);
                }

                yield return new WaitForSeconds(0.15f);
                
                cardToTransact.transform.parent = rightCardsFolder.transform;
                cardToTransact.name = "left " + rightCardCount;
            }
        }

        if (whichWay == 1) //To Left
        {
            for (int i = 0; _isTouched && rightCardsFolder.transform.childCount > 0; i++)
            {
                string cardName = rightCardsFolder.transform.GetChild(rightCardsFolder.transform.childCount - 1).name;
                GameObject cardToTransact = GameObject.Find(cardName);

                string cardName2;
                Transform cardTransactPos;

                if (rightCardCount > 0)
                {
                    cardName2 = leftCardsFolder.transform.GetChild(leftCardsFolder.transform.childCount - 1).name;
                    cardTransactPos = GameObject.Find(cardName2).transform;
                    cardToTransact.transform.position = Vector3.Lerp(cardToTransact.transform.position,
                        new Vector3(cardTransactPos.transform.position.x,
                            cardTransactPos.position.y + i * 0.05f,
                            cardTransactPos.transform.position.z), 0.1f);
                }
                else
                {
                    cardTransactPos = leftHand.transform;
                    cardToTransact.transform.position = Vector3.Lerp(cardToTransact.transform.position,
                        new Vector3(cardTransactPos.transform.position.x,
                            cardTransactPos.transform.position.y + i * 0.05f,
                            cardTransactPos.transform.position.z), 0.1f);
                }

                yield return new WaitForSeconds(0.15f);

                cardToTransact.transform.parent = leftCardsFolder.transform;
                cardToTransact.name = "left " + leftCardCount;
            }
        }
        
        yield break;
    }

    #endregion

    #endregion
}
