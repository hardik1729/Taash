using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpCreate : MonoBehaviour
{
	string[] stateText=new string[]{"Select a Card","Select All Cards", "Select Cards Upto 7", "Add More Decks", "Enter to Create Room"};
	int statePos=0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject StateCircle=new GameObject("StateCircle");
		StateCircle.transform.SetParent(GameObject.Find("HelpObject").transform,false);
		Image img=StateCircle.AddComponent<Image>() as Image;
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Circle");
		Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		img.sprite = NewSprite;
		RectTransform StateCircleRectTransform = StateCircle.GetComponent<RectTransform>();
		StateCircleRectTransform.localPosition = new Vector3(-335+1*(92.5F)+1*100, 550-(132.5F)*0, 0);
		StateCircleRectTransform.sizeDelta = new Vector2(125, 150);

		GameObject StateText=new GameObject("StateText");
    	StateText.transform.SetParent(GameObject.Find("StateCircle").transform,false);
    	Text text=StateText.AddComponent<Text>() as Text;
        text.fontSize=30;
        text.color=Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text=stateText[statePos];
        RectTransform StateTextRectTransform = text.GetComponent<RectTransform>();
    	StateTextRectTransform.localPosition = new Vector3(0, 80, 0);
    	StateTextRectTransform.sizeDelta = new Vector2(300, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.currentSelectedGameObject){
        	statePos=(statePos+1)%stateText.Length;
        	HelpDisplay();
        }
    }

    void HelpDisplay(){
    	GameObject StateCircle=GameObject.Find("StateCircle");
    	GameObject StateText=GameObject.Find("StateText");
    	switch(statePos){
    		case 0:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(-335+1*(92.5F)+1*100, 550-(132.5F)*0, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(125, 150);

			    StateText.GetComponent<Text>().text=stateText[statePos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
    			break;
    		case 1:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(-192.5F, -500-90, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			    StateText.GetComponent<Text>().text=stateText[statePos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, -80, 0);
			    break;
			case 2:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(192.5F, -500-90, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			    StateText.GetComponent<Text>().text=stateText[statePos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, -80, 0);
			    break;
			case 3:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(140, -500+10, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

			    StateText.GetComponent<Text>().text=stateText[statePos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, -40, 0);
			    break;
			case 4:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(0, -500-90, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			    StateText.GetComponent<Text>().text=stateText[statePos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, -80, 0);
			    break;
    		default:
    			break;
    	}
    }
}
