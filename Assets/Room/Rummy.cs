using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rummy : MonoBehaviour
{
	GameObject TableObject;
	GameObject UserObject;

	List<string> PlayCards=new List<string>();
	List<string> PlayedCards=new List<string>();

	bool Floatation=false;
    // Start is called before the first frame update
    void Start()
    {
    	TableObject=GameObject.Find("TableObject");
		UserObject=GameObject.Find("UserObject");

        if(PlayerPrefs.GetString("PlayCards")!=""){
        	PlayCards=PlayerPrefs.GetString("PlayCards").Split(',').ToList();
        	PlayerPrefs.SetString("PlayCards","");
        }

        PlayerPrefs.SetString("FloatCard","");

        GameObject v=new GameObject("V");
		v.transform.SetParent(GameObject.Find("VirtualUserObject").transform,false);
		Image Vimg = v.AddComponent<Image>() as Image;
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Transparent");
		Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		Vimg.sprite = NewSprite;
		RectTransform rectTransform = v.GetComponent<RectTransform>();
		rectTransform.localPosition = new Vector3(0, 0, 0);
		rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);

		GameObject collect=new GameObject("Collect");
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
		CollectrectTransform.sizeDelta = new Vector2(0, 0);

		GameObject upload=new GameObject("Upload");
		upload.transform.SetParent(GameObject.Find("Temp").transform,false);
		Button UploadBtn = upload.AddComponent<Button>() as Button;
		Image UploadImg = upload.AddComponent<Image>() as Image;
		Texture2D SpriteUpload = Resources.Load<Texture2D>("Upload");				
		Sprite UploadSprite = Sprite.Create(SpriteUpload, new Rect(0, 0, SpriteUpload.width, SpriteUpload.height),new Vector2(0,0),1);
		UploadImg.sprite=UploadSprite;
		UploadBtn.image=UploadImg;
		UploadBtn.onClick.AddListener(delegate { Uploaded(); });
		RectTransform UploadrectTransform = UploadBtn.GetComponent<RectTransform>();
		UploadrectTransform.localPosition = new Vector3(-340, -340, 0);
		UploadrectTransform.sizeDelta = new Vector2(120, 120);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetString("Recieved")!=""){
			string message=PlayerPrefs.GetString("Recieved");
			if(message=="DisplayTable"){
				displayTable();
				PlayerPrefs.SetString("Recieved","");
			}else if(message=="RestartDeck"){
				StartCoroutine(PlayAudio("PlayCard"));
				RestartDeck();
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="ClickDeckPick"){
				StartCoroutine(PlayAudio("PlayCard"));
				ClickDeckPick(message.Split(':').ToList()[1]);
				Floatation=true;
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="ClickDeckPlace"){
				StartCoroutine(PlayAudio("PlayCard"));
				ClickDeckPlace(message.Split(':').ToList()[1],message.Split(':').ToList()[2]);
				Floatation=false;
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="ClickCardPick"){
				StartCoroutine(PlayAudio("PlayCard"));
				ClickCardPick(message.Split(':').ToList()[1],message.Split(':').ToList()[2]);
				Floatation=true;
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="ClickCardPlace"){
				StartCoroutine(PlayAudio("PlayCard"));
				ClickCardPlace(message.Split(':').ToList()[1],message.Split(':').ToList()[2]);
				Floatation=false;
				PlayerPrefs.SetString("Recieved","");
			}else if(message=="Collected"){
				StartCoroutine(PlayAudio("PlayCard"));
				Floatation=false;
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="Uploaded"){
				StartCoroutine(PlayAudio("PlayCard"));
				Upload(message.Split(':').ToList()[1].Split(',').ToList());
				Floatation=true;
				PlayerPrefs.SetString("Recieved","");
			}
		}

		if(PlayerPrefs.GetString("FloatCard")!=""){
			string card=PlayerPrefs.GetString("FloatCard");
			string name="";
			if(card.Length==2)
				name=name+card[1]+card[0];
			else
				name=name+card[1]+card[2]+card[0];
			Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			GameObject.Find("V").GetComponent<Image>().sprite = NewSprite;
			GameObject.Find("Collect").GetComponent<RectTransform>().sizeDelta=new Vector2(80,80);
		}else{
			Texture2D SpriteTexture = Resources.Load<Texture2D>("Transparent");
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			GameObject.Find("V").GetComponent<Image>().sprite = NewSprite;
			GameObject.Find("Collect").GetComponent<RectTransform>().sizeDelta=new Vector2(0,0);
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

	float transparentColor=0;
	float translucentColor1=0.25F;
	float translucentColor2=0.5F;
	float opaqueColor=1;

	string[] UserColor=new string[]{"Red","Green","Blue","Yellow"};

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
				if(!(r==2 && c==2 || r==3 && c==2))
					btn.onClick.AddListener(delegate { onTableCard(); });
				else if(r==3 && c==2)
					btn.onClick.AddListener(delegate { onClickCard(); });
				else if(r==2 && c==2)
					btn.onClick.AddListener(delegate { onClickDeck(); });
				RectTransform rectTransform = btn.GetComponent<RectTransform>();
				rectTransform.localPosition = new Vector3(locX, locY, 0);
				rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);

				GameObject B=new GameObject("B"+"RC"+r+""+c);
				B.transform.SetParent(rc.transform,false);
				Image BImage=B.AddComponent<Image>() as Image;
				Texture2D SpriteB = Resources.Load<Texture2D>("RectangleBlack");
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

		GameObject Undo=new GameObject("RestartDeck");
		Undo.transform.SetParent(GameObject.Find("RC22").transform,false);
		Button UndoBtn = Undo.AddComponent<Button>() as Button;
		Image UndoImg = Undo.AddComponent<Image>() as Image;
		Texture2D SpriteUndo = Resources.Load<Texture2D>("Undo");				
		Sprite UndoSprite = Sprite.Create(SpriteUndo, new Rect(0, 0, SpriteUndo.width, SpriteUndo.height),new Vector2(0,0),1);
		UndoImg.sprite=UndoSprite;
		UndoBtn.image=UndoImg;
		UndoBtn.onClick.AddListener(delegate { onRestartDeck(); });
		RectTransform UndorectTransform = UndoBtn.GetComponent<RectTransform>();
		UndorectTransform.localPosition = new Vector3(0, 0, 0);
		UndorectTransform.sizeDelta = new Vector2(90, 90);

		CardDeck();
	}

	void CardDeck(){
		foreach (Transform child in GameObject.Find("RC32").transform){
			if(child.name!="BRC32")
				Destroy(child.gameObject);
		}
		for(int i=0;i<PlayCards.Count;i++){
			string name="CardBack";
			GameObject c=new GameObject("T"+i);
			c.transform.SetParent(GameObject.Find("RC22").transform,false);
			Image img = c.AddComponent<Image>() as Image;
			Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			img.sprite = NewSprite;
			RectTransform rectTransform = c.GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(0, 0, 0);
			rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);
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
			if(time-elapsedTime<0.01)
				elapsedTime=time;
			yield return null;
		}
	}

	private IEnumerator SmoothLerpCreate (float time, GameObject child, string TempCard)
	{
		Vector3 startingPos  = child.transform.localPosition;
		Vector3 finalPos = new Vector3(0,0,0);
		float elapsedTime = 0;
		
		while (elapsedTime <= time)
		{
			child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
			if(time==elapsedTime){
				Destroy(child);
				PlayerPrefs.SetString("FloatCard",TempCard);
				yield break;
			}
			elapsedTime += Time.deltaTime;
			if(time-elapsedTime<0.01)
				elapsedTime=time;
			yield return null;
		}
	}

	public void onRestartDeck(){
		if(!Floatation)
			PlayerPrefs.SetString("Send","RestartDeck");
	}

	void RestartDeck(){
		PlayCards.Clear();
		for(int i=PlayedCards.Count-1;i>-1;i--)
			PlayCards.Add(PlayedCards[i]);
		PlayedCards.Clear();
		CardDeck();
		Image BRCOld1=GameObject.Find("BRC32").GetComponent<Image>();
		Image BRCOld2=GameObject.Find("BRC22").GetComponent<Image>();
		Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RectangleBlack");
		Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
		BRCOld1.sprite = SpriteOld;
		BRCOld2.sprite = SpriteOld;
		Color color1=BRCOld1.color;
		Color color2=BRCOld2.color;
		color1.a=transparentColor;
		color2.a=transparentColor;
		BRCOld1.color=color1;
		BRCOld2.color=color2;
	}

	public void onClickDeck(){
		if(!Floatation && PlayCards.Count!=0)
			PlayerPrefs.SetString("Send","ClickDeckPick:"+PlayerPrefs.GetString("User"));
		else if(Floatation && PlayerPrefs.GetString("FloatCard")!="")
			PlayerPrefs.SetString("Send","ClickDeckPlace:"+PlayerPrefs.GetString("User")+":"+PlayerPrefs.GetString("FloatCard"));
	}

	void ClickDeckPlace(string distributor, string card){
		PlayerPrefs.SetString("FloatCard","");
		PlayCards.Add(card);
		GameObject rc=GameObject.Find("RC22");
		
		Image BRC=GameObject.Find("BRC22").GetComponent<Image>();
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Rectangle"+UserColor[PlayerPrefs.GetString("Players").Split(',').ToList().IndexOf(distributor)]);
		Sprite Sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		BRC.sprite = Sprite;
		Color color=BRC.color;
		color.a=opaqueColor;
		BRC.color=color;
		Image BRCOld=GameObject.Find("BRC32").GetComponent<Image>();
		Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RectangleBlack");
		Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
		BRCOld.sprite = SpriteOld;
		Color colorOld=BRCOld.color;
		colorOld.a=transparentColor;
		BRCOld.color=colorOld;
		
		GameObject child=new GameObject("T"+rc.transform.childCount);
		child.transform.SetParent(rc.transform,false);
		if(distributor!=PlayerPrefs.GetString("User"))
			child.transform.position=GameObject.Find(distributor).transform.position;
		else
			child.transform.position=UserObject.transform.position;
		Image img = child.AddComponent<Image>() as Image;
		Texture2D SpriteTextureCard = Resources.Load<Texture2D>("CardBack");
		Sprite NewSprite = Sprite.Create(SpriteTextureCard, new Rect(0, 0, SpriteTextureCard.width, SpriteTextureCard.height),new Vector2(0,0),1);
		img.sprite = NewSprite;
		RectTransform rectTransform = img.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);	
		StartCoroutine (SmoothLerpCreatePlace (0.2f,child));
	}

	void ClickDeckPick(string distributor){
		GameObject RC=GameObject.Find("RC22");
		
		Image BRC=GameObject.Find("BRC22").GetComponent<Image>();
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Rectangle"+UserColor[PlayerPrefs.GetString("Players").Split(',').ToList().IndexOf(distributor)]);
		Sprite Sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		BRC.sprite = Sprite;
		Color color=BRC.color;
		color.a=opaqueColor;
		BRC.color=color;
		Image BRCOld=GameObject.Find("BRC32").GetComponent<Image>();
		Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RectangleBlack");
		Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
		BRCOld.sprite = SpriteOld;
		Color colorOld=BRCOld.color;
		colorOld.a=transparentColor;
		BRCOld.color=colorOld;
		
		if(PlayerPrefs.GetString("User")==distributor){
			GameObject child=RC.transform.GetChild(RC.transform.childCount-1).gameObject;
			string TempCard=PlayCards[PlayCards.Count-1];
			PlayCards.RemoveAt(PlayCards.Count-1);
			Vector3 TempPosition=child.transform.position;
			child.transform.SetParent(GameObject.Find("VirtualUserObject").transform,false);
			child.transform.position=TempPosition;
			StartCoroutine (SmoothLerpCreate(0.2f,child,TempCard));
		}else{
			GameObject child=RC.transform.GetChild(RC.transform.childCount-1).gameObject;
			PlayCards.RemoveAt(PlayCards.Count-1);
			Vector3 TempPosition=child.transform.position;
			child.transform.SetParent(GameObject.Find(distributor).transform,false);
			child.transform.position=TempPosition;
			StartCoroutine (SmoothLerpDestroy(0.2f,child));
		}
	}

	public void Collected(){
		if(PlayerPrefs.GetString("UserCards")=="" || PlayerPrefs.GetString("UserCards")=="Start")
			PlayerPrefs.SetString("UserCards",PlayerPrefs.GetString("FloatCard"));
		else
			PlayerPrefs.SetString("UserCards",PlayerPrefs.GetString("UserCards")+","+PlayerPrefs.GetString("FloatCard"));
		PlayerPrefs.SetString("FloatCard","");
		PlayerPrefs.SetString("Send","Collected");
	}

	public void Uploaded(){
		if(!Floatation && PlayerPrefs.GetString("UserCards")!="" && PlayerPrefs.GetString("UserCards")!="Start")
			PlayerPrefs.SetString("Send","Uploaded:"+PlayerPrefs.GetString("UserCards"));
	}

	void Upload(List<string> cards){
		PlayerPrefs.SetString("FloatCard","");
		float Vstart=0;
		float VCardWidth=37.5F;
		int Vcount=cards.Count;
		if(Vcount%2==0)
			Vstart=VCardWidth/2+(Vcount/2-1)*VCardWidth;
		else if(Vcount!=1)
			Vstart=VCardWidth*(Vcount-1)/2;
		Vstart=Vstart*(-1);
		for(int i=0;i<cards.Count;i++){
			float loc=Vstart+VCardWidth*i;
			GameObject v=new GameObject("V"+i);
			v.transform.SetParent(GameObject.Find("VirtualUserObject").transform,false);
			Image Vimg = v.AddComponent<Image>() as Image;
			string name="";
			if(cards[i].Length==2)
				name=name+cards[i][1]+cards[i][0];
			else
				name=name+cards[i][1]+cards[i][2]+cards[i][0];
			Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			Vimg.sprite=NewSprite;
			RectTransform rectTransform = Vimg.GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(loc, 0, 0);
			rectTransform.sizeDelta=new Vector2(TableCardWidth,TableCardHeight);
		}
	}

	public void onClickCard(){
		if((Floatation && PlayerPrefs.GetString("FloatCard")!=""))
			PlayerPrefs.SetString("Send","ClickCardPlace:"+PlayerPrefs.GetString("User")+":"+PlayerPrefs.GetString("FloatCard"));
		else if(!Floatation && PlayerPrefs.GetInt("CardNumSwap")!=-1){
			List<string> cards=PlayerPrefs.GetString("UserCards").Split(',').ToList();
			PlayerPrefs.SetString("Send","ClickCardPlace:"+PlayerPrefs.GetString("User")+":"+cards[PlayerPrefs.GetInt("CardNumSwap")]);
			cards.RemoveAt(PlayerPrefs.GetInt("CardNumSwap"));
			PlayerPrefs.SetString("UserCards",string.Join(",",cards.ToArray()));
		}else if(!Floatation && PlayedCards.Count!=0)
			PlayerPrefs.SetString("Send","ClickCardPick:"+PlayerPrefs.GetString("User")+":"+PlayedCards[PlayedCards.Count-1]);
	}

	private IEnumerator SmoothLerpCreatePlace (float time, GameObject child)
	{
		Vector3 startingPos  = child.transform.localPosition;
		Vector3 finalPos = new Vector3(0,0,0);
		float elapsedTime = 0;
		
		while (elapsedTime <= time)
		{
			child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
			if(time==elapsedTime)
				yield break;
			elapsedTime += Time.deltaTime;
			if(time-elapsedTime<0.01)
				elapsedTime=time;
			yield return null;
		}
	}

	void ClickCardPlace(string distributor,string card){
		PlayerPrefs.SetString("FloatCard","");
		PlayedCards.Add(card);
		GameObject rc=GameObject.Find("RC32");
		
		Image BRC=GameObject.Find("BRC32").GetComponent<Image>();
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Rectangle"+UserColor[PlayerPrefs.GetString("Players").Split(',').ToList().IndexOf(distributor)]);
		Sprite Sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		BRC.sprite = Sprite;
		Color color=BRC.color;
		color.a=opaqueColor;
		BRC.color=color;
		Image BRCOld=GameObject.Find("BRC22").GetComponent<Image>();
		Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RectangleBlack");
		Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
		BRCOld.sprite = SpriteOld;
		Color colorOld=BRCOld.color;
		colorOld.a=transparentColor;
		BRCOld.color=colorOld;
		
		GameObject child=new GameObject("L"+rc.transform.childCount);
		child.transform.SetParent(rc.transform,false);
		if(distributor!=PlayerPrefs.GetString("User"))
			child.transform.position=GameObject.Find(distributor).transform.position;
		else
			child.transform.position=UserObject.transform.position;
		string name="";
		if(card.Length==2)
			name=name+card[1]+card[0];
		else
			name=name+card[1]+card[2]+card[0];
		Image img = child.AddComponent<Image>() as Image;
		Texture2D SpriteTextureCard = Resources.Load<Texture2D>(name);
		Sprite NewSprite = Sprite.Create(SpriteTextureCard, new Rect(0, 0, SpriteTextureCard.width, SpriteTextureCard.height),new Vector2(0,0),1);
		img.sprite = NewSprite;
		RectTransform rectTransform = img.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);	
		StartCoroutine (SmoothLerpCreatePlace (0.2f,child));
	}

	void ClickCardPick(string distributor,string card){
		GameObject RC=GameObject.Find("RC32");
		
		Image BRC=GameObject.Find("BRC32").GetComponent<Image>();
		Texture2D SpriteTexture = Resources.Load<Texture2D>("Rectangle"+UserColor[PlayerPrefs.GetString("Players").Split(',').ToList().IndexOf(distributor)]);
		Sprite Sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
		BRC.sprite = Sprite;
		Color color=BRC.color;
		color.a=opaqueColor;
		BRC.color=color;
		Image BRCOld=GameObject.Find("BRC22").GetComponent<Image>();
		Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RectangleBlack");
		Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
		BRCOld.sprite = SpriteOld;
		Color colorOld=BRCOld.color;
		colorOld.a=transparentColor;
		BRCOld.color=colorOld;
		
		if(PlayerPrefs.GetString("User")==distributor){
			GameObject child=RC.transform.GetChild(RC.transform.childCount-1).gameObject;
			string TempCard=PlayedCards[PlayedCards.Count-1];
			PlayedCards.RemoveAt(PlayedCards.Count-1);
			Vector3 TempPosition=child.transform.position;
			child.transform.SetParent(GameObject.Find("VirtualUserObject").transform,false);
			child.transform.position=TempPosition;
			StartCoroutine (SmoothLerpCreate(0.2f,child,TempCard));
		}else{
			GameObject child=RC.transform.GetChild(RC.transform.childCount-1).gameObject;
			PlayedCards.RemoveAt(PlayedCards.Count-1);
			Vector3 TempPosition=child.transform.position;
			child.transform.SetParent(GameObject.Find(distributor).transform,false);
			child.transform.position=TempPosition;
			StartCoroutine (SmoothLerpDestroy(0.2f,child));
		}
	}

	public void onTableCard(){

	}

	private IEnumerator PlayAudio (string name)
	{
		AudioSource audio = GameObject.Find(name).GetComponent<AudioSource>();
		audio.Play();
		yield return new WaitForSeconds(audio.clip.length);
	}
}
