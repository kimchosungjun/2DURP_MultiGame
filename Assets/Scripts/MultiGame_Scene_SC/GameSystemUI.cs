using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSystemUI : MonoBehaviour
{
    [SerializeField, Header("게임 시작 전 UI")] GameObject beforeUIObj;
    public void DecideActiveState_BeforeUI()
    {
        if(beforeUIObj.activeSelf)
            beforeUIObj.SetActive(false);
        else
            beforeUIObj.SetActive(true);
    }
}
