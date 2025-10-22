using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndParenting : MonoBehaviour
{
    private Transform Parent;
    void Start()
    {
        Parent = transform.parent;
    }

    public void OnSelectEntered()
    {
        transform.SetParent(null);
    }
    public void OnSelectExited()
    {
        transform.SetParent(Parent);
    }
}
