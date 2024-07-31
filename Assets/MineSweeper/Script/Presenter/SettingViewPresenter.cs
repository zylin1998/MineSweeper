using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Loyufei;
using Loyufei.DomainEvents;
using Loyufei.ViewManagement;

namespace MineSweeper
{
    public class SettingViewPresenter : Presenter
    {
        public SettingViewPresenter(SettingView view, DomainEventService service) : base(service)
        {
            View = view;

            Init();
        }

        public SettingView View { get; }

        public Dictionary<int, DropdownListener> DropdownListeners { get; private set; }
        public Dictionary<int, ButtonListener>   ButtonListeners   { get; private set; }

        public Dictionary<int, int> Size { get; } = new()
        { { 0, Declarations.MinSize }, { 1, Declarations.MinSize } };

        private int _MinMineCount = (int)(Declarations.MinSize.Pow(2) * 0.1);
        private int _MineCount    = (int)(Declarations.MinSize.Pow(2) * 0.1);

        private UnPause _UnPause = new();

        protected override void RegisterEvents()
        {
            Register<OpenSetting>(Open);
        }

        private void Init()
        {
            var listeners = View.ToArray();

            DropdownListeners = listeners.OfType<DropdownListener>().ToDictionary(d => d.Id);
            ButtonListeners   = listeners.OfType<ButtonListener>()  .ToDictionary(d => d.Id);

            var min = Declarations.MinSize;
            var max = Declarations.MaxSize;

            SetOption(DropdownListeners[0], min, max.X);
            SetOption(DropdownListeners[1], min, max.Y);

            DropdownListeners[0].AddListener((value) => SetSize(0, (int)value + min));
            DropdownListeners[1].AddListener((value) => SetSize(1, (int)value + min));
            DropdownListeners[2].AddListener((value) => _MineCount = (int)value + _MinMineCount);

            SetOption(DropdownListeners[2], _MinMineCount, (int)(Size[0] * Size[1] * 0.7));

            ButtonListeners[0].AddListener((id) => Start());
            ButtonListeners[1].AddListener((id) => Quit());
            ButtonListeners[2].AddListener((id) => Close());
        }

        private void SetOption(DropdownListener dropdown, int min, int max) 
        {
            var listener = dropdown.Listener;
            
            listener.options.Clear();
            listener.AddOptions(GetOptions(min, max).ToList());

            listener.SetValueWithoutNotify(0);
        }

        private IEnumerable<TMP_Dropdown.OptionData> GetOptions(int min, int max) 
        {
            for (int i = min; i <= max; i++) 
            {
                yield return new TMP_Dropdown.OptionData(i.ToString());
            }
        }

        private void SetSize(int id, int size)
        {
            Size[id] = size;

            var length = Size[0] * Size[1];

            _MinMineCount = (int)(length * 0.1);

            _MineCount = _MinMineCount;
            
            SetOption(DropdownListeners[2], _MinMineCount, (int)(length * 0.7));
        }

        private void Start() 
        {
            SettleEvents(new GameStart(new Offset2DInt(Size[0], Size[1]), _MineCount));

            View.Close();
        } 

        private void Quit() 
        {
            Application.Quit();
        }

        private void Close() 
        {
            View.Close();

            SettleEvents(_UnPause);
        }

        private void Open(OpenSetting open) 
        {
            View.Open();
        }
    }

    public class OpenSetting : DomainEventBase 
    {

    }
}