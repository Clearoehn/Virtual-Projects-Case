using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(10.76f, 10.76f, 10.76f), 0.05f);
    }
}
