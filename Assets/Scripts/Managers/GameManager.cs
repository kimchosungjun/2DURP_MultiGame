using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject _gameManager = new GameObject("GameManager");
                _gameManager.AddComponent<GameManager>();
                instance = _gameManager.GetComponent<GameManager>();
            }
            return instance;
        }
    }
    #endregion

    #region Refer Manager

    [SerializeField]
    OmokGridManager omok;
    public OmokGridManager Omok { get { return omok; } }

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
        omok.Init();
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

    public void ClearAllManagers()
    {
        omok.Clear();
    }
}
