/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceive : MonoBehaviour {
    public string allReceivedUDPPackets = ""; // clean up this from time to time!

    // udpclient object
    private UdpClient client;

    // infos
    public string lastReceivedUDPPacket = "";

    // public
    // public string IP = "127.0.0.1"; default local
    public int port; // define > init

    // receiving Thread
    private Thread receiveThread;


    // start from shell
    private static void Main() {
        var receiveObj = new UDPReceive();
        receiveObj.init();

        var text = "";
        do {
            text = Console.ReadLine();
        } while (!text.Equals("exit"));
    }

    // start from unity3d
    public void Start() {
        init();
    }

    // OnGUI
    private void OnGUI() {
        var rectObj = new Rect(40, 10, 200, 400);
        var style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                         + "shell> nc -u 127.0.0.1 : " + port + " \n"
                         + "\nLast Packet: \n" + lastReceivedUDPPacket
                         + "\n\nAll Messages: \n" + allReceivedUDPPackets
            , style);
    }

    // init
    private void init() {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData() {
        client = new UdpClient(port);
        while (true)
            try {
                // Bytes empfangen.
                var anyIP = new IPEndPoint(IPAddress.Any, 0);
                var data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                var text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                // ....
                allReceivedUDPPackets = allReceivedUDPPackets + text;
            } catch (Exception err) {
                print(err.ToString());
            }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket() {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}