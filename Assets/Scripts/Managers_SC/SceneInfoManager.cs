using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class SceneInfoManager : MonoBehaviourPunCallbacks
{
    // Local Scene 불러오기
    public void LocalLoadScene(SceneNameType _sceneNameType) { SceneManager.LoadScene((int)_sceneNameType); }

    // Multi Scene 불러오기
    public void MultiLoadScene(SceneNameType __sceneNameType) => PhotonNetwork.LoadLevel((int)__sceneNameType);
}
