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
    public GameObject PanelLoad;
    public string[] files;
    [SerializeField] private Transform panelLoadtransform;      
    [SerializeField] private GameObject buttonPrefab; 
    private bool isSaveActive = false;


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
        var allMove = GameObject.FindGameObjectsWithTag("Movable");
        SaveData data = new SaveData();

        foreach (var obj in allObjects)
        {
            data.objects.Add(new SpawnedObjectData
            {
                name = obj.GetComponent<PrefabName>().AdressableName,
                position = new Vector3(obj.transform.position.x, obj.transform.position.y-10, obj.transform.position.z),
                rotation = obj.transform.rotation
            });
        }
        Debug.Log("objets normaux loadés");

        foreach (var obj in allMove)
        {
            data.moves.Add(new SpawnedMoveData
            {
                name = obj.GetComponent<PrefabName>().AdressableName,
                position = new Vector3(obj.transform.Find("Platform Move 520").position.x, obj.transform.Find("Platform Move 520").position.y - 10, obj.transform.Find("Platform Move 520").position.z),
                rotation = new Quaternion(0, obj.transform.Find("Platform Move 520").rotation.y, 0, obj.transform.Find("Platform Move 520").rotation.w),
                Endposition = new Vector3(obj.transform.Find("End").position.x, obj.transform.Find("End").position.y - 10, obj.transform.Find("End").position.z),
                Speed = obj.GetComponent<SpeedDefiner>().speed
            });
        }
        Debug.Log("objets mouvants loadés");

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
        GameObject[] arr2 = GameObject.FindGameObjectsWithTag("Movable");
        var list2 = new List<GameObject>(arr2);
        foreach (var go in list2) Destroy(go);
        if (isRespawn)Instantiate(EndPrefab);
        if (isSaveActive)
        {
            LoadLevel(savePath);
        }

        
    }

    public void Load()
    {
        PanelLoad.SetActive(true);
        GameObject Base = Instantiate(buttonPrefab, panelLoadtransform);
        var txtbase = Base.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        txtbase.text = "Niveau de base";
        Base.GetComponent<Button>().onClick.AddListener(() =>
        {
            isSaveActive = false;
            savePath = DefaultSavePath;
            cancel();
            GameObject[] arr = GameObject.FindGameObjectsWithTag("LoadButton");
            var list = new List<GameObject>(arr);
            foreach (var go in list) Destroy(go);
            PanelLoad.SetActive(false);
            
        });

        foreach (string file in files) {
            GameObject btnGO = Instantiate(buttonPrefab, panelLoadtransform);

            // Modifier le texte du bouton (TextMeshProUGUI ou Text)
            var txt = btnGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null) txt.text = Path.GetFileNameWithoutExtension(file);
            
            btnGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                
                cancel(false);
                isSaveActive = true;
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
        foreach (var movData in sceneData.moves)
        {
            yield return StartCoroutine(SpawnMovesFromAddressables(movData));
        }
        PanelLoad.SetActive(false);
        


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

            GameObject instance = Instantiate(prefab, new Vector3(objData.position.x, objData.position.y+10, objData.position.z), objData.rotation);
            Debug.Log($"Spawned network object: {objData.name} à {objData.position}");
        }
        else
        {
            Debug.LogWarning($"Échec du chargement de l'adressable: {objData.name}");
        }
        
    }

    private IEnumerator SpawnMovesFromAddressables(SpawnedMoveData data)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(data.name);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab introuvable pour l'adressableKey: {data.name}");
                yield break;
            }

            GameObject instance = Instantiate(prefab, new Vector3(data.position.x, data.position.y + 10,data.position.z),data.rotation);
            instance.transform.Find("End").position = new Vector3(data.Endposition.x, data.Endposition.y + 10,data.Endposition.z);
            instance.GetComponent<SpeedDefiner>().speed = data.Speed;
            Debug.Log($"Spawned network object: {data.name} à {data.position}");
        }
        else
        {
            Debug.LogWarning($"Échec du chargement de l'adressable: {data.name}");
        }
    }
}