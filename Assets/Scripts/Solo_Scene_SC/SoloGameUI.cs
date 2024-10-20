using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloGameUI : MonoBehaviour
{
    [SerializeField, Header("종료")] GameObject exitObj;
    [SerializeField, Header("승리")] GameObject victoryObj;
    [SerializeField] TextMeshProUGUI victoryText;
    [SerializeField, Header("재대결")] GameObject rematchObj;
    [SerializeField, Header("솔로게임 컨트롤러")] SoloPlayController controller;

    [SerializeField,Header("0은 흰돌, 1은 검은돌")] Sprite[] dolSprites;
    [SerializeField, Header("0은 플레이어1, 1은 플레이어2")] Image[] dolImages;

    private void Awake()
    {
        exitObj.SetActive(false);
        victoryObj.SetActive(false);
        rematchObj.SetActive(false);
    }

    void Update()
    {
        #region Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitObj.activeSelf)
            {
                exitObj.SetActive(false);
            }
            else
            {
                exitObj.SetActive(true);
            }
        }
        #endregion
    }

    public void Victory(bool _isPlayer1Win)
    {
        if (_isPlayer1Win)
            victoryText.text = "플레이어 1 승리!!";
        else
            victoryText.text = "플레이어 2 승리!!";
        victoryObj.SetActive(true);
        StartCoroutine(ShowTimer());
    }

    IEnumerator ShowTimer()
    {
        float timer = 0f;
        while (timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        victoryObj.SetActive(false);
        rematchObj.SetActive(true);
    }

    public void Rematch()
    {
        controller.StopGame = false;
        controller.Rematch();
        rematchObj.SetActive(false);
    }

    public void ExitSolo()
    {
        OmokGameManager.Instance.Scene.LocalLoadScene(SceneNameType.Title_Scene);
    }

    public void InActiveExit()
    {
        exitObj.SetActive(false);
        if(rematchObj.activeSelf)
            rematchObj.SetActive(false);
    }

    public void SetImage(bool _isPlayer1Black)
    {
        if (_isPlayer1Black)
        {
            dolImages[0].sprite = dolSprites[1];
            dolImages[1].sprite = dolSprites[0];
        }
        else
        {
            dolImages[0].sprite = dolSprites[0];
            dolImages[1].sprite = dolSprites[1];
        }
    }
}
