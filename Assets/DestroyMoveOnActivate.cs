using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMoveOnActivate : MonoBehaviour
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
        Destroy(gameObject.transform.parent);
    }
}
