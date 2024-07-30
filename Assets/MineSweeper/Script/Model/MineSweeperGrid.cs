using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Loyufei;

namespace MineSweeper
{
    public class MineSweeperGrid : FlexibleRepositoryBase<int, int>
    {
        [Inject]
        public Random Random { get; }

        public IOffset2DInt Size { get; protected set; }

        public IEntity<int> this[IOffset2DInt offset] 
            => SearchAt(offset.X + offset.Y * Size.X);

        public static IOffset2DInt[] Surround { get; } = new IOffset2DInt[8]
        {
            new Offset2DInt(-1,  1),
            new Offset2DInt( 0,  1),
            new Offset2DInt( 1,  1),
            new Offset2DInt(-1,  0),
            new Offset2DInt( 1,  0),
            new Offset2DInt(-1, -1),
            new Offset2DInt( 0, -1),
            new Offset2DInt( 1, -1)
        };

        public bool IsClamp(IOffset2DInt offset) 
        {
            return offset.X.IsClamp(0, Size.X - 1) && offset.Y.IsClamp(0, Size.Y - 1);
        }

        public int Detected(IOffset2DInt offset)
        {
            var check = Check(offset);

            if (Equals(check, 1)) { return -1; }

            if (Equals(check, 0)) { return Surround.Count(delta => Check(offset, delta) == 1); }

            return check;
        }

        public int Check(IOffset2DInt offset, IOffset2DInt delta) 
        {
            return Check(new Offset2DInt(offset.X + delta.X, offset.Y + delta.Y));
        }

        public int Check(IOffset2DInt offset) 
        {
            if (!IsClamp(offset)) { return int.MinValue; }

            return this[offset].Data;
        }

        public IEnumerable<IOffset2DInt> BuryMine(int count, bool useSeed = false) 
        {
            var positions = Random.UniqueArray(0, _Reposits.Count, count, useSeed);

            var (x, y) = (0, 0);
            for (int i = 0; i < _Reposits.Count; i++) 
            {
                var offset  = new Offset2DInt(x, y);
                var reposit = this[offset].To<IReposit>(); 
                var isMine  = positions.Any(p => Equals(reposit.Identity, p));
                
                reposit.Preserve(isMine ? 1 : 0);

                (x, y) = x >= Size.X - 1 ? (0, ++y) : (++x, y);
                
                if (isMine) { yield return offset; }
            }
        }

        public void Reset(IOffset2DInt size) 
        {
            Size = size;
            var capacity = Size.X * Size.Y;
            var overflow = _Reposits.Count > capacity;
            
            if (overflow) 
            {
                Release(capacity);
            }

            else 
            {
                var amount = capacity - _Reposits.Count;
                
                Create(amount).ToArray();
            }

            for (var index = 0; index < _Reposits.Count; index++) 
            {
                _Reposits[index].SetIdentify(index);
            }
        }
    }
}