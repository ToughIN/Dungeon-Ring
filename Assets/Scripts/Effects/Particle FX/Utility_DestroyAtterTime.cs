using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_DestroyAtterTime : MonoBehaviour
{
    [SerializeField] private float timeUntilDestroy = 5;

    private void Awake()
    {
        Destroy(gameObject,timeUntilDestroy);
    }
}
