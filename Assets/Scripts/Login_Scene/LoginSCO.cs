using UnityEngine;

[CreateAssetMenu(fileName ="LoginSCO",menuName ="LoginSCO",order =int.MaxValue)]
public class LoginSCO : ScriptableObject
{
    public bool isLogin = false;
    public string id = "";
    public string password = ""; 
}

public class LoadLoginInformation
{
    public bool isLogin = false;
    public string id = "";
    public string password = "";

    public LoadLoginInformation(bool isLogin, string id, string password)
    {
        this.isLogin = isLogin;
        this.id = id;
        this.password = password;
    }
}
