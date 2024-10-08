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

    public string GetSceneName(SceneNameType _sceneNameType)
    {
        return Enums.GetEnumName<SceneNameType>(_sceneNameType);
    }

    public int GetSceneNumber(SceneNameType _sceneNameType)
    {
        return Enums.GetEnumValue<SceneNameType>(_sceneNameType);
    }
}
