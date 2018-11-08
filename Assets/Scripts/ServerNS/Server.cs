using System.Collections.Generic;
using ChannelNS;
using Utils;

namespace ServerNS {
    public class Server {
        private readonly Dictionary<GenericChannel, byte> _channelNumbers = new Dictionary<GenericChannel, byte>();
        private readonly Dictionary<byte, GenericChannel> _channels = new Dictionary<byte, GenericChannel>();
        private Server _other;

        public void RegisterChannel(byte number, GenericChannel channel) {
            _channelNumbers[channel] = number;
            _channels[number] = channel;
            channel.SetupSender(bytes => SendPacket(bytes, channel));
        }

        public void SetupOthererver(Server other) {
            _other = other;
        }

        private void SendPacket(byte[] bytes, GenericChannel channel) {
            _other.ReceivePacket(ArrayUtils.AddByteToArray(bytes, _channelNumbers[channel]));
        }

        private void ReceivePacket(byte[] bytes) {
            var channelNumber = bytes[bytes.Length - 1];
            _channels[channelNumber].ReceivePackage(ArrayUtils.RemoveBytes(bytes, 1));
        }
    }
}