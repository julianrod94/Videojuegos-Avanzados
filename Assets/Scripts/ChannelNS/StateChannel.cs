using System;
using System.Threading;
using StateNS;
using UnityEngine;

namespace ChannelNS {
    public abstract class StateChannel<T> : Channel<T> where T : IInterpolatableState<T> {
        private long _period;
        private Func<T> _stateProvider;
        private Timer _timer;
        public Interpolator<T> Interpolator;

        public void SetupStateProvider(Func<T> stateProvider) {
            _stateProvider = stateProvider;
        }

        public void SetupPeriod(long period) {
            _period = period;
        }

        public void SetupInterpolator(Interpolator<T> interpolator) {
            Interpolator = interpolator;
            interpolator.StartInterpolating();
        }

        public void StartSending() {
            Interpolator.StartInterpolating();
            TimerCallback tc = state => SendState();
            _timer = new Timer(tc, null, 0, _period);
        }

        public override void Dispose() {
           base.Dispose();
            if (_timer != null) {
                _timer.Dispose();
            }
        }

        public void SendState() {
            var newState = _stateProvider();
            if(newState == null) return;
            var data = SerializeData(newState);
            Strategy.SendPackage(data);
        }

        protected override void ProcessData(byte[] bytes) {
            var newState = DeserializeData(bytes);
            Interpolator.AddFrame(newState);
        }
    }
}