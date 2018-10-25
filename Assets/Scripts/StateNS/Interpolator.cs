using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateNS {
	public 	enum InterpolatorState {
		Uninitialized,
		WaitingFirstFrame,
		Interpolating
	}
	

	public class Interpolator : MonoBehaviour {
	
		public static Interpolator Instance;
		
		public float MaxTimeUntilJump;
		public int FrameBufferSize;
	
		private readonly SortedDictionary<IInterpolatableState, bool> _frames = 
			new SortedDictionary<IInterpolatableState, bool>(new WorldStateComparer());

		private IInterpolatableState _pastState;
		private IInterpolatableState _targetState;

		public InterpolatorState CurrentState { get; private set; }
		private float _time = -1;
	
		// Use this for initialization
		public void Awake () {
			if (Instance == null) {
				Instance = this;
				CurrentState = InterpolatorState.Uninitialized;
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(this);	
			}
		}

		public void StartInterpolating() {
			if (CurrentState == InterpolatorState.Uninitialized) {
				CurrentState = InterpolatorState.WaitingFirstFrame;	
			}
		}
	
		public void AddFrame(IInterpolatableState state) {
			lock (this) {
				Debug.Log("Adding Frame");
				_frames[state] = true;
			}
		}

		public void Update() {
			lock (this) {
				if (CurrentState == InterpolatorState.Uninitialized) {
					return;
				}
			
				PickPastState();
				if(_pastState == null) { return; }

				Debug.Log("Interpolating with " + _frames.Count + "frames on queue");
			
				_time += Time.deltaTime;

				if (_targetState != null && _targetState.TimeStamp() < _time) {
					_pastState = _targetState;
					_targetState = null;
				}
			
				var nextState = _frames.FirstOrDefault().Key;
			
				if(_targetState == null && nextState == null) { return; }
			
				if (_targetState == null) {
					_targetState = nextState;
					_frames.Remove(nextState);
				}
			
				_pastState.UpdateState(
					(_time - _pastState.TimeStamp()) / (_targetState.TimeStamp() -_pastState.TimeStamp()),
					_targetState
				);
			}
		}

		private void PickPastState() {
			if(CurrentState == InterpolatorState.Uninitialized) { return; }
		
			var newPastState = _frames.FirstOrDefault().Key;

			if (CurrentState == InterpolatorState.WaitingFirstFrame) {
				if (_frames.Count >= FrameBufferSize) {
					SetStateAndRemove(newPastState);
				}
				return;
			}
		
			if(_targetState == null && (_time - _pastState.TimeStamp()) > MaxTimeUntilJump) {
				_pastState = null;
				CurrentState = InterpolatorState.WaitingFirstFrame;
				return;
			}
		
		
			while (newPastState != null && newPastState.TimeStamp() < _pastState.TimeStamp()) {
				_frames.Remove(newPastState);
				newPastState = _frames.FirstOrDefault().Key;
			}
		}

		private void SetStateAndRemove(IInterpolatableState newState) {
			if (newState != null) {
				CurrentState = InterpolatorState.Interpolating;
				_pastState = newState;
				_time = _pastState.TimeStamp();
				_frames.Remove(newState);
			}
		}
		
		
		private class WorldStateComparer: IComparer<IInterpolatableState> {
			public int Compare(IInterpolatableState x, IInterpolatableState y) {
				System.Diagnostics.Debug.Assert(y != null, "y != null");
				System.Diagnostics.Debug.Assert(x != null, "x != null");
				if (y.TimeStamp() < x.TimeStamp()) {
					return 1;
				}
			
				if (y.TimeStamp() > x.TimeStamp()) {
					return -1;
				}

				return 0;
			}
		}
	}
}