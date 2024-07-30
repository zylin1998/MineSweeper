using Loyufei;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MineSweeper
{
    public class MineCountContext : MonoBehaviour, IUpdateContext
    {
        [SerializeField]
        private TextMeshProUGUI _MineCount;

        public object Id => Declarations.MineCount;

        public void SetContext(object value)
        {
            _MineCount.SetText(value.To<int>().ToString());
        }
    }
}