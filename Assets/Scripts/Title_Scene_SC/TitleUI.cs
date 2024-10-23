using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    #region Link Manager
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
            Screen.SetResolution(1080, 1920, true);
        }
    }
    #endregion

    #region Link UI
    [SerializeField] GameObject decideObj;
    [SerializeField] Button[] btns;
    public void Start()
    {
        OmokGameManager.Instance.Account.TitleController = this;
    }

    public void LoadSoloPlay()
    {
        OmokGameManager.Instance.Scene.LocalLoadScene(SceneNameType.SoloGame_Scene);
    }

    public void LoadMultiPlay()
    {
        int _btnCnt = btns.Length;
        for(int i=0; i<_btnCnt; i++)
        {
            btns[i].interactable = false;
        }
        OmokGameManager.Instance.Loading.FadeOut();
        OmokGameManager.Instance.Account.RenewCache();
    }

    public void CanSkipLogin(bool canSkip)
    {
        if (canSkip)
            OmokGameManager.Instance.Network.Connect();
        else
        {
            OmokGameManager.Instance.Scene.LocalLoadScene(SceneNameType.Login_Scene);
            OmokGameManager.Instance.Loading.FadeIn();
        }
    }

    public void ActiveExitDecidePanel()
    {
        decideObj.SetActive(true);
    }
    
    public void YesExit()
    {
        Application.Quit();
    }

    public void NoExit()
    {
        decideObj.SetActive(false);
    }
    #endregion
}
