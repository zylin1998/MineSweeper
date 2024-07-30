using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Loyufei.ViewManagement
{
    public class ToggleListener : MonoListenerAdapter<Toggle>
    {
        public override void AddListener(Action<object> callBack)
        {
            Listener.onValueChanged.AddListener((isOn) => callBack.Invoke(isOn));
        }
    }
}