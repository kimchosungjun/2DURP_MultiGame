using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject decideObj;
    [SerializeField] Button[] btns;

    public void Start()
    {
        OmokGameManager.Instance.Account.TitleController = this;
    }

    public void LoadSoloPlay()
    {
        OmokGameManager.Instance.Scene.LoadScene(SceneNameType.SoloGame_Scene);
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
        {
            OmokGameManager.Instance.Scene.LoadScene(SceneNameType.Lobby_Scene);
            OmokGameManager.Instance.Network.Connect();
        }
        else
            OmokGameManager.Instance.Scene.LoadScene(SceneNameType.Login_Scene);
        OmokGameManager.Instance.Loading.FadeIn();

        // 있어도 되고 없어도 되는 코드
        //int _btnCnt = btns.Length;
        //for (int i = 0; i < _btnCnt; i++)
        //{
        //    btns[i].interactable = true;
        //}
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
}
