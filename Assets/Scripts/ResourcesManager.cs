using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager 
{
    public T LoadResource<T>(string _fileName, ResourceType _type)  where T : Object 
    {
        string _rootPaht = Enums.GetEnumName<ResourceType>(_type)+"/";
        _rootPaht += _fileName;
        T _res = Resources.Load<T>(_rootPaht);
        return _res;
    }  
}
