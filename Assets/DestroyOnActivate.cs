using System.Collections;
using UnityEngine;

public class DestroyOnActivate : MonoBehaviour
{
    private AudioSource destroySound;

    private void Awake()
    {
        destroySound = GetComponent<AudioSource>();
    }

    public void OnActivate()
    {
        destroySound.Play();
        StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitWhile(() => destroySound.isPlaying);
        Destroy(gameObject);
    }
}
