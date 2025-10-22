using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSpeed : MonoBehaviour
{
    public GameObject Platform;
    public Slider slider;

    public void OnClick()
    {
        Platform.GetComponent<SpeedDefiner>().speed = (int)slider.value;
        //Platform = null;
        gameObject.SetActive(false);
    }
}
