using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPhoton : MonoBehaviourPunCallbacks
{
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
}

