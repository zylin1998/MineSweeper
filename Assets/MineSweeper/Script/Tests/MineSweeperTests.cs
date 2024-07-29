using Zenject;
using NUnit.Framework;
using Loyufei;
using System.Linq;
using System.Drawing;
using UnityEngine.UIElements;
using UnityEngine;

namespace MineSweeper.UnitTests
{
    [TestFixture]
    public class MineSweeperTests : ZenjectUnitTestFixture
    {
        private int[]        _Seeds     = new int[16]
        { 1, 17, 25, 3 , 9, 15, 21, 99, 50, 77, 65, 34, 47, 80, 29, 59, };
        private IOffset2DInt _Size      = new Offset2DInt(5, 5);
        private int          _MineCount = 4;
        [SetUp]
        public void Binding() 
        {
            Container
                .Bind<Loyufei.Random>()
                .AsSingle();

            Container
                .Bind<MineSweeperGrid>()
                .AsSingle();
        }

        [Test]
        public void GridTest() 
        {
            var random = Container.Resolve<Loyufei.Random>();
            var grid   = Container.Resolve<MineSweeperGrid>();

            random.SetSeeds(_Seeds);

            grid.Reset(_Size);

            Assert.AreEqual(25, grid.Capacity);

            var offsets = grid.BuryMine(_MineCount, true).ToArray();

            Assert.AreEqual(4, offsets.Length);
        }

        private void ShowMap(MineSweeperGrid grid) 
        {
            var str = string.Empty;
            var (x, y) = (0, 0);
            for (int i = 0; i < grid.Capacity; i++)
            {
                var offset = new Offset2DInt(x, y);
                var entity = grid[offset];

                str += string.Format("{0} ", entity.Data);

                if (x >= grid.Size.X - 1) 
                {
                    str += "\n";
                }

                (x, y) = x >= grid.Size.X - 1 ? (0, ++y) : (++x, y);
            }

            Debug.Log(str);
        }
    }
}