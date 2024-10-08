using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoginExitUI : MonoBehaviour
{
    [Header("UI 컨트롤러")]
    [SerializeField] LoginUIController controller;

    [Header("종료 UI")]
    bool isExitState = false;

    UnityAction pressExit = null;
    UnityAction pressReturnGame = null;

    [SerializeField] Button exitBtn;
    [SerializeField, Tooltip("0 : 투명한 판넬, 1 : 게임 종료 창")] GameObject[] exitPanels;
    [SerializeField, Tooltip("0 : Yes, 1 : No")] Button[] decideBtns;

    public void Init()
    {
        // 판넬이 켜져있다면 끄기
        if (exitPanels[0].activeSelf || exitPanels[1].activeSelf)
        {
            exitPanels[0].SetActive(false);
            exitPanels[1].SetActive(false);
        }

        pressExit = null;
        pressReturnGame = null;

        pressExit += PressExit;
        pressReturnGame += ReturnGame;

        exitBtn.onClick.AddListener(() => { pressExit(); });
        decideBtns[0].onClick.AddListener(() => ExitGame());
        decideBtns[1].onClick.AddListener(() => { pressReturnGame(); });
    }

    public void PressExit()
    {
        if (controller.Account.ClearAccount())
            return;

        exitPanels[0].SetActive(true);
        exitPanels[1].SetActive(true);
    }

    public void ReturnGame()
    {
        exitPanels[0].SetActive(false);
        exitPanels[1].SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 이미 종료창이 뜬 상태
            if (isExitState)
            {
                pressReturnGame();
            }
            // 종료창이 뜨지 않는 상태
            else
            {
                pressExit();
            }
        }
    }
}
