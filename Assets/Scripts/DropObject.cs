using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] private int level;

    private void Start()
    {
        gameObject.name = level.ToString();
    }
}
