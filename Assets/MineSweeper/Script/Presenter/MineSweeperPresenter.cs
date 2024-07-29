using Loyufei;
using Loyufei.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace MineSweeper
{
    public class MineSweeperPresenter : Presenter
    {
        public MineSweeperPresenter(MineSweeperModel model, DomainEventService service) : base(service)
        {
            Model = model;
        }

        public MineSweeperModel Model { get; }

        private UpdateGridView _Update   = new();
        private GameOver       _GameOver = new();

        public void Start(GameStart start) 
        {
            Model.Start(start.Size, start.MineCount);

            SettleEvents(_Update);
        }

        public void Detected(Detected detected) 
        {
            var result = Model.Detected(detected.Offset);

            SettleEvents(result ? _Update : _GameOver);
        }
    }

    public class GameStart : DomainEventBase 
    {
        public GameStart(IOffset2DInt size, int mineCount)
        {
            Size      = size;
            MineCount = mineCount;
        }

        public IOffset2DInt Size      { get; }
        public int          MineCount { get; }
    }

    public class Detected : DomainEventBase 
    {
        public Detected(IOffset2DInt offset)
        {
            Offset = offset;
        }

        public IOffset2DInt Offset { get; }
    }

    public class UpdateGridView : DomainEventBase 
    {

    }

    public class GameOver : DomainEventBase 
    {

    }
}
