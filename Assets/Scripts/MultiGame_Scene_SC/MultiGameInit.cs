using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiGameInit : MonoBehaviour
{
    [SerializeField] PhotonView pv;
    [SerializeField] GameSystem gameSystem = null;

    [SerializeField] float readyTimer = 5f;

    public void Ready()
    {
        if (!pv.IsMine)
        {
            Debug.Log("내 것이 아니오.");
            return;
        }

        if (FindObjectOfType<GameSystem>() != null)
            gameSystem = FindObjectOfType<GameSystem>();

        // 방장일 때만 게임 시스템을 생성
        if (gameSystem == null && PhotonNetwork.IsMasterClient)
        {
            //pv.RPC("CreateGameSystem", RpcTarget.AllBuffered);
            gameSystem = PhotonNetwork.Instantiate("GameSystem", Vector3.zero, Quaternion.identity).GetComponent<GameSystem>();
        }
        StartCoroutine(ReadyTimer());
    }

    IEnumerator ReadyTimer()
    {
        float timer = 0f;
        while (timer < readyTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void NotReady()
    {
        // 방장일 때만 게임 시스템을 삭제
        if (gameSystem != null && PhotonNetwork.IsMasterClient)
        {
            //pv.RPC("DestroyGameSystem", RpcTarget.AllBuffered); // 오타 수정
            PhotonNetwork.Destroy(gameSystem.gameObject);
            gameSystem = null; // 삭제 후 null로 초기화
        }
        StopAllCoroutines();
    }

    [PunRPC]
    public void CreateGameSystem()
    {
        if (gameSystem == null) // 중복 생성 방지
        {
            gameSystem = PhotonNetwork.Instantiate("GameSystem", Vector3.zero, Quaternion.identity).GetComponent<GameSystem>();
        }
    }

    [PunRPC]
    public void DestroyGameSystem()
    {
        if (gameSystem != null) // null 체크 추가
        {
            PhotonNetwork.Destroy(gameSystem.gameObject);
            gameSystem = null; // 삭제 후 null로 초기화
        }
    }
}
