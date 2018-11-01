using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateNS {
    public enum InterpolatorState {
        Uninitialized,
        WaitingFirstFrame,
        Interpolating
    }

    public class Interpolator<T> where T : IInterpolatableState<T> {
        private readonly SortedDictionary<T, bool> _frames =
            new SortedDictionary<T, bool>(new WorldStateComparer());

        private T _pastState;
        private T _targetState;
        private float _time = -1;
        public int FrameBufferSize = 2;

        public float MaxTimeUntilJump = 1;
        public T PresentState { get; private set; }

        public InterpolatorState CurrentState { get; private set; }


        public void StartInterpolating() {
            if (CurrentState == InterpolatorState.Uninitialized) CurrentState = InterpolatorState.WaitingFirstFrame;
        }

        public void AddFrame(T state) {
            if (PresentState == null || state.TimeStamp() > PresentState.TimeStamp()) {
                lock (this) {
                    _frames[state] = true;
                }
            }
        }

        public void Update(float deltaTime) {
            lock (this) {
                if (CurrentState == InterpolatorState.Uninitialized) return;

                PickPastState();
                if (_pastState == null) return;

                _time += deltaTime;

                if (_targetState != null && _targetState.TimeStamp() < _time) {
                    _pastState = _targetState;
                    _targetState = default(T);
                }

                var nextState = _frames.FirstOrDefault().Key;

                if (_targetState == null && nextState == null) return;

                if (_targetState == null) {
                    _targetState = nextState;
                    _frames.Remove(nextState);
                }

                PresentState = _pastState.UpdateState(
                    (_time - _pastState.TimeStamp()) / (_targetState.TimeStamp() - _pastState.TimeStamp()),
                    _targetState
                );
            }
        }

        private void PickPastState() {
            if (CurrentState == InterpolatorState.Uninitialized) return;

            var newPastState = _frames.FirstOrDefault().Key;

            if (CurrentState == InterpolatorState.WaitingFirstFrame) {
                if (_frames.Count >= FrameBufferSize) 
                    SetStateAndRemove(newPastState);
                return;
            }

            if (_targetState == null && _time - _pastState.TimeStamp() > MaxTimeUntilJump) {
                _pastState = default(T);
                CurrentState = InterpolatorState.WaitingFirstFrame;
                return;
            }

            if (_targetState == null) {
                return;
            }

            while (newPastState != null && newPastState.TimeStamp() <= _targetState.TimeStamp()) {
                _frames.Remove(newPastState);
                newPastState = _frames.FirstOrDefault().Key;
            }
        }

        private void SetStateAndRemove(T newState) {
            if (newState != null) {
                CurrentState = InterpolatorState.Interpolating;
                _pastState = newState;
                _time = _pastState.TimeStamp();
                _frames.Remove(newState);
            }
        }


        private class WorldStateComparer : IComparer<T> {
            public int Compare(T x, T y) {
                System.Diagnostics.Debug.Assert(y != null, "y != null");
                System.Diagnostics.Debug.Assert(x != null, "x != null");
                if (y.TimeStamp() < x.TimeStamp()) return 1;

                if (y.TimeStamp() > x.TimeStamp()) return -1;

                return 0;
            }
        }
    }
}