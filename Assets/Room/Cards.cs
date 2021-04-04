using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cards : MonoBehaviour
{
    // Start is called before the first frame update
    string[] cards=new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7","S6","S5","S4","S3","S2",
						"HA","HK","HQ","HJ","H10","H9","H8","H7","H6","H5","H4","H3","H2",
						"CA","CK","CQ","CJ","C10","C9","C8","C7","C6","C5","C4","C3","C2",
						"DA","DK","DQ","DJ","D10","D9","D8","D7","D6","D5","D4","D3","D2"};

    GameObject TableObject;
    GameObject UserObject;
    Scrollbar s;
    Button Next;
    Button Prev;

    void Start()
    {
        TableObject=GameObject.Find("TableObject");
        UserObject=GameObject.Find("UserObject");

        s=GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
	    Next=GameObject.Find("Next").GetComponent<Button>();
	    Prev=GameObject.Find("Previous").GetComponent<Button>();
    }

    float transparentColor=0;
	float translucentColor1=0.25F;
	float translucentColor2=0.5F;
	float opaqueColor=1;

	float startTime=0;
    float holdTime=0.5F;
    bool LongPressCard=false;
    bool Decision=false;
    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetString("UserCards")=="Start" || PlayerPrefs.GetString("UserCards")=="" || PlayerPrefs.GetString("UserCards").Split(',').ToList().Count!=Count){
        	if(PlayerPrefs.GetString("UserCards")!="Start" && PlayerPrefs.GetString("UserCards")!=""){
	        	destroyCards();
	        	displayCards();
	        }else{
	        	destroyCards();
	        	Count=0;
	        	count=0;
	        	s.value=0;
	        }
        }
        if(Input.GetKeyDown(KeyCode.Mouse0)
			&& EventSystem.current.currentSelectedGameObject
         	&& EventSystem.current.currentSelectedGameObject.name.Length<4
         	&& Time.time-startTime>holdTime){
			startTime=Time.time;
			Decision=true;
			LongPressCard=true;
		}
		if(Time.time-startTime>0 && Time.time-startTime<holdTime && LongPressCard){
			if(LongPressCard && !Input.GetKey(KeyCode.Mouse0)){
				LongPressCard=false;
			}
		}else if(Time.time-startTime>0 && Decision){
			if(LongPressCard){
				List<string> UserCards=PlayerPrefs.GetString("UserCards").Split(',').ToList().OrderBy(x => Array.IndexOf(cards,x)).ToList();
				string combinedString = string.Join( ",", UserCards.ToArray() );
				PlayerPrefs.SetString("UserCards",combinedString);
				Drag();
				LongPressCard=false;
			}
			Decision=false;
		}
		if(updateNav){
    		updateNav=false;
	        if(Count>MaxCards){
	        	PlayerPrefs.SetString("BannerAd","No");
	        	Next.enabled=true;
	        	Prev.enabled=true;
	        	s.enabled=true;
				if(s.value>=1.0F*0/(Count-count+1) && s.value<=1.0F*(1)/(Count-count+1)){
					Image imgP=Prev.image;
					Color colorP=imgP.color;
					colorP.a=translucentColor1;
					imgP.color=colorP;
				}else{
					Image imgP=Prev.image;
					Color colorP=imgP.color;
					colorP.a=opaqueColor;
					imgP.color=colorP;
				}
				if(s.value>=1.0F*(Count-count)/(Count-count+1) && s.value<=1.0F*(Count-count+1)/(Count-count+1)){
					Image imgN=Next.image;
					Color colorN=imgN.color;
					colorN.a=translucentColor1;
					imgN.color=colorN;
				}else{
					Image imgN=Next.image;
					Color colorN=imgN.color;
					colorN.a=opaqueColor;
					imgN.color=colorN;
				}
				Image imgH=GameObject.Find("Handle").GetComponent<Image>();
				Color colorH=imgH.color;
				colorH.a=opaqueColor;
				imgH.color=colorH;
				Image imgS=GameObject.Find("Scrollbar").GetComponent<Image>();
				Color colorS=imgS.color;
				colorS.a=opaqueColor;
				imgS.color=colorS;
		    }else{
		    	PlayerPrefs.SetString("BannerAd","Yes");
		    	Next.enabled=false;
				Image imgN=Next.image;
				Color colorN=imgN.color;
				colorN.a=transparentColor;
				imgN.color=colorN;
				Prev.enabled=false;
				Image imgP=Prev.image;
				Color colorP=imgP.color;
				colorP.a=transparentColor;
				imgP.color=colorP;
				s.enabled=false;
				Image imgH=GameObject.Find("Handle").GetComponent<Image>();
				Color colorH=imgH.color;
				colorH.a=transparentColor;
				imgH.color=colorH;
				Image imgS=GameObject.Find("Scrollbar").GetComponent<Image>();
				Color colorS=imgS.color;
				colorS.a=transparentColor;
				imgS.color=colorS;
			}
		}
    }

    float CardWidth=120;
	float CardHeight=180;
	float gap=45;
	int MaxCards=13;

    int Count=0;
	int count=0;
	float start;
    bool updateNav=true;

    void destroyCards(){
    	foreach (Transform child in UserObject.transform){
		    if(child.gameObject.name[0]=='C' && child.gameObject.name.Length<=3)
		    	Destroy(child.gameObject);
		}
    	updateNav=true;
    }

    void displayCards(){
    	List<string> UserCards=PlayerPrefs.GetString("UserCards").Split(',').ToList();
		Count=UserCards.Count;
		start=0;
		count=Count;
		PlayerPrefs.SetInt("CardNumSwap",-1);

    	if(count>MaxCards){
			count=MaxCards;
			s.size=1.0F*count/Count;
		}else{
			s.size=1;
		}
		if(count%2==0)
			start=gap/2+(count/2-1)*gap;
		else if(count!=1)
			start=gap*(count-1)/2;
		start=start*(-1);

		int i=Location();
    	if(i!=-1){
    		int j=0;
			foreach(string card in UserCards){
				if(j>=i){
	    			int idx=j-i;
					string name="";
					if(card.Length==2)
						name=name+card[1]+card[0];
					else
						name=name+card[1]+card[2]+card[0];
					GameObject c=new GameObject("C"+idx);
					c.transform.SetParent(UserObject.transform,false);
					Button btn = c.AddComponent<Button>() as Button;
					Image img = c.AddComponent<Image>() as Image;
					Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
					float loc=start+gap*idx;
					Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
					img.sprite = NewSprite;
					btn.image=img;
					btn.onClick.AddListener(delegate { onClick(); });
					RectTransform rectTransform = btn.GetComponent<RectTransform>();
		        	rectTransform.localPosition = new Vector3(loc, 0, 0);
		        	rectTransform.sizeDelta = new Vector2(CardWidth, CardHeight);
					idx=idx+1;
					if(idx==MaxCards)
						break;
				}
				j=j+1;
			}
		}
		updateNav=true;
    }

    public void onClick(){
    	List<string> UserCards=PlayerPrefs.GetString("UserCards").Split(',').ToList();
    	int i=Location();
    	int idx;
    	string name=EventSystem.current.currentSelectedGameObject.name;
    	if(name.Length==3){
    		idx=10+(name[2]-48);
    	}else{
    		idx=name[1]-48;
    	}
    	if(PlayerPrefs.GetInt("CardNumSwap")!=-1)
    	{
    		StartCoroutine(PlayAudio("PlayCard"));
    		string Card1=UserCards[PlayerPrefs.GetInt("CardNumSwap")];
    		string Card2=UserCards[idx+i];
    		UserCards[PlayerPrefs.GetInt("CardNumSwap")]=Card2;
    		UserCards[idx+i]=Card1;
    		PlayerPrefs.SetInt("CardNumSwap",-1);
    		string combinedString = string.Join( ",", UserCards.ToArray() );
			PlayerPrefs.SetString("UserCards",combinedString);
    		Drag();
    	}else{
    		StartCoroutine(PlayAudio("PlayCard"));
    		PlayerPrefs.SetInt("CardNumSwap",idx+i);
    		GameObject c=GameObject.Find(name);
			Button btn = c.GetComponent<Button>();
			RectTransform rectTransform = btn.GetComponent<RectTransform>();
			rectTransform.Translate(0,50,0);
    	}
    }

    public void onNext(){
    	if(Next.image.color.a==opaqueColor){
    		StartCoroutine(PlayAudio("PlayCard"));
    		Shift(true);
    		updateNav=true;
    	}
    }
    
    public void onPrev(){
    	if(Prev.image.color.a==opaqueColor){
    		StartCoroutine(PlayAudio("PlayCard"));
    		Shift(false);
    		updateNav=true;
    	}
    }
    
    int Location(){
    	int i=0;
    	for(;i<Count-count+1;i++){
    		if(s.value>=1.0F*i/(Count-count+1) && s.value<=1.0F*(i+1)/(Count-count+1)){
    			break;
    		}
    	}
    	if(i==Count-count+1)
    		return -1;
    	return i;
    }
    
    void Shift(bool x){
    	int i=Location();
    	if(x){
    		i=i+1;
    	}else{
    		i=i-1;
    	}
    	s.value=(1.0F*i/(Count-count+1)+1.0F*(i+1)/(Count-count+1))/2;
    	Drag();
    	updateNav=true;
    }
    
    public void Drag(){
    	List<string> UserCards=PlayerPrefs.GetString("UserCards").Split(',').ToList();
    	int i=Location();
    	if(i!=-1){
    		int j=0;
	    	foreach(string card in UserCards){
	    		if(j>=i){
	    			int idx=j-i;
	    			string name="";
					if(card.Length==2)
						name=name+card[1]+card[0];
					else
						name=name+card[1]+card[2]+card[0];
					GameObject c=GameObject.Find("C"+idx);
					Button btn = c.GetComponent<Button>();
					Image img = c.GetComponent<Image>();
					Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
					float SpriteWidth=SpriteTexture.width;
					float loc=start+gap*idx;
					Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
					img.sprite = NewSprite;
					btn.image=img;
					RectTransform rectTransform = btn.GetComponent<RectTransform>();
					rectTransform.localPosition = new Vector3(loc, 0, 0);
					if(PlayerPrefs.GetInt("CardNumSwap")==j)
						rectTransform.Translate(0,50,0);
					idx=idx+1;
					if(idx==MaxCards)
						break;
		    	}
		    	j=j+1;
	    	}
	    	updateNav=true;
	  	}
    }

    private IEnumerator PlayAudio (string name)
    {
        AudioSource audio = GameObject.Find(name).GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
