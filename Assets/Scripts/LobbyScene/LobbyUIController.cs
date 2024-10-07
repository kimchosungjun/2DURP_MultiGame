using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] LobbyExitUI exit;
    [SerializeField] LoginUI login;
    [SerializeField] CreateAccountUI account;

    public LobbyExitUI Exit { get { return exit; } }
    public LoginUI Login { get { return login; } }
    public CreateAccountUI Account { get { return account; } }

    void Awake()
    {
        if (login != null)
            login.Init();
        else
            Debug.LogError("로그인 ui 부재");

        if (exit != null)
            exit.Init();
        else
            Debug.LogError("종료 ui 부재");

        if (account != null)
            account.Init();
        else
            Debug.LogError("회원가입 ui 부재");
    }

    private void Start()
    {
        GameManager.Instance.Account.LobbyController = this;
    }

    void Update()
    {
        exit.Execute();
    }
}
