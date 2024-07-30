using Loyufei.ViewManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MineSweeper
{
    public class DropdownListener : MonoListenerAdapter<TMP_Dropdown>
    {
        public override void AddListener(Action<object> callBack)
        {
            Listener.onValueChanged.AddListener((value) => callBack.Invoke(value));
        }
    }
}