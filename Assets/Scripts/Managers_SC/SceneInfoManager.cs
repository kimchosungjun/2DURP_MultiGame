using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneInfoManager 
{
    public void LoadScene(SceneNameType _sceneNameType)
    {
        SceneManager.LoadScene((int)_sceneNameType);
    }

    /// <summary>
    /// 두번째 인자값이 무조건 True여야 작동
    /// </summary>
    /// <param name="_sceneNameType"></param>
    /// <param name="_noEvent"></param>
    public void LoadScene(SceneNameType _sceneNameType, bool _noEvent)
    {
        if(_noEvent)
        {
            SceneManager.LoadScene((int)_sceneNameType);
            OmokGameManager.Instance.Loading.LinkFadeEffect();
        }
    }

    public string GetSceneName(SceneNameType _sceneNameType)
    {
        return Enums.GetEnumName<SceneNameType>(_sceneNameType);
    }

    public int GetSceneNumber(SceneNameType _sceneNameType)
    {
        return Enums.GetEnumValue<SceneNameType>(_sceneNameType);
    }

    string otherPlayerName = string.Empty;
    public void EnterRoom(SceneNameType __sceneNameType, string _otherPlayerName="")
    {
        SceneManager.LoadScene((int)__sceneNameType);
        SceneManager.sceneLoaded += LoadRoomScene;
        if (!_otherPlayerName.Equals(string.Empty)) otherPlayerName = _otherPlayerName;
    }

    void LoadRoomScene(Scene _scene, LoadSceneMode _mode)
    {
        SceneManager.sceneLoaded -= LoadRoomScene;
        OmokGameManager.Instance.Loading.ShowLoading(false);
        OmokGameManager.Instance.Network.CallCreateGameSystem();
        GameObject uiObj = GameObject.FindWithTag("UI");
        if (uiObj == null)
        {
            Debug.Log("해당 UI를 찾지 못했다");
            return;
        }
        GameSystemUI gameSystemUI = uiObj.GetComponent<GameSystemUI>();
        gameSystemUI.SetOtherName(otherPlayerName);
        gameSystemUI.SetMyName();
    }

    
}
