using System;
using System.Collections;
using System.Collections.Generic;
using ChannelNS;
using EventNS.keepAliveNS;
using SenderStrategyNS;
using ServerNS;
using UnityEngine;
using Utils;

namespace PackageProcessNS.KeepAliveNS {
    public class ServerKeepAliveManager: MonoBehaviour {
        
        Dictionary<int, KeepAliveChannel> _keepAlives = new Dictionary<int, KeepAliveChannel>();
        Dictionary<int, float> _lastKeepAlives = new Dictionary<int, float>();

        public static ServerKeepAliveManager Instance;

        private void Awake() {
            Instance = this;
        }

        public void AddPlayer(int id, ChannelManager cm) {
            var channel = new KeepAliveChannel((b => {
                if (_lastKeepAlives.ContainsKey(id)) {
                    _lastKeepAlives[id] = CurrentTime.Time;
                    Debug.Log("Receiving KEEP ALIVE");
                }
            } ), new ReliableStrategy(0.1f, 20));

            cm.RegisterChannel((int) RegisteredChannels.KeepAliveChannel, channel);
            _keepAlives[id] = channel;
            _lastKeepAlives[id] = CurrentTime.Time;
            StartCoroutine(SendKeepAlive(id));
        }

        void RemovePlayer(int id) {
            if (_keepAlives.ContainsKey(id)) {
                _keepAlives[id].Dispose();
                _keepAlives.Remove(id);
                _lastKeepAlives.Remove(id);
            }
        }

        IEnumerator SendKeepAlive(int id) {
            while (_keepAlives.ContainsKey(id)) {
                Debug.Log("SENDING KEEP ALIVE");
                _keepAlives[id].SendEvent(true);
                yield return new WaitForSeconds(1);
            }
        }
        
        private void Update() {
            var dced = new List<int>();
            foreach (var lastKeepAlive in _lastKeepAlives) {
                if (CurrentTime.Time- lastKeepAlive.Value > 5) {
                    Debug.LogError("PLAYER " + lastKeepAlive.Key + "WAS DISCONECTED");
                    dced.Add(lastKeepAlive.Key);
                }
            }
            
            foreach (var i in dced) {
                RemovePlayer(i);
                ServerGameManager.Instance.DisconnectPlayer(i);
            }
        }

        private void OnDestroy() {
            foreach (var keepAliveChannel in _keepAlives) {
                keepAliveChannel.Value.Dispose();
            }
        }
    }
}