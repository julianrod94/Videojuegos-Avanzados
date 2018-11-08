using System.Collections;
using System.Collections.Generic;
using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using SenderStrategyNS;
using UnityEngine;
using UnityEngine.UI;

public class TextReceiver : MonoBehaviour {
	private EventChannel<TextMessage> _textChannel;

	private string currentString;

	private Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		_textChannel = new TextEventChannel(
			data => { currentString = data.message; }, new TrivialStrategy());
		SetupEverything.instance.receiver.RegisterChannel(_textChannel);
//		SetupEverything.instance.receiver.RegisterChannel(1, _textChannel);
	}
	
	// Update is called once per frame
	void Update () {
		text.text = currentString;
	}
}
