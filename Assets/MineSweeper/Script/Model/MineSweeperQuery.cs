using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Loyufei;

namespace MineSweeper
{
    public class MineSweeperQuery
    {
        private struct Info 
        {
            public Info(IOffset2DInt offset, int mineCount)
                => (Offset, MineCount) = (offset, mineCount);

            public IOffset2DInt Offset    { get; }
            public int          MineCount { get; }
        }

        private List<Info>    Offsets { get; } = new();
        
        public IOffset2DInt[] AllMine { get; private set; }
        
        public IOffset2DInt Size      { get; set; }
        public int          MineCount { get; set; }

        #region Detect

        public bool IsDetected(IOffset2DInt offset) 
        {
            return Offsets.Exists(o => Equals(o.Offset.X, offset.X) && Equals(o.Offset.Y, offset.Y));
        }

        public void Detected(IOffset2DInt offset, int mineCount) 
        {
            Offsets.Add(new(offset, mineCount));
        }

        public IEnumerable<(IOffset2DInt offset, int mineCount)> GetDetected() 
        {
            foreach(var info in Offsets) 
            {
                if (info.MineCount < 0) { continue; }

                yield return (info.Offset, info.MineCount);
            }

            Offsets.Clear();
        }

        #endregion

        public void SetMines(IEnumerable<IOffset2DInt> mines) 
        {
            AllMine = mines.ToArray();
        }
    }
}
