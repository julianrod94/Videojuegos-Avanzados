using System;

namespace ChannelNS {
    public interface ISenderStrategy {
        void SetupListener(Action<byte[]> receiver);
        void SetupSender(Action<byte[]> sender);
        void SendPackage(byte[] bytes);
        void ReceivePackage(byte[] bytes);
    }
}