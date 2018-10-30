using System;
using System.Collections.Generic;
using System.Linq;
using ChannelNS;

namespace ServerNS {
    public class Server {
        private readonly Dictionary<GenericChannel, byte> _channelNumbers = new Dictionary<GenericChannel, byte>();
        private readonly Dictionary<byte, GenericChannel> _channels = new Dictionary<byte, GenericChannel>();
        private byte _currentChannels;
        private Server _other;

        public void RegisterChannel(GenericChannel channel) {
            _channelNumbers[channel] = _currentChannels;
            _channels[_currentChannels] = channel;
            channel.SetupSender(bytes => SendPacket(bytes, channel));
            _currentChannels++;
        }

        public void SetupOtherServer(Server other) {
            _other = other;
        }

        private void SendPacket(byte[] bytes, GenericChannel channel) {
            var modifiedArray = AddByteToArray(bytes, _channelNumbers[channel]);
            _other.ReceivePacket(modifiedArray);
        }

        private void ReceivePacket(byte[] bytes) {
            var channelNumber = bytes[bytes.Length - 1];
            var newArray = new byte[bytes.Length -1];
            Buffer.BlockCopy(bytes,0,newArray,0,bytes.Length - 1);
            _channels[channelNumber].ReceivePackage(newArray);
        }

        private byte[] AddByteToArray(byte[] bArray, byte newByte) {
            var newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[bArray.Length] = newByte;
            return newArray;
        }
    }
}