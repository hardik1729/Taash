﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distribution : MonoBehaviour
{
    // Start is called before the first frame update
    string[] Cards=new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7","S6","S5","S4","S3","S2",
						"HA","HK","HQ","HJ","H10","H9","H8","H7","H6","H5","H4","H3","H2",
						"CA","CK","CQ","CJ","C10","C9","C8","C7","C6","C5","C4","C3","C2",
						"DA","DK","DQ","DJ","D10","D9","D8","D7","D6","D5","D4","D3","D2"};

	float TableCardWidth=100;
	float TableCardHeight=150;

    GameObject TableObject;
    GameObject UserObject;
    bool activeDistribute=true;
    bool distribution=false;
    List<string> PlayCards=new List<string>();

    void Start()
    {
    	TableObject=GameObject.Find("TableObject");
    	UserObject=GameObject.Find("UserObject");
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetString("Recieved")!=""){
        	string message=PlayerPrefs.GetString("Recieved");
        	if(message=="ClearAll"){
    			ClearAll();
				InitialPlayCards();
    			PlayerPrefs.SetString("Recieved","");
    		}else if(message.Split(':').ToList()[0]=="PlayCards"){
    			PlayCards=new List<string>(message.Split(':').ToList()[1].Split(',').ToList());
    			PlayerPrefs.SetString("Recieved","");
    		}else if(message=="activeDistribute"){
    			activeDistribute=!activeDistribute;
    			if(activeDistribute){
    				foreach (Transform child in GameObject.Find("TableObject").transform)
					    Destroy(child.gameObject);
    			}
    			PlayerPrefs.SetString("Recieved","");
    		}else if(message.Split(':').ToList()[0]=="Distribute"){
    			Distribute(message.Split(':').ToList()[1]);
    			PlayerPrefs.SetString("Recieved","");
    		}
    	}

    	if(distribution && !GameObject.Find("Tick").GetComponent<Button>().enabled && PlayCards.Count==0){
			GameObject.Find("Tick").GetComponent<Button>().enabled=true;
		}
    }

    public void ClearAll(){
    	foreach (Transform child in TableObject.transform)
		    Destroy(child.gameObject);
		foreach (Transform child in UserObject.transform)
		    Destroy(child.gameObject);
	    foreach (Transform child in GameObject.Find("VirtualUserObject").transform)
	    	Destroy(child.gameObject);
		activeDistribute=true;
    	distribution=false;
		PlayCards.Clear();
		PlayerPrefs.SetString("UserCards","Start");
		PlayerPrefs.SetInt("Collected",-1);
		PlayerPrefs.SetInt("CardNumSwap",-1);
		PlayerPrefs.SetString("SelectedTableCard","");
		PlayerPrefs.SetString("LastMessage","");
    }

    private IEnumerator SmoothLerpCreate (float time, GameObject child)
    {
	    Vector3 startingPos  = child.transform.localPosition;
	    Vector3 finalPos = new Vector3(0,0,0);
	    float elapsedTime = 0;
	    
	    while (elapsedTime <= time)
	    {
	        child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
	        if(time==elapsedTime){
	        	Destroy(child);
	        	if(PlayerPrefs.GetString("UserCards")=="Start")
	        		PlayerPrefs.SetString("UserCards",TempUserCards[0]);
	        	else
	        		PlayerPrefs.SetString("UserCards",PlayerPrefs.GetString("UserCards")+","+TempUserCards[0]);
    			TempUserCards.RemoveAt(0);
	        	yield break;
	        }
	        elapsedTime += Time.deltaTime;
	        if(time-elapsedTime<0.01)
	        	elapsedTime=time;
	        yield return null;
	    }
    }

    private IEnumerator SmoothLerpDestroy (float time, GameObject child)
    {
	    Vector3 startingPos  = child.transform.localPosition;
	    Vector3 finalPos = new Vector3(0,0,0);
	    float elapsedTime = 0;
	    
	    while (elapsedTime <= time)
	    {
	        child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
	        if(time==elapsedTime){
	        	Destroy(child);
	        	yield break;
	        }
	        elapsedTime += Time.deltaTime;
	        if(time-elapsedTime<0.01){
	        	elapsedTime=time;
	        }
	        yield return null;
	    }
    }

    public void AllDistribute(){
    	string message="";
    	int n=PlayCards.Count;
    	while(n>0){
    		message=message+"Distribute:"+PlayerPrefs.GetString("User");
    		n=n-PlayerPrefs.GetString("Players").Split(',').ToList().Count;
    		if(n>0)
    			message=message+";";
    	}
    	PlayerPrefs.SetString("Send",message);
    	GameObject.Find("Tick").GetComponent<Button>().enabled=false;
    }

    public void tickDistribute(){
    	distribution=false;
    	PlayerPrefs.SetString("Send","activeDistribute"+";DisplayTable");
    	PlayCards.Clear();
    	foreach (Transform child in TableObject.transform)
		    Destroy(child.gameObject);
    	Destroy(GameObject.Find("ShareAll"));
    	Destroy(GameObject.Find("Tick"));
    }

    public void clickDistribute(){
    	if(activeDistribute){
			GameObject Tick=new GameObject("Tick");
			Tick.transform.SetParent(UserObject.transform,false);
			Button TickBtn = Tick.AddComponent<Button>() as Button;
			Image TickImage=Tick.AddComponent<Image>() as Image;
			Texture2D SpriteTick = Resources.Load<Texture2D>("Done");				
			Sprite TickSprite = Sprite.Create(SpriteTick, new Rect(0, 0, SpriteTick.width, SpriteTick.height),new Vector2(0,0),1);
			TickImage.sprite=TickSprite;
			TickBtn.image=TickImage;
			TickBtn.onClick.AddListener(delegate { tickDistribute(); });
			RectTransform rectTransform = TickBtn.GetComponent<RectTransform>();
	    	rectTransform.localPosition = new Vector3(0, 160, 0);
	    	rectTransform.sizeDelta = new Vector2(100, 70);
	    	
	    	GameObject ShareAll=new GameObject("ShareAll");
			ShareAll.transform.SetParent(UserObject.transform,false);
			Button ShareAllBtn = ShareAll.AddComponent<Button>() as Button;
			Image ShareAllImage=ShareAll.AddComponent<Image>() as Image;
			Texture2D SpriteShareAll = Resources.Load<Texture2D>("ShareAll");				
			Sprite ShareAllSprite = Sprite.Create(SpriteShareAll, new Rect(0, 0, SpriteShareAll.width, SpriteShareAll.height),new Vector2(0,0),1);
			ShareAllImage.sprite=ShareAllSprite;
			ShareAllBtn.image=ShareAllImage;
			ShareAllBtn.onClick.AddListener(delegate { AllDistribute(); });
			RectTransform SArectTransform = ShareAllBtn.GetComponent<RectTransform>();
	    	SArectTransform.localPosition = new Vector3(0, 320, 0);
	    	SArectTransform.sizeDelta = new Vector2(100, 100);

	    	ShufflePlayCards();
	    	PlayerPrefs.SetString("Send","PlayCards:"+string.Join(",",PlayCards.ToArray())+";activeDistribute");
	    	distribution=true;
    	}else if(distribution){
    		PlayerPrefs.SetString("Send","Distribute:"+PlayerPrefs.GetString("User"));
    	}
    }

    List<string> TempUserCards=new List<string>();

    public void Distribute(string distributor){
    	List<string> players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
    	int i=players.IndexOf(distributor);
    	int k=players.IndexOf(PlayerPrefs.GetString("User"));
    	for(int j=1;j<=players.Count && TableObject.transform.childCount!=0;j++){
    		if((i+j)%players.Count==k){
    			GameObject child=TableObject.transform.GetChild(TableObject.transform.childCount-1).gameObject;
    			TempUserCards.Add(PlayCards[TableObject.transform.childCount-1]);
    			PlayCards.RemoveAt(TableObject.transform.childCount-1);
    			Vector3 TempPosition=TableObject.transform.GetChild(0).position;
    			child.transform.SetParent(UserObject.transform,false);
    			child.transform.position=TempPosition;
    			StartCoroutine (SmoothLerpCreate(0.5f,child));
			}else{
				GameObject child=TableObject.transform.GetChild(TableObject.transform.childCount-1).gameObject;
				PlayCards.RemoveAt(TableObject.transform.childCount-1);
				Vector3 TempPosition=TableObject.transform.GetChild(0).position;
				child.transform.SetParent(GameObject.Find(players[(i+j)%players.Count]).transform,false);
				child.transform.position=TempPosition;
				StartCoroutine (SmoothLerpDestroy(0.5f,child));
			}
    	}
    }

    public void InitialPlayCards(){
    	List<string> decks=new List<string>(PlayerPrefs.GetString("Cards").Split(',').ToList());
    	foreach(string deck in decks){
    		for(int i=0;i<deck.Length;i++){
    			if(deck[i]=='1'){
    				PlayCards.Add(Cards[i]);
    			}
    		}
    	}
    	for(int i=0;i<PlayCards.Count;i++){
			string name="CardBack";
			GameObject c=new GameObject("T"+i);
			c.transform.SetParent(TableObject.transform,false);
			Button btn = c.AddComponent<Button>() as Button;
			Image img = c.AddComponent<Image>() as Image;
			Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			img.sprite = NewSprite;
			btn.image=img;
			btn.onClick.AddListener(delegate { clickDistribute(); });
			RectTransform rectTransform = btn.GetComponent<RectTransform>();
        	rectTransform.localPosition = new Vector3(0, 237.5F, 0);
        	rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);
		}
    }

    public void ShufflePlayCards(){
    	System.Random ran = new System.Random();
    	int n=PlayCards.Count;
    	while (n > 1) {
    		n--;
    		int k=ran.Next(n+1);
    		string value=PlayCards[k];
    		PlayCards[k]=PlayCards[n];
    		PlayCards[n]=value;
    	}
    }
}