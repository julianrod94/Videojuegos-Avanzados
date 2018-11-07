using System;
using System.Net;
using System.Net.Sockets;

public class Client : UDPConnection {
    public Client(Action<IPAddress, int, byte[]> passPacket) : base(passPacket) {
    }

    public void Connect(string ServerIp, int ServerPort) {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
        client = new UdpClient(LocalPort);
    }


//    // OnGUI
//    void OnGUI() {
//        Rect rectObj = new Rect(40, 280, 200, 400);
//        GUIStyle style = new GUIStyle();
//        style.alignment = TextAnchor.UpperLeft;
//        GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + ServerPort + " #\n"
//                         + "shell> nc -lu 127.0.0.1  " + ServerPort + " \n"
//            , style);
//
//        // ------------------------
//        // send it
//        // ------------------------
//        strMessage = GUI.TextField(new Rect(40, 380, 140, 20), strMessage);
//        if (GUI.Button(new Rect(190, 380, 40, 20), "send")) {
//            sendString(strMessage + "\n");
//        }
//    }

    // inputFromConsole
//    private void inputFromConsole() {
//        try {
//            string text;
//            do {
//                text = Console.ReadLine();
//
//                // Den Text zum Remote-Client senden.
//                if (text != "") {
//                    // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
//                    byte[] data = Encoding.UTF8.GetBytes(text);
//
//                    // Den Text zum Remote-Client senden.
//                    client.Send(data, data.Length, remoteEndPoint);
//                }
//            } while (text != "");
//        }
//        catch (Exception err) {
//            print(err.ToString());
//        }
//
//    }
}