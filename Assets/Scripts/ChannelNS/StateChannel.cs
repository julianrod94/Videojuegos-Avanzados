using System.Diagnostics;
using System.Threading;
using StateNS;

namespace ChannelNS {
    public abstract class StateChannel<T>: Channel<T> where T: IInterpolatableState {
        private Interpolator _interpolator;
        private IStateProvider<T> _stateProvider;
        private long _period;
        private Timer _timer;
        
        public StateChannel<T> SetupStateProvider(IStateProvider<T> stateProvider) {
            _stateProvider = stateProvider;
            return this;
        }

        public StateChannel<T> SetupPeriod(long period) {
            _period = period;
            return this;
        }
        
        public StateChannel<T> SetupInterpolator(Interpolator interpolator) {
            _interpolator = interpolator;
            return this;
        }

        public void StartSending() {
            TimerCallback tc = state => { SendState(_stateProvider.PollState()); };
            _timer = new Timer(tc, null, 0, _period);
        }

        public void DisposeTimer() {
            _timer.Dispose();
        }
        
        public void SendState(T newState) {
            byte[] data = SerializeData(newState);
            Strategy.SendPackage(data);
        }

        protected override void ProcessData(byte[] bytes) {
            T newState = DeserializeData(bytes);
            _interpolator.AddFrame(newState);
        }
    }
}