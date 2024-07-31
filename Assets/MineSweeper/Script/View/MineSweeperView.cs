using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Loyufei;
using Loyufei.ViewManagement;

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
            for (; Listeners.Any();) 
            {
                MinePool.Despawn(Listeners.Pop());
            }
        }

        public void ShowMine(bool fulfilled) 
        {
            var context = fulfilled ? -2 : -1;

            Query.AllMine.ForEach(offset =>
            {
                this[offset]?.SetContext(context);
            });
        }

        public bool ShowGround()
        {
            var update = false;

            foreach (var info in Query.GetDetected().ToArray())
            {
                var listener = this[info.offset];

                if (listener.Context == -2) 
                {
                    _FlagCount--;

                    update = true;
                }
                
                listener.SetContext(info.mineCount);
            }

            return update;
        }

        public bool CheckFulfilled() 
        {
            if (Listeners.Count(l => l.Context <= -2) != Query.AllMine.Length) { return false; }

            var fulfilled = Query.AllMine.All(m => this[m].Context <= -2);
            
            if (fulfilled) { ShowMine(true); }

            return fulfilled;
        }

        public bool SetFlag(MineListener listener) 
        {
            var context = listener.Context;
            var isCover = context == -3;
            
            if (!context.IsClamp(-3, -2))  { return false; }
            if (isCover && MineCount == 0) { return false; }

            var (nextState, delta) = isCover ? (-2, 1) : (-3, -1);
            
            listener.SetContext(nextState);
            
            _FlagCount += delta;
            
            return true;
        }
    }
}