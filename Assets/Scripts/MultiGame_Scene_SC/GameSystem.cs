using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameSystem : MonoBehaviourPunCallbacks
{
    [SerializeField] GameSystemUI ui;
    [SerializeField] Image tstimage;
    [SerializeField] Sprite spr;
    [SerializeField] Sprite tstspr;
    [SerializeField] PhotonView pv;

    [PunRPC]
    public void SetImage()
    {
        tstimage.sprite = spr;
    }

    public void SetTest()
    {

        tstimage.sprite = tstspr;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetTest();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            pv.RPC("SetImage", RpcTarget.AllBuffered);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (ui == null)
                ui = FindObjectOfType<GameSystemUI>();

            if (ui == null)
                return;

            ui.DecideActiveState_BeforeUI();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            pv.RPC("DecideBeforeAll", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DecideBeforeAll()
    {
        if (ui == null)
            ui = FindObjectOfType<GameSystemUI>();

        if (ui == null)
            return;

        ui.DecideActiveState_BeforeUI();
    }
}
