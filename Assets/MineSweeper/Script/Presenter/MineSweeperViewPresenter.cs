using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Loyufei;
using Loyufei.DomainEvents;
using Loyufei.ViewManagement;

namespace MineSweeper
{
    public class MineSweeperViewPresenter : Presenter
    {
        public MineSweeperViewPresenter(Timer timer, MineSweeperView view, DomainEventService service) : base(service)
        {
            View  = view;
            Timer = timer;

            Init();
        }

        public MineSweeperView View    { get; }
        [Inject]
        public DataUpdater     Updater { get; }
        [Inject]
        public Timer           Timer   { get; }
        
        private int  _DetectedType = -1;
        private int  _PassTime     = 0;
        private bool _Interactable = false;

        private OpenSetting _OpenSetting = new();
        
        public Dictionary<int, ToggleListener> ToggleListeners { get; private set; }

        #region Override

        protected override void RegisterEvents()
        {
            Register<LayoutGridView>(Layout);
            Register<UpdateGridView>(UpdateGrid);
            Register<GameOver>(GameOver);
            Register<UnPause>(UnPause);
        }

        #endregion

        private void Init()
        {
            var listeners = View.ToArray();

            listeners
                .OfType<ButtonListener>()
                .FirstOrDefault()
                .AddListener((id) => Pause());

            ToggleListeners = listeners
                .OfType<ToggleListener>()
                .ToDictionary(k => k.Id);

            ToggleListeners[0].AddListener((id) => ToggleEvent(0));
            ToggleListeners[1].AddListener((id) => ToggleEvent(1));

            Timer.Elapsed += () =>
            {
                _PassTime += 1;

                Updater.Update(Declarations.Timer, _PassTime);
            };
        }

        #region ListenerEvents

        private void ToggleEvent(int id)
        {
            if (!ToggleListeners[id].Listener.isOn) { return; }

            _DetectedType = -1 - id;
        }

        private void Pause()
        {
            SettleEvents(_OpenSetting);

            Timer.Stop();
        }

        private void Detected(MineListener listener)
        {
            if (!_Interactable) { return; }

            if (_DetectedType == -2 && View.SetFlag(listener)) 
            {
                Updater.Update(Declarations.MineCount, View.MineCount);
            }

            if (_DetectedType == -1 && listener.Context == -3)
            {
                SettleEvents(new Detected(listener.Offset));
            }
        }

        #endregion

        #region Event Recieve

        public void Layout(LayoutGridView layout) 
        {
            View.RemoveLayout();

            var listeners = View.Layout().ToArray();

            listeners.ForEach(l =>
            {
                l.AddListener((offset) => Detected(l));
            });

            _Interactable = true;
            _PassTime     = 0;

            Updater.Update(Declarations.Timer    , _PassTime);
            Updater.Update(Declarations.MineCount, View.MineCount);

            ToggleListeners[0].Listener.isOn = true;
            
            Timer.Start();
        }

        public void UpdateGrid(UpdateGridView update) 
        {
            if (View.ShowGround())
            {
                Updater.Update(Declarations.MineCount, View.MineCount);
            }

            if (View.CheckFulfilled()) 
            {
                _Interactable = false;

                Timer.Stop();
            }
        }

        public void GameOver(GameOver gameOver) 
        {
            View.ShowMine(false);

            _Interactable = false;
            
            Timer.Stop();
        }

        public void UnPause(UnPause unPause) 
        {
            if (!_Interactable) { return; }
            
            Timer.Start();
        }

        #endregion
    }

    public class UnPause : DomainEventBase 
    {

    }
}