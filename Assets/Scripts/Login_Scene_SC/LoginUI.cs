using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LoginUI : MonoBehaviour
{
    enum LoginType
    {
        ID = 0,
        PASSWORD = 1
    }

    UnityAction sucessLogin = null;
    UnityAction failLogin = null;

    [Header("로그인 관련")]    
    [SerializeField] Toggle rememberToggle;
    [SerializeField] TMP_InputField[] loginInputs;
    [SerializeField] Button loginBtn;

    [Header("로그인 실패")]
    [SerializeField] TextMeshProUGUI warnText;
    [SerializeField] Animator warnTextAnim;

    public void Init()
    {
        sucessLogin = null;
        failLogin = null;

        sucessLogin += PassLoginSystem;
        failLogin += FailLoginSystem;
        loginBtn.onClick.AddListener(() => PressLogin());
    }    

    

    /// <summary>
    /// 아이디 창을 눌렀을 때 호출
    /// </summary>
    public void ResetLogin()
    {
        warnText.gameObject.SetActive(false);
    }

    public void PressLogin()
    {
        string _id = loginInputs[(int)LoginType.ID].text;
        string _password = loginInputs[(int)LoginType.PASSWORD].text;
        OmokGameManager.Instance.Account.Login(_id, _password);
        OmokGameManager.Instance.Loading.ShowLoading(true);
    }

    public void CheckLogin(bool isLogin)
    {
        OmokGameManager.Instance.Loading.ShowLoading(false);
        if (isLogin)
            sucessLogin.Invoke();
        else
            failLogin.Invoke();
    }

    public void PassLoginSystem()
    {
        OmokGameManager.Instance.Network.Connect();
        OmokGameManager.Instance.Scene.LoadScene(SceneNameType.Lobby_Scene);
        //Debug.Log(OmokGameManager.Instance.Account.ID);
        OmokGameManager.Instance.Account.LoadData.RenewLog(OmokGameManager.Instance.Account.ID);
    }

    public void FailLoginSystem()
    {
        OmokGameManager.Instance.Account.ID = "";

        warnText.text = "로그인에 실패했습니다.";
        warnText.gameObject.SetActive(true);
        warnTextAnim.SetTrigger("Shake");

        // 아이디는 기억하지 않을 때만 초기화
        if (!rememberToggle.isOn)
            loginInputs[(int)LoginType.ID].text = "";

        loginInputs[(int)LoginType.PASSWORD].text = "";
    }

    public void OverlapLogin()
    {
        OmokGameManager.Instance.Loading.ShowLoading(false);
        warnText.text = "해당 ID는 이미 접속중입니다.";
        warnText.gameObject.SetActive(true);
        warnTextAnim.SetTrigger("Shake");

        // 아이디는 기억하지 않을 때만 초기화
        if (!rememberToggle.isOn)
            loginInputs[(int)LoginType.ID].text = "";

        loginInputs[(int)LoginType.PASSWORD].text = "";
    }
}
