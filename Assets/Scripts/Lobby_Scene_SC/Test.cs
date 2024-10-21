using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(PhotonNetwork.CountOfPlayersInRooms);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("나는 방장");
            }
            else
            {
                Debug.Log("나는 방장이 아님");

            }
        }
    }
}
