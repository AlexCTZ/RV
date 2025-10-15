using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public SpawnAndGrab spawnAndGrab;
    public GameObject spawnObject;
    public void OnClick()
    {
        spawnAndGrab.objectToSpawn = spawnObject;
        spawnAndGrab.SpawnInHand();
    }
}
