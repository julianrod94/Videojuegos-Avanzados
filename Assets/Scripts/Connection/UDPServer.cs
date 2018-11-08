using System;
using System.Net;
using System.Threading;

public class Server : UDPConnection {
    private Thread listenThread;

    public Server(Action<IPAddress, int, byte[]> passPacket) : base(passPacket) {
    }

    public void SetupServer(int port) {
        listenThread = new Thread(Listen);
        listenThread.IsBackground = true;
        listenThread.Start();
    }

    // OnGUI
//    void OnGUI()
//    {
//        Rect rectObj=new Rect(40,10,200,400);
//            GUIStyle style = new GUIStyle();
//                style.alignment = TextAnchor.UpperLeft;
//        GUI.Box(rectObj,"# UDPReceive\n127.0.0.1 "+port+" #\n"
//                    + "shell> nc -u 127.0.0.1 : "+port+" \n"
//                    + "\nLast Packet: \n"+ lastReceivedUDPPacket
//                    + "\n\nAll Messages: \n"+allReceivedUDPPackets
//                ,style);
//    }
 
}