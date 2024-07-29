using System.Collections;
using System.Collections.Generic;
using Loyufei;

namespace MineSweeper
{
    public class MineSweeperGrid : FlexibleRepositoryBase<int, int>
    { 
        public void Reset(int capacity) 
        {
            var overflow = _Reposits.Count > capacity;

            if (overflow) 
            {
                Release(capacity);
            }

            else 
            {
                Create(capacity - _Reposits.Count);
            }
        }
    }
}