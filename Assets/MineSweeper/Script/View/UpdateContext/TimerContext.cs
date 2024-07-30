using Loyufei;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MineSweeper
{
    public class TimerContext : MonoBehaviour, IUpdateContext
    {
        [SerializeField]
        private TextMeshProUGUI _TimeText;

        public object Id => Declarations.Timer;

        public void SetContext(object value)
        {
            _TimeText.SetText(value.ToString());
        }
    }
}