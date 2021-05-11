using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpPlay : MonoBehaviour
{
	string[] BtnName=new string[]{"Restart","Discard","ShareAll","Keep","Upload","Undo"};
	string[] BtnDes=new string[]{"Restart","Delete Cards","Share All Cards","Start Playing","Show","Undo"};
	int BtnPos=0;
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
		StateCircleRectTransform.localPosition = new Vector3(340, -340, 0);
		StateCircleRectTransform.sizeDelta = new Vector2(130, 150);

		GameObject StateText=new GameObject("StateText");
    	StateText.transform.SetParent(GameObject.Find("StateCircle").transform,false);
    	Text text=StateText.AddComponent<Text>() as Text;
        text.fontSize=30;
        text.color=Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text=BtnDes[BtnPos];
        RectTransform StateTextRectTransform = text.GetComponent<RectTransform>();
    	StateTextRectTransform.localPosition = new Vector3(0, 80, 0);
    	StateTextRectTransform.sizeDelta = new Vector2(300, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.currentSelectedGameObject){
        	BtnPos=(BtnPos+1)%BtnName.Length;
        	while(GameObject.Find(BtnName[BtnPos])==null)
        		BtnPos=(BtnPos+1)%BtnName.Length;
        	HelpDisplay();
        }
    }

    void HelpDisplay(){
    	GameObject StateCircle=GameObject.Find("StateCircle");
    	GameObject StateText=GameObject.Find("StateText");
    	switch(BtnPos){
    		case 0:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(340, -340, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(130, 150);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
    			break;
    		case 1:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(57.5F, 82.5F+237.5F, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 75);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0);
			    break;
			case 2:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(0, -500+320, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
			    break;
			case 3:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(0, -500+160, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
			    break;
			case 4:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(-340, -340, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(130,150);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
    			break;
    		case 5:
				StateCircle.GetComponent<RectTransform>().localPosition = new Vector3(-340, -340, 0);
				StateCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(130,150);

			    StateText.GetComponent<Text>().text=BtnDes[BtnPos];
			    StateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 80, 0);
    			break;
			default:
    			break;
    	}
    }
}
