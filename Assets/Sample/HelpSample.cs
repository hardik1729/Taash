using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpSample : MonoBehaviour
{
	string[] stateText=new string[]{"Create a Room","Connect to a Room"};
	int[] stateY=new int[]{0,-125}; 
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
		StateCircleRectTransform.localPosition = new Vector3(0, stateY[statePos], 0);
		StateCircleRectTransform.sizeDelta = new Vector2(500, 125);

		GameObject StateText=new GameObject("StateText");
    	StateText.transform.SetParent(GameObject.Find("StateCircle").transform,false);
    	Text text=StateText.AddComponent<Text>() as Text;
        text.fontSize=30;
        text.color=Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text=stateText[statePos];
        RectTransform StateTextRectTransform = text.GetComponent<RectTransform>();
    	StateTextRectTransform.localPosition = new Vector3(0, 75, 0);
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
		StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(0, stateY[statePos], 0);

		GameObject StateText=GameObject.Find("StateText");
        StateText.GetComponent<Text>().text=stateText[statePos];
    }
}
