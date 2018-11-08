using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSurvived : MonoBehaviour {
	private Text text;
	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		text = GetComponent<Text>();
		print(ScenesOrganizer.Instance);
		text.text = "Time survived: " + ScenesOrganizer.Instance.getFormattedLastTime();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
