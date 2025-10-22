using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnedObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class SpawnedMoveData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 Endposition;
    public int Speed;
}

[System.Serializable]
public class SaveData
{
    public List<SpawnedObjectData> objects = new List<SpawnedObjectData>();
    public List<SpawnedMoveData> moves = new List<SpawnedMoveData>();
}