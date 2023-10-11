using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public int Amount { get { return amount; } }
    protected int amount;

    public string Name { get { return _name; } }
    protected string _name;

    public abstract void MakePrefab(string name);
}
