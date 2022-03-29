using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideData : MonoBehaviour
{
    public GameObject playerData;

    void Start(){
        playerData.SetActive(false);
    }

    public void OnMouseOver(){
        playerData.SetActive(true);
    }

    public void OnMouseExit(){
        playerData.SetActive(false);
    }
}
