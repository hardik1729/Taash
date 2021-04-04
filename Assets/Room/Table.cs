using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Table : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject TableObject;
    GameObject UserObject;

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
    		if(message=="DisplayTable"){
    			displayTable();
    			PlayerPrefs.SetInt("Collected",0);
    			Collect=false;
    			PlayerPrefs.SetString("Recieved","");
    		}else if(message.Split(':').ToList()[0]=="Table"){
    			StartCoroutine(PlayAudio("PlayCard"));
	        	Tabulation(message);
	        	PlayerPrefs.SetString("LastMessage",message);
	        	PlayerPrefs.SetString("Recieved","");
	        }else if(message.Split(':').ToList()[0]=="Collect"){
	        	StartCoroutine(PlayAudio("PlayCard"));
	        	Collection(message);
	        	PlayerPrefs.SetString("LastMessage",message);
	        	PlayerPrefs.SetString("Recieved","");
	        }else if(message=="Undo"){
	        	StartCoroutine(PlayAudio("PlayCard"));
	        	Undo();
	        	PlayerPrefs.SetString("LastMessage","");
	        	PlayerPrefs.SetString("Recieved","");
	        }
        }

        if(PlayerPrefs.GetString("SelectedTableCard")=="" && PlayerPrefs.GetInt("Collected")!=-1){
			GameObject CollectedObject=GameObject.Find("Collected");
            CollectedObject.GetComponent<Text>().text="Collection : "+PlayerPrefs.GetInt("Collected");
            CollectedObject.GetComponent<RectTransform>().sizeDelta=new Vector2(800, 75);
		}else{
			GameObject CollectedObject=GameObject.Find("Collected");
            CollectedObject.GetComponent<Text>().text="Collection : "+PlayerPrefs.GetInt("Collected");
            CollectedObject.GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		}

		if(PlayerPrefs.GetInt("Collected")!=-1){
			GameObject.Find("Undo").GetComponent<RectTransform>().sizeDelta=new Vector2(90, 90);
		}else{
			GameObject.Find("Undo").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		}
		if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.currentSelectedGameObject){
			if(PlayerPrefs.GetString("SelectedTableCard")!=""){
		    	Image BRCOld=GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.GetChild(0).gameObject.GetComponent<Image>();
				Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
				Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
				BRCOld.sprite = SpriteOld;
				Color color=BRCOld.color;
				color.a=transparentColor;
				BRCOld.color=color;
				for(int i=1;i<GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount;i++){
					Destroy(GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")+"V"+(i-1)));
				}
				Destroy(GameObject.Find("Collect"));
				Collect=false;
				PlayerPrefs.SetString("SelectedTableCard","");
		    }
		}
    }

	int row=4;
	int col=5;
	float startX=-250;
	float startY=500;
	float TableCardWidth=100;
	float TableCardHeight=150;
	float ColWidth=125;
	float RowHeigth=175;
	float RectWidth=212.5F;
	float RectHeight=187.5F;
	float RectRotation=90;

	bool Collect=false;

	float transparentColor=0;
	float translucentColor1=0.25F;
	float translucentColor2=0.5F;
	float opaqueColor=1;

    void displayTable(){
    	for(int r=0;r<row;r++){
			for(int c=0;c<col;c++){
				GameObject rc=new GameObject("RC"+r+""+c);
				rc.transform.SetParent(TableObject.transform,false);
				Button btn = rc.AddComponent<Button>() as Button;
				Image img = rc.AddComponent<Image>() as Image;
				Texture2D SpriteTexture = Resources.Load<Texture2D>("Transparent");
				float locX=startX+c*ColWidth;
				float locY=startY-r*RowHeigth;
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
				img.sprite = NewSprite;
				btn.image=img;
				btn.onClick.AddListener(delegate { onTableCard(); });
				RectTransform rectTransform = btn.GetComponent<RectTransform>();
	        	rectTransform.localPosition = new Vector3(locX, locY, 0);
	        	rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);

				GameObject B=new GameObject("B"+"RC"+r+""+c);
				B.transform.SetParent(rc.transform,false);
				Image BImage=B.AddComponent<Image>() as Image;
				Texture2D SpriteB = Resources.Load<Texture2D>("RoundedRectangle");
				Sprite BSprite = Sprite.Create(SpriteB, new Rect(0, 0, SpriteB.width, SpriteB.height),new Vector2(0,0),1);
				RectTransform rectTransformB = BImage.GetComponent<RectTransform>();
	        	rectTransformB.localEulerAngles = new Vector3(0, 0, RectRotation);
	        	rectTransformB.sizeDelta = new Vector2(RectWidth, RectHeight);
				BImage.sprite=BSprite;
				Color color=BImage.color;
				color.a=transparentColor;
				BImage.color=color;
			}
		}
    }

    public void onTableCard(){
		string BtnName="";
		if(EventSystem.current.currentSelectedGameObject!=null){
			BtnName=EventSystem.current.currentSelectedGameObject.name;
			if(!(BtnName.Length==4 && BtnName[0]=='R' && BtnName[1]=='C')){
				BtnName="";
			}
		}
		if(BtnName!="" && PlayerPrefs.GetInt("CardNumSwap")!=-1){
			List<string> UserCards=PlayerPrefs.GetString("UserCards").Split(',').ToList();
			string name="";
			if(UserCards[PlayerPrefs.GetInt("CardNumSwap")].Length==2)
				name=name+UserCards[PlayerPrefs.GetInt("CardNumSwap")][1]+UserCards[PlayerPrefs.GetInt("CardNumSwap")][0];
			else if(UserCards[PlayerPrefs.GetInt("CardNumSwap")].Length==3)
				name=name+UserCards[PlayerPrefs.GetInt("CardNumSwap")][1]+UserCards[PlayerPrefs.GetInt("CardNumSwap")][2]+UserCards[PlayerPrefs.GetInt("CardNumSwap")][0];
			PlayerPrefs.SetString("Send","Table:"+PlayerPrefs.GetString("User")+":"+BtnName+":"+name);
			UserCards.RemoveAt(PlayerPrefs.GetInt("CardNumSwap"));
			PlayerPrefs.SetInt("CardNumSwap",-1);
			string combinedString = string.Join( ",", UserCards.ToArray() );
			PlayerPrefs.SetString("UserCards",combinedString);
		}else{
			if(PlayerPrefs.GetString("SelectedTableCard")!=""){
				Image BRCOld=GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.GetChild(0).gameObject.GetComponent<Image>();
				Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
				Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
				BRCOld.sprite = SpriteOld;
				Color color=BRCOld.color;
				color.a=transparentColor;
				BRCOld.color=color;
				for(int i=0;i<GameObject.Find("VirtualUserObject").transform.childCount;i++){
					Destroy(GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")+"V"+i));
				}
			}
			if(BtnName=="")
				BtnName=PlayerPrefs.GetString("SelectedTableCard");
			if(BtnName!=""){
				Image BRC=GameObject.Find(BtnName).transform.GetChild(0).gameObject.GetComponent<Image>();
				Texture2D SpriteTexture = Resources.Load<Texture2D>("RectangleOutlined");
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
				BRC.sprite = NewSprite;
				Color color=BRC.color;
				color.a=opaqueColor;
				BRC.color=color;

				float Vstart=0;
				float VCardWidth=37.5F;
				int Vcount=GameObject.Find(BtnName).transform.childCount-1;
				if(Vcount%2==0)
					Vstart=VCardWidth/2+(Vcount/2-1)*VCardWidth;
				else if(Vcount!=1)
					Vstart=VCardWidth*(Vcount-1)/2;
				Vstart=Vstart*(-1);
				for(int i=1;i<GameObject.Find(BtnName).transform.childCount;i++){
					Image img=GameObject.Find(BtnName).transform.GetChild(i).GetComponent<Image>();
					float loc=Vstart+VCardWidth*(i-1);
					GameObject v=new GameObject(BtnName+"V"+(i-1));
					v.transform.SetParent(GameObject.Find("VirtualUserObject").transform,false);
					Image Vimg = v.AddComponent<Image>() as Image;
					Vimg.sprite=img.sprite;
					RectTransform rectTransform = Vimg.GetComponent<RectTransform>();
		        	rectTransform.localPosition = new Vector3(loc, 0, 0);
		        	rectTransform.sizeDelta=new Vector2(TableCardWidth,TableCardHeight);
				}
				PlayerPrefs.SetString("SelectedTableCard",BtnName);
			}
			if(!Collect && PlayerPrefs.GetString("SelectedTableCard")!="" && GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount>1){
				GameObject collect=new GameObject("Collect");
				Collect=true;
				collect.transform.SetParent(UserObject.transform,false);
				Button CollectBtn = collect.AddComponent<Button>() as Button;
				Image CollectImg = collect.AddComponent<Image>() as Image;
				Texture2D SpriteCollect = Resources.Load<Texture2D>("Collect");				
				Sprite CollectSprite = Sprite.Create(SpriteCollect, new Rect(0, 0, SpriteCollect.width, SpriteCollect.height),new Vector2(0,0),1);
				CollectImg.sprite=CollectSprite;
				CollectBtn.image=CollectImg;
				CollectBtn.onClick.AddListener(delegate { Collected(); });
				RectTransform CollectrectTransform = CollectBtn.GetComponent<RectTransform>();
	        	CollectrectTransform.localPosition = new Vector3(0, 137.5F, 0);
	        	CollectrectTransform.sizeDelta = new Vector2(80, 80);
			}else if(Collect && (PlayerPrefs.GetString("SelectedTableCard")=="" || GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount<=1)){
				Destroy(GameObject.Find("Collect"));
				Collect=false;
			}
		}
	}
	
	public void Collected(){
		string message="Collect:"+PlayerPrefs.GetString("User")+":"+PlayerPrefs.GetString("SelectedTableCard")+":";
		for(int i=1;i<GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount;i++){
			GameObject child=GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.GetChild(i).gameObject;
			message=message+child.name.Split(':').ToList()[1];
			if(i!=GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount-1)
				message=message+",";
    	}
		PlayerPrefs.SetString("Send",message);
	}

	void Tabulation(string message){
    	GameObject rc=GameObject.Find(message.Split(':').ToList()[2]);
    	string senderName=message.Split(':').ToList()[1];
    	GameObject child=new GameObject(rc.name+rc.transform.childCount+":"+message.Split(':').ToList()[3]);
		child.transform.SetParent(rc.transform,false);
		if(senderName!=PlayerPrefs.GetString("User")){
    		child.transform.position=GameObject.Find(senderName).transform.position;
    	}else{
    		child.transform.position=UserObject.transform.position;
    	}
		Image img = child.AddComponent<Image>() as Image;
		Texture2D SpriteTexture = Resources.Load<Texture2D>(message.Split(':').ToList()[3]);
		Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		img.sprite = NewSprite;
		RectTransform rectTransform = img.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);	
        StartCoroutine (SmoothLerpCreate (0.2f,child));
    	if(message.Split(':').ToList()[0]=="Table")
    		onTableCard();
    }

    void Collection(string message){
    	GameObject rc=GameObject.Find(message.Split(':').ToList()[2]);
    	string senderName=message.Split(':').ToList()[1];
    	if(PlayerPrefs.GetString("SelectedTableCard")==rc.name){
			Image BRCOld=GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.GetChild(0).gameObject.GetComponent<Image>();
			Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
			Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
			BRCOld.sprite = SpriteOld;
			Color color=BRCOld.color;
			color.a=transparentColor;
			BRCOld.color=color;
			for(int i=1;i<GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")).transform.childCount;i++){
				Destroy(GameObject.Find(PlayerPrefs.GetString("SelectedTableCard")+"V"+(i-1)));
			}
			PlayerPrefs.SetString("SelectedTableCard","");
			Collect=false;
			Destroy(GameObject.Find("Collect"));
		}
		while(rc.transform.childCount!=1){
			GameObject child=rc.transform.GetChild(1).gameObject;
			if(senderName!=PlayerPrefs.GetString("User")){
        		child.transform.SetParent(GameObject.Find(senderName).transform,false);
        		child.transform.position=rc.transform.position;
        	}else{
        		child.transform.SetParent(UserObject.transform,false);
        		child.transform.position=rc.transform.position;
        	}
    		StartCoroutine (SmoothLerpDestroy (0.2f,child));
    	}
    	if(PlayerPrefs.GetString("User")==senderName)
    		PlayerPrefs.SetInt("Collected",PlayerPrefs.GetInt("Collected")+1);
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

    private IEnumerator SmoothLerpCreateUndoTable (float time, GameObject child,string name)
    {
	    Vector3 startingPos  = child.transform.localPosition;
	    Vector3 finalPos = new Vector3(0,0,0);
	    float elapsedTime = 0;
	    
	    while (elapsedTime <= time)
	    {
	        child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
	        if(time==elapsedTime){
	        	Destroy(child);
	        	if(PlayerPrefs.GetString("UserCards")=="")
	        		PlayerPrefs.SetString("UserCards",name);
	        	else
	        		PlayerPrefs.SetString("UserCards",PlayerPrefs.GetString("UserCards")+","+name);
	        	yield break;
	        }
	        elapsedTime += Time.deltaTime;
	        if(time-elapsedTime<0.01)
	        	elapsedTime=time;
	        yield return null;
	    }
    }

    public void onUndo(){
    	if(PlayerPrefs.GetString("LastMessage")!="")
    		PlayerPrefs.SetString("Send","Undo");
    }

    public void Undo(){
    	string message=PlayerPrefs.GetString("LastMessage");
    	if(message.Split(':').ToList()[0]=="Table"){
    		string senderName=message.Split(':').ToList()[1];
    		GameObject rc=GameObject.Find(message.Split(':').ToList()[2]);
    		string name="";
			if(message.Split(':').ToList()[3].Length==2)
				name=name+message.Split(':').ToList()[3][1]+message.Split(':').ToList()[3][0];
			else if(message.Split(':').ToList()[3].Length==3)
				name=name+message.Split(':').ToList()[3][2]+message.Split(':').ToList()[3][0]+message.Split(':').ToList()[3][1];
    		GameObject child=rc.transform.GetChild(rc.transform.childCount-1).gameObject;
			if(senderName!=PlayerPrefs.GetString("User")){
        		child.transform.SetParent(GameObject.Find(senderName).transform,false);
        		child.transform.position=rc.transform.position;
        		StartCoroutine(SmoothLerpDestroy(0.2f,child));
        	}else{
        		child.transform.SetParent(UserObject.transform,false);
        		child.transform.position=rc.transform.position;
        		StartCoroutine (SmoothLerpCreateUndoTable (0.2f,child,name));
        	}
    	}else if(message.Split(':').ToList()[0]=="Collect"){
    		string senderName=message.Split(':').ToList()[1];
    		GameObject rc=GameObject.Find(message.Split(':').ToList()[2]);
    		List<string> names=message.Split(':').ToList()[3].Split(',').ToList();
    		string newMessage="Collect:"+senderName+":"+rc.name+":";
    		foreach(string name in names){
    			Tabulation(newMessage+name);
    		}
    		if(PlayerPrefs.GetString("User")==senderName){
    			PlayerPrefs.SetInt("Collected",PlayerPrefs.GetInt("Collected")-1);
    		}
    	}
    	onTableCard();
    }

    private IEnumerator PlayAudio (string name)
    {
        AudioSource audio = GameObject.Find(name).GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
