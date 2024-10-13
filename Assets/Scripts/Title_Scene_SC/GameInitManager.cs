using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitManager : MonoBehaviour
{
    [SerializeField] GameObject gameManagerObj;

    void Awake()
    {
        InitManager();
    }

    public void InitManager()
    {
        GameObject gmnr = GameObject.Find("OmokGameManager");
        if (gmnr == null)
        {
            GameObject go = GameObject.Instantiate(gameManagerObj);
            go.name = "OmokGameManager";
        }
    }
}
