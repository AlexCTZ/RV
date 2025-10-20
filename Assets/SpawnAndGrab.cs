using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnAndGrab : MonoBehaviour
{
    private AudioSource spawnSound;

    [Header("Références")]
    public GameObject objectToSpawn; // ton prefab
    public XRRayInteractor rightHandInteractor; // main droite
    public XRRayInteractor leftHandInteractor;  // main gauche (optionnel)

    [Header("Options")]
    public bool spawnInRightHand = true;

    private void Awake()
    {
        spawnSound = GetComponent<AudioSource>();
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
        spawnSound.Play();

    }
}
