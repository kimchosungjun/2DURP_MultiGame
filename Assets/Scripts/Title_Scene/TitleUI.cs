using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject decideObj;
    public void LoadSoloPlay()
    {
        GameManager.Instance.Scene.LoadScene(SceneNameType.SoloGame_Scene);
    }

    public void LoadMultiPlay()
    {
        bool _isLogin = GameManager.Instance.Account.IsLoginState();
        if(_isLogin)
        {
            GameManager.Instance.Account.RenewCache();
            GameManager.Instance.Scene.LoadScene(SceneNameType.Lobby_Scene);
        }
        else
        {
            GameManager.Instance.Scene.LoadScene(SceneNameType.Login_Scene);
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
}
