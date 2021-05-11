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
		PlayerPrefs.SetInt("HelpState",0);
	}

	public void onRestart(){
		PlayerPrefs.SetString("Send","ClearAll"+";"+"GetIn"+";"+"Cards:"+PlayerPrefs.GetString("Cards")+";"+"Order:"+GameObject.Find("players").GetComponent<Text>().text);
	}

	private IEnumerator PlayAudio ()
	{
		AudioSource audio = GameObject.Find("Click").GetComponent<AudioSource>();
		audio.Play();
		yield return new WaitForSeconds(audio.clip.length);
	}

	public void help(){
		StartCoroutine(PlayAudio());
		if(PlayerPrefs.GetInt("HelpState")==1){
			PlayerPrefs.SetInt("HelpState",0);
			Image BImage=GameObject.Find("Help").GetComponent<Image>() as Image;
			Texture2D SpriteB = Resources.Load<Texture2D>("HelpOutlined");
			Sprite BSprite = Sprite.Create(SpriteB, new Rect(0, 0, SpriteB.width, SpriteB.height),new Vector2(0,0),1);
			BImage.sprite=BSprite;
			foreach (Transform child in GameObject.Find("HelpCenter").transform)
				Destroy(child.gameObject);
		}else{
			PlayerPrefs.SetInt("HelpState",1);
			Image BImage=GameObject.Find("Help").GetComponent<Image>() as Image;
			Texture2D SpriteB = Resources.Load<Texture2D>("HelpFilled");
			Sprite BSprite = Sprite.Create(SpriteB, new Rect(0, 0, SpriteB.width, SpriteB.height),new Vector2(0,0),1);
			BImage.sprite=BSprite;
			GameObject HelpObject=new GameObject("HelpObject");
			HelpObject.transform.SetParent(GameObject.Find("HelpCenter").transform,false);
			HelpObject.AddComponent<HelpPlay>();
		}
	}
}
