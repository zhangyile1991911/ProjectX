using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenUI(UIEnum.PhoneWindow,openComplete,null);
    }

    void openComplete(IUIBase openUI)
    {
        var phoneWindow = openUI as PhoneWindow;
        phoneWindow.IsDebug = true;
    }
}
