using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            GlobalSignalManager.Inst.FireSignal(new JumpButtonPressedSignal()) ;
        }
    }
}
