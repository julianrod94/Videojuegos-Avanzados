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
        }

        public void StartSending() {
            Interpolator.StartInterpolating();
            TimerCallback tc = state => SendState(_stateProvider());
            _timer = new Timer(tc, null, 0, _period);
        }

        public void DisposeTimer() {
            _timer.Dispose();
        }

        public void SendState(T newState) {
            Debug.Log("SendPacket " + newState);
            var data = SerializeData(newState);
            Debug.Log(data);
            Strategy.SendPackage(data);
        }

        protected override void ProcessData(byte[] bytes) {
            Debug.Log("Processing");
            var newState = DeserializeData(bytes);
            Debug.Log("newsteate" + newState);
            Interpolator.AddFrame(newState);
        }
    }
}