using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.streamingAssetsPath, "save.json");
    }

    //Sauvegarde tous les objets avec AddressableObject
    public void Save()
    {
        var allObjects = GameObject.FindGameObjectsWithTag("Spawnable");
        SaveData data = new SaveData();

        foreach (var obj in allObjects)
        {
            data.objects.Add(new SpawnedObjectData
            {
                name = obj.GetComponent<PrefabName>().AdressableName,
                position = obj.transform.position,
                rotation = obj.transform.rotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Sauvegarde de {data.objects.Count} objets -> {savePath}");
    }
}