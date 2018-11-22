using System.Collections.Generic;
using System.Net;
using ChannelNS;
using UnityEngine;
using Utils;

namespace ServerNS {
    public class ChannelManager {
        private readonly Dictionary<GenericChannel, byte> _channelNumbers = new Dictionary<GenericChannel, byte>();
        private readonly Dictionary<byte, GenericChannel> _channels = new Dictionary<byte, GenericChannel>();
        private UDPConnection _connection;
        private IPAddress ip;
        private int port;

        public ChannelManager(UDPConnection udpConnection, IPAddress ip, int port) {
            _connection = udpConnection;
            this.ip = ip;
            this.port = port;
        }

        public void RegisterChannel(byte number, GenericChannel channel) {
            Debug.LogWarning("REGISTERING CHANNEL" + number);
            _channelNumbers[channel] = number;
            _channels[number] = channel;
            channel.SetupSender(bytes => SendPacket(bytes, channel, ip, port));
        }

        private void SendPacket(byte[] bytes, GenericChannel channel, IPAddress ip, int port) {
            _connection.SendPacket(ArrayUtils.AddByteToArray(bytes, _channelNumbers[channel]), ip, port);
        }

        public void ReceivePacket(byte[] bytes) {
            var channelNumber = bytes[bytes.Length - 1];
            _channels[channelNumber].ReceivePackage(ArrayUtils.RemoveBytes(bytes, 1));
        }
    }
}