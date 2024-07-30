using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Loyufei;

namespace MineSweeper
{
    public class MineSweeperModel
    {
        public MineSweeperModel(MineSweeperGrid grid, MineSweeperQuery query) 
        {
            Grid  = grid;

            Query = query;
        }

        public MineSweeperGrid  Grid  { get; }
        public MineSweeperQuery Query { get; }

        public void Start(IOffset2DInt size, int count) 
        {
            Grid.Reset(size);

            Query.Size      = size;
            Query.MineCount = count;

            Query.SetMines(Grid.BuryMine(count));
        }

        public bool Detected(IOffset2DInt offset) 
        {
            var detected = Grid.Detected(offset);

            Query.Detected(offset, detected);

            if (detected < 0) { return false; }

            if (detected > 0) { return true; }

            MineSweeperGrid.Surround.ForEach(o =>
            {
                var next = new Offset2DInt(o.X + offset.X, o.Y + offset.Y);

                if (Query.IsDetected(next)) { return; }

                Detected(next);
            });

            return true;
        }
    }
}
