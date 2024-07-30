using Loyufei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public static class Declarations
    {
        public const string MineSweeper = "MineSweeper";
        public const string MineCount   = "MineCount";
        public const string Timer       = "Timer";

        public static IOffset2DInt MaxSize { get; } = new Offset2DInt(53, 24);
        public static int          MinSize { get; } = 5;
    }
}
