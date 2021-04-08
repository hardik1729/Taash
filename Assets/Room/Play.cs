using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Play : MonoBehaviour
{
	// Start is called before the first frame update

	void Start()
	{
		PlayerPrefs.SetString("UserCards","Start");
		PlayerPrefs.SetInt("Collected",-1);
		PlayerPrefs.SetInt("CardNumSwap",-1);
		PlayerPrefs.SetString("SelectedTableCard","");
		PlayerPrefs.SetString("LastActiveTableCard","");
		PlayerPrefs.SetString("LastActiveTableCardColor","");
		PlayerPrefs.SetString("LastMessage","");
	}

	public void onRestart(){
		PlayerPrefs.SetString("Send","ClearAll"+";"+"GetIn"+";"+"Cards:"+PlayerPrefs.GetString("Cards")+";"+"Order:"+GameObject.Find("players").GetComponent<Text>().text);
	}
}
