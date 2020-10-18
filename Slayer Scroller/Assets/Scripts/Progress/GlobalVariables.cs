using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : Singleton<GlobalVariables>

{
    public bool initialized = false;
    public bool HasUI = false; //whether there is a ui popup on the screen

}
