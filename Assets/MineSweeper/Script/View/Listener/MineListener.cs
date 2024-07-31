using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Loyufei;
using Loyufei.ViewManagement;
using TMPro;

namespace MineSweeper
{
    public class MineListener : ButtonListener
    {
        public class Pool : MemoryPool<IOffset2DInt, MineListener> 
        {
            public Pool() : base() 
            {
                DespawnRoot = new GameObject("MineListener").transform;
            }

            public Transform Content     { get; set; }
            public Transform DespawnRoot { get; }

            protected override void Reinitialize(IOffset2DInt offset, MineListener listener)
            {
                listener.Offset = offset;
                listener.SetContext(-3);
                
                listener.transform.SetParent(Content);

                listener.gameObject.SetActive(true);
            }

            protected override void OnDespawned(MineListener listener)
            {
                listener.transform.SetParent(DespawnRoot);

                listener.gameObject.SetActive(false);
            }
        }

        [SerializeField]
        private Image _Icon;
        [SerializeField]
        private TextMeshProUGUI _Surround;

        [Inject]
        public MineSprites Sprites { get; }

        public IOffset2DInt Offset { get; set; }

        public int Context { get; set; }

        public override void AddListener(Action<object> callBack)
        {
            Listener.onClick.RemoveAllListeners();

            Listener.onClick.AddListener(() => callBack.Invoke(Offset));
        }

        public void SetContext(int context) 
        {
            Context = context;

            if (Equals(context, -1)) { _Icon.sprite = Sprites.Mine; }
            if (Equals(context, -2)) { _Icon.sprite = Sprites.Flag; }
            if (Equals(context, -3)) { _Icon.sprite = Sprites.Cover; }

            if (context.IsClamp(0, 8)) { _Icon.sprite = Sprites.Ground; }
            if (context.IsClamp(1, 8)) { _Surround.SetText(context.ToString()); }

            else { _Surround.SetText(string.Empty); }
        }
    }

    [Serializable]
    public class MineSprites
    {
        [SerializeField]
        private Sprite _Cover;
        [SerializeField]
        private Sprite _Ground;
        [SerializeField]
        private Sprite _Flag;
        [SerializeField]
        private Sprite _Mine;

        public Sprite Cover  => _Cover;
        public Sprite Ground => _Ground;
        public Sprite Flag   => _Flag;
        public Sprite Mine   => _Mine;
    }
}