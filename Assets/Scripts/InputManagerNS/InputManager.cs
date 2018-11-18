﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace InputManagerNS {
    public enum InputEnum {
        Forward=0,
        Back,
        Left,
        Right
    }

    public class PlayerAction {
        public InputEnum inputCommand;
        public int inputNumber;
        public float deltaTime;
        public Quaternion rotation;

        public PlayerAction(
            InputEnum inputCommand, 
            int inputNumber, 
            float deltaTime,
            Quaternion rotation
            ) {
            this.inputCommand = inputCommand;
            this.inputNumber = inputNumber;
            this.deltaTime = deltaTime;
            this.rotation = rotation;
        }

        protected bool Equals(PlayerAction other) {
            return inputNumber == other.inputNumber;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerAction) obj);
        }

        public override int GetHashCode() {
            return inputNumber;
        }
    }
    
    public class InputManager {
        
        
        private int inputCounter = 0;
        private int lastEmptiedInput = -1;

        private readonly SortedDictionary<PlayerAction, bool> _inputs =
            new SortedDictionary<PlayerAction, bool>(new PlayerActionComparer());
        
        public void SubmitInput(InputEnum inputEnum, Quaternion rotation) {
            lock (this) {
                _inputs[new PlayerAction(inputEnum, inputCounter++, Time.deltaTime, rotation)] = true;
            }
        }
        
        public void ReceivePlayerAction(PlayerAction playerAction) {
            if(playerAction.inputNumber <= lastEmptiedInput) { return; }
            lock (this) {
                _inputs[playerAction] = true;
            }
        }

        public ICollection<PlayerAction> Inputs() {
            return _inputs.Keys;
        }
        
        public int Count() {
            return _inputs.Count;
        }

        public void EmptyUpTo(int inputNumber) {
            lock (this) {
                var itemsToRemove = _inputs.Where(a => a.Key.inputNumber <= inputNumber).ToList();
                foreach (var i in itemsToRemove) {
                    _inputs.Remove(i.Key);
                }
            }
        }
        
        public void EmptyAll(int lastInput) {
            lock (this) {
                lastEmptiedInput = lastInput;
                _inputs.Clear();
            }
        }
        
        private class PlayerActionComparer : IComparer<PlayerAction> {
            public int Compare(PlayerAction x, PlayerAction y) {
                return x.inputNumber - y.inputNumber;
            }
        }
    }

}