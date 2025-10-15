using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnActivate : MonoBehaviour
{
    public void OnActivate()
    {
        Destroy(gameObject);
    }
}
