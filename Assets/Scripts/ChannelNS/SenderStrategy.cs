using System;
using ChannelNS;

namespace ChannelNS {
    public interface ISenderStrategy {
        ISenderStrategy SetupListener(Action<byte[]> receiver);
        ISenderStrategy SetupSender(UDPSend sender);
        void SendPackage(byte[] bytes);
        void ReceivePackage(byte[] bytes);
    }
}

