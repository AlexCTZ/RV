using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SaveSpeed : MonoBehaviour
{

    public Slider slider;
    private AudioSource spawnSound;

    [Header("Références")]
    public GameObject objectToSpawn; 
    public XRRayInteractor rightHandInteractor; 
    public XRRayInteractor leftHandInteractor;  

    [Header("Options")]
    public bool spawnInRightHand = true;

    private void Awake()
    {
        spawnSound = GetComponent<AudioSource>();
    }
    public void OnClick()
    {
        SpawnInHand();
        
        //Platform = null;
        gameObject.SetActive(false);
    }

    

    

    public void SpawnInHand()
    {
        // Choisir la main
        XRRayInteractor hand = spawnInRightHand ? rightHandInteractor : leftHandInteractor;

        if (hand == null)
        {
            Debug.LogWarning("Aucune main assignée !");
            return;
        }

        // Spawn l’objet
        GameObject spawned = Instantiate(objectToSpawn, hand.transform.position, hand.transform.rotation);
        spawned.GetComponent<SpeedDefiner>().speed = (int)slider.value;
        spawnSound.Play();

    }
}
