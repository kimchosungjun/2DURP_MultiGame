using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            GameManager.Instance.Loading.LinkFadeEffect();
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
}
