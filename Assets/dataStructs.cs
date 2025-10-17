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
public class SaveData
{
    public List<SpawnedObjectData> objects = new List<SpawnedObjectData>();
}