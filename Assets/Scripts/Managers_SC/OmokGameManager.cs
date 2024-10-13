using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokGameManager : MonoBehaviour
{
    #region Singleton
    static OmokGameManager instance;
    public static OmokGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject _gameManager = new GameObject("GameManager");
                _gameManager.AddComponent<OmokGameManager>();
                instance = _gameManager.GetComponent<OmokGameManager>();
            }
            return instance;
        }
    }
    #endregion

    #region Refer Manager
    [SerializeField]
    NetworkManager network;
    public NetworkManager Network { get { return network; } }

    [SerializeField]
    AccountManager account;
    public AccountManager Account { get { return account; } }

    [SerializeField]
    LoadingManager loading;
    public LoadingManager Loading { get { return loading; } }

    #endregion

    #region Cashing Manager

    ResourcesManager resource = new ResourcesManager();
    public ResourcesManager Resource { get { return resource; } }

    SceneInfoManager scene = new SceneInfoManager();
    public SceneInfoManager Scene { get { return scene; } }

    #endregion

    private void Awake()
    {
        InitSingleton();
        loading.Init(); 
    }
    

    public void InitSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
