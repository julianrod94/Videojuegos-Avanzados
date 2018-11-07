﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ChannelNS;
using Utils;

public class ChannelManager {
    private readonly Dictionary<GenericChannel, byte> _channelNumbers = new Dictionary<GenericChannel, byte>();
    private readonly Dictionary<byte, GenericChannel> _channels = new Dictionary<byte, GenericChannel>();
    private byte _currentChannels;
    private UDPConnection _connection;
    private IPAddress ip;
    private int port;

    public ChannelManager(UDPConnection udpConnection, IPAddress ip, int port) {
        _connection = udpConnection;
        this.ip = ip;
        this.port = port;
    }

    public void RegisterChannel(GenericChannel channel) {
        _channelNumbers[channel] = _currentChannels;
        _channels[_currentChannels] = channel;
        channel.SetupSender(bytes => SendPacket(bytes, channel, ip, port));
        _currentChannels++;
    }

    private void SendPacket(byte[] bytes, GenericChannel channel, IPAddress ip, int port) {
        _connection.SendPacket(ArrayUtils.AddByteToArray(bytes, _channelNumbers[channel]), ip, port);
    }

    public void ReceivePacket(byte[] bytes) {
        var channelNumber = bytes[bytes.Length - 1];
        _channels[channelNumber].ReceivePackage(ArrayUtils.RemoveBytes(bytes, 1));
    }
}