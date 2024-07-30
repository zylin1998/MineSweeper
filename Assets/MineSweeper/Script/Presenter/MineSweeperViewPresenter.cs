using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using Loyufei;
using Loyufei.DomainEvents;
using Loyufei.ViewManagement;
using UnityEngine;

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

        private void Init()
        {
            var listeners = View.ToArray();

            listeners
                .OfType<ButtonListener>()
                .FirstOrDefault()
                .AddListener((id) => { Pause(); });

            ToggleListeners = listeners
                .OfType<ToggleListener>()
                .ToDictionary(k => k.Id);

            ToggleListeners[0].AddListener((id) => ToggleEvent(ToggleListeners[0]));
            ToggleListeners[1].AddListener((id) => ToggleEvent(ToggleListeners[1]));

            Timer.Elapsed += () =>
            {
                _PassTime += 1;

                Updater.Update(Declarations.Timer, _PassTime);
            };
        }

        #region Override

        protected override void RegisterEvents()
        {
            Register<LayoutGridView>(Layout);
            Register<UpdateGridView>(UpdateGrid);
            Register<GameOver>(GameOver);
        }

        #endregion

        #region ListenerEvents

        private void ToggleEvent(ToggleListener toggle)
        {
            if (!toggle.Listener.isOn) { return; }

            for(var i = 0; i <= 1; i++) 
            {
                if (i == toggle.Id) { continue; }

                ToggleListeners[i].Listener.SetIsOnWithoutNotify(false);
            }

            _DetectedType = -1 - toggle.Id;
        }

        private void Pause()
        {
            SettleEvents(_OpenSetting);

            Timer.Stop();
        }

        private void Detected(MineListener listener)
        {
            if (!_Interactable) { return; }

            if (_DetectedType == -2) 
            {
                if (View.SetFlag(listener))
                {
                    Updater.Update(Declarations.MineCount, View.MineCount);
                }
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

            Timer.Start();
        }

        public void UpdateGrid(UpdateGridView update) 
        {
            View.ShowGround();

            var fulfilled = View.CheckFulfilled();

            if (fulfilled) 
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

        #endregion
    }
}