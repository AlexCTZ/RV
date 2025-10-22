using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMove : MonoBehaviour
{
    public SpawnAndGrabMove spawnAndGrab;
    public GameObject spawnObject;

    public void OnClick()
    {

        spawnAndGrab.objectToSpawn = spawnObject;
        spawnAndGrab.SpawnInHand();

    }
}
