using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Manager : MonoBehaviour
{
    public static Environment_Manager instance;
    

    public void Start()
    {
        instance = this;
    }
}
