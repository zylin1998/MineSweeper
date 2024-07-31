using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Loyufei;
using Loyufei.DomainEvents;

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

        protected override void RegisterEvents()
        {
            Register<GameStart>(Start);
            Register<Detected>(Detected);
        }

        public void Start(GameStart start) 
        {
            Model.Start(start.Size, start.MineCount);

            SettleEvents(new LayoutGridView());
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

    public class LayoutGridView : DomainEventBase 
    {
        
    }

    public class UpdateGridView : DomainEventBase 
    {

    }

    public class GameOver : DomainEventBase 
    {

    }
}
