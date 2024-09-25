using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    InputController input;

    void Awake()
    {
        input.Init();    
    }

    void Update()
    {
        //input.Execute();
    }
}
