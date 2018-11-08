using System;
using System.Collections;
using System.Collections.Generic;
using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using SenderStrategyNS;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextProvider : MonoBehaviour {
	private EventChannel<TextMessage> _textChannel;

	private int nmber = 0;
	// Use this for initialization
	void Start () {
		_textChannel = new TextEventChannel((s) => { }, new TrivialStrategy());
//		SetupEverything.instance.sender.RegisterChannel(1, _textChannel);
		SetupEverything.instance.sender.RegisterChannel(_textChannel);
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.value < 0.1) {
			_textChannel.SendEvent(new TextMessage(nmber, "SARASA" + nmber));
			nmber++;
		}
	}
}

public class TextMessage {
	public int numberMessage;
	public String message;
	public TextMessage(int numberMessage, string message) {
		this.numberMessage = numberMessage;
		this.message = message;
	}
}
