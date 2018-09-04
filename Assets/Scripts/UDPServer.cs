using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		StartServer();
	}

	// Update is called once per frame
	void Update () {
		
	}
	
	private void StartServer()
	{
		
		
		new Thread(() => 
		{
			Thread.CurrentThread.IsBackground = true; 
			UdpClient udpServer = new UdpClient(11000);
			while (true)
			{
				Debug.Log("dasdas");
				var remoteEP = new IPEndPoint(IPAddress.Any, 11000); 
				var data = udpServer.Receive(ref remoteEP); // listen on port 11000
				Debug.Log("receive data from " + remoteEP);
				udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
			}
		}).Start();
       
	}
}
