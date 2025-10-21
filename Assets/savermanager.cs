using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SaveManager : MonoBehaviour
{
    public string Active;
    private string savePath;
    private string DefaultSavePath;
    public GameObject EndPrefab;
    private AudioSource saveSound;
    public GameObject Panel;
    public string[] files;
    [SerializeField] private Transform panel;      
    [SerializeField] private GameObject buttonPrefab; 


    private void Awake()
    {

        
        savePath = GetFirstFreeSavePath(Application.streamingAssetsPath);
        DefaultSavePath = savePath;
        saveSound = GetComponent<AudioSource>();
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
        saveSound.Play();
        Debug.Log($"Sauvegarde de {data.objects.Count} objets -> {savePath}");
        DefaultSavePath = GetFirstFreeSavePath(Application.streamingAssetsPath);
    }
    public void cancel(bool isRespawn = true)
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Spawnable");
        var list = new List<GameObject>(arr);
        foreach (var go in list) Destroy(go);
        if(isRespawn)Instantiate(EndPrefab);
    }

    public void Load()
    {
        Panel.SetActive(true);
        GameObject Base = Instantiate(buttonPrefab, panel);
        var txtbase = Base.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        txtbase.text = "Niveau de base";
        Base.GetComponent<Button>().onClick.AddListener(() =>
        {
            savePath = DefaultSavePath;
            cancel();
            GameObject[] arr = GameObject.FindGameObjectsWithTag("LoadButton");
            var list = new List<GameObject>(arr);
            foreach (var go in list) Destroy(go);
            Panel.SetActive(false);
            
        });

        foreach (string file in files) {
            GameObject btnGO = Instantiate(buttonPrefab, panel);

            // Modifier le texte du bouton (TextMeshProUGUI ou Text)
            var txt = btnGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null) txt.text = Path.GetFileNameWithoutExtension(file);
            
            btnGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                cancel(false);
                savePath = file;
                LoadLevel(file);
                GameObject[] arr = GameObject.FindGameObjectsWithTag("LoadButton");
                var list = new List<GameObject>(arr);
                foreach (var go in list) Destroy(go);
            });
        }
    }



    public string GetFirstFreeSavePath(string folderPath)
    {

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);


        files = Directory.GetFiles(folderPath, "save*.json");

        
        if (files.Length == 0)
            return Path.Combine(folderPath, "save1.json");

        var usedIndexes = files
            .Select(f => Path.GetFileNameWithoutExtension(f))  
            .Select(name =>
            {
                // Essayer extraire le nombre après "save"
                if (int.TryParse(name.Substring(4), out int n))
                    return n;
                return -1;
            })
            .Where(n => n > 0)
            .OrderBy(n => n)
            .ToList();


        int candidate = 1;
        foreach (var n in usedIndexes)
        {
            if (n != candidate) break;
            candidate++;
        }

        string fileName = $"save{candidate}.json";
        
        return Path.Combine(folderPath, fileName);
    }



    private SaveData sceneData;
    public void LoadLevel(string LevelToLoad)
    {
        if (LevelToLoad == null)
        {
            Debug.LogError("Aucun fichier JSON assigné au LevelSpawner !");
            return;
        }

        string jsonContent = File.ReadAllText(LevelToLoad);

        sceneData = JsonUtility.FromJson<SaveData>(jsonContent);

        if (sceneData == null || sceneData.objects == null)
        {
            Debug.LogError("Impossible de parser les données du JSON !");
            return;
        }


        StartCoroutine(SpawnObjectsFromJson());
    }




    private IEnumerator SpawnObjectsFromJson()
    {
        foreach (var objData in sceneData.objects)
        {
            yield return StartCoroutine(SpawnFromAddressables(objData));
        }
        Panel.SetActive(false);
        


    }

    private IEnumerator SpawnFromAddressables(SpawnedObjectData objData)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(objData.name);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab introuvable pour l'adressableKey: {objData.name}");
                yield break;
            }

            GameObject instance = Instantiate(prefab, objData.position, objData.rotation);
            Debug.Log($"Spawned network object: {objData.name} à {objData.position}");
        }
        else
        {
            Debug.LogWarning($"Échec du chargement de l'adressable: {objData.name}");
        }
        
    }
}