using System;
using System.Collections.Generic;
using ChannelNS;
using EventNS.keepAliveNS;
using SenderStrategyNS;
using ServerNS;
using UnityEngine;

namespace PackageProcessNS.KeepAliveNS {
    public class ServerKeepAliveManager: MonoBehaviour {
        Dictionary<int, KeepAliveChannel> _keepAlives = new Dictionary<int, KeepAliveChannel>();
        Dictionary<int, long> _lastKeepAlives = new Dictionary<int, long>();

        public static ServerKeepAliveManager Instance;

        private void Awake() {
            Instance = this;
        }

        public void AddPlayer(int id, ChannelManager cm) {
            var channel = new KeepAliveChannel((b => {
                if (_lastKeepAlives.ContainsKey(id)) {
                    _lastKeepAlives[id] = DateTime.Now.Millisecond;
                }
            } ), new ReliableStrategy(0.1f, 20));

            cm.RegisterChannel((int) RegisteredChannels.KeepAliveChannel, channel);
            _keepAlives[id] = channel;
            _lastKeepAlives[id] = DateTime.Now.Millisecond;
        }

        void RemovePlayer(int id) {
            if (_keepAlives.ContainsKey(id)) {
                _keepAlives[id].Dispose();
                _keepAlives.Remove(id);
                _lastKeepAlives.Remove(id);
            }
        }

        private void Update() {
            var dced = new List<int>();
            foreach (var lastKeepAlive in _lastKeepAlives) {
                if (DateTime.Now.Millisecond - lastKeepAlive.Value > 5000) {
                    dced.Add(lastKeepAlive.Key);
                }
            }
            
            foreach (var i in dced) {
                RemovePlayer(i);
                ServerGameManager.Instance.RemovePlayer(i);
            }
        }

        private void OnDestroy() {
            foreach (var keepAliveChannel in _keepAlives) {
                keepAliveChannel.Value.Dispose();
            }
        }
    }
}