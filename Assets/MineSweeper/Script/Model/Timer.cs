using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;

namespace MineSweeper
{
    public class Timer
    {
        public float Interval { get; set; } = 1;
        public bool  Enable   { get; private set; } = false;

        private List<Action> _CallBacks = new();

        public event Action Elapsed 
        {
            add    => _CallBacks.Add(value);

            remove => _CallBacks.Remove(value);
        }

        public void Start() 
        {
            Enable = true;

            Observable
                .Interval(TimeSpan.FromSeconds(Interval))
                .TakeWhile(l => Enable)
                .Subscribe(l => _CallBacks.ForEach(c => c.Invoke()));
        }

        public void Stop() 
        {
            Enable = false;
        }
    }
}
