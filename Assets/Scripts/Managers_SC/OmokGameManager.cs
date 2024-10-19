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

    [SerializeField]
    SceneInfoManager scene;
    public SceneInfoManager Scene { get { return scene; } }
    #endregion

    #region Cashing Manager

    ResourcesManager resource = new ResourcesManager();
    public ResourcesManager Resource { get { return resource; } }

    #endregion

    private void Awake()
    {
        InitSingleton();
        loading.Init();
        SetResolution();
    }
    
    public void SetResolution()
    {
        int targetWidth = 1080;  // 예: 1080 (FHD 가로 해상도)
        int targetHeight = 1920; // 예: 1920 (FHD 세로 해상도)

        float targetAspect = (float)targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        Camera.main.aspect = targetAspect;

        if (currentAspect > targetAspect)
        {
            float inset = 1.0f - targetAspect / currentAspect;
            Camera.main.rect = new Rect(inset / 2.0f, 0.0f, 1.0f - inset, 1.0f);
        }
        else
        {
            float inset = 1.0f - currentAspect / targetAspect;
            Camera.main.rect = new Rect(0.0f, inset / 2.0f, 1.0f, 1.0f - inset);
        }
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
