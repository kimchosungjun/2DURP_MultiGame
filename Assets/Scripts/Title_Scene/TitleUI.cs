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
        GameManager.Instance.Account.TitleController = this;
    }

    public void LoadSoloPlay()
    {
        GameManager.Instance.Scene.LoadScene(SceneNameType.SoloGame_Scene);
    }

    public void LoadMultiPlay()
    {
        int _btnCnt = btns.Length;
        for(int i=0; i<_btnCnt; i++)
        {
            btns[i].interactable = false;
        }
        GameManager.Instance.Loading.FadeOut();
        GameManager.Instance.Account.RenewCache();
    }

    public void CanSkipLogin(bool canSkip)
    {
        if (canSkip)
            GameManager.Instance.Scene.LoadScene(SceneNameType.Lobby_Scene);
        else
            GameManager.Instance.Scene.LoadScene(SceneNameType.Login_Scene);
        GameManager.Instance.Loading.FadeIn();

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
