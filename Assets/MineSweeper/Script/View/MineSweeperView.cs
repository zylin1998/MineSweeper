using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Loyufei;
using Loyufei.ViewManagement;
using UnityEngine.UI;
using System.Linq;

namespace MineSweeper
{
    public class MineSweeperView : MenuBase, IUpdateGroup
    {
        [SerializeField]
        private Transform       _MineArea;
        [SerializeField]
        private GridLayoutGroup _GridLayout;

        private int _FlagCount = 0;

        private Stack<MineListener> Listeners { get; } = new();

        public MineListener.Pool MinePool { get; private set; }

        public int MineCount
            => Query.MineCount - _FlagCount;
        public MineListener this[IOffset2DInt offset] 
            => Listeners.FirstOrDefault(l => l.Offset.X == offset.X && l.Offset.Y == offset.Y);
        public IEnumerable<IUpdateContext> Contexts 
            => GetComponentsInChildren<IUpdateContext>();
        [Inject]
        public MineSweeperQuery Query   { get; }
        
        [Inject]
        private void Construct(MineListener.Pool pool) 
        {
            MinePool = pool;

            MinePool.Content = _MineArea;
        }

        public IEnumerable<MineListener> Layout() 
        {
            var size = Query.Size;

            _GridLayout.constraintCount = size.X;

            _FlagCount = 0;

            var (x, y) = (0, 0);
            for (var i = 0; i < size.X * size.Y; i++) 
            {
                var listener = MinePool.Spawn(new Offset2DInt(x, y));

                Listeners.Push(listener);

                yield return listener;

                (x, y) = x >= size.X - 1 ? (0, ++y) : (++x, y);
            }
        }

        public void RemoveLayout() 
        {
            for (;  Listeners.Any();) 
            {
                MinePool.Despawn(Listeners.Pop());
            }
        }

        public void ShowMine(bool fulfilled) 
        {
            foreach(var offset in Query.AllMine) 
            {
                this[offset]?.SetContext(fulfilled ? -2 : -1);
            }
        }

        public void ShowGround()
        {
            foreach (var info in Query.GetDetected().ToArray())
            {
                this[info.offset].SetContext(info.mineCount);
            }
        }

        public bool CheckFulfilled() 
        {
            if (Listeners.Count(l => l.Context <= -2) != Query.AllMine.Length) { return false; }

            if (Query.AllMine.All(m => this[m].Context <= -2)) 
            {
                ShowMine(true);

                return true;
            }

            return false;
        }

        public bool SetFlag(MineListener listener) 
        {
            if (listener.Context == -3 && _FlagCount < Query.MineCount) 
            {
                listener.SetContext(-2);

                _FlagCount++;

                return true;
            }

            if (listener.Context == -2) 
            {
                listener.SetContext(-3);

                _FlagCount--;

                return true;
            }

            return false;
        }
    }
}