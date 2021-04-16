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
		
	}

	public void onRestart(){
		PlayerPrefs.SetString("Send","ClearAll"+";"+"GetIn"+";"+"Cards:"+PlayerPrefs.GetString("Cards")+";"+"Order:"+GameObject.Find("players").GetComponent<Text>().text);
	}
}
