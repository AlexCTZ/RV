using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObject : MonoBehaviour
{
    [SerializeField] private Transform snap; // The destination Transform
    public bool isMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Snap")) return;
        if (isMoving) return;
        Debug.Log("collider entered");

        
        var otherSnap = other.transform.Find("Where");

        
        var localRotationToGo = Quaternion.LookRotation(snap.forward);

        
        other.transform.root.localRotation = localRotationToGo * other.transform.localRotation;

        
        var deltaPosition = snap.position - otherSnap.position;

        //TODO update other position with deltaPosition
        other.transform.root.position += deltaPosition;
        
    }
}
