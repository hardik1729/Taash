using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class Distribution : MonoBehaviour
{
	// Start is called before the first frame update
	string[] Cards=new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7","S6","S5","S4","S3","S2",
						"HA","HK","HQ","HJ","H10","H9","H8","H7","H6","H5","H4","H3","H2",
						"CA","CK","CQ","CJ","C10","C9","C8","C7","C6","C5","C4","C3","C2",
						"DA","DK","DQ","DJ","D10","D9","D8","D7","D6","D5","D4","D3","D2"};

	float TableCardWidth=100;
	float TableCardHeight=150;

	int Count=0;

	GameObject TableObject;
	GameObject UserObject;
	bool activeDistribute=true;
	bool distribution=false;
	List<string> PlayCards=new List<string>();

	void Start()
	{
		TableObject=GameObject.Find("TableObject");
		UserObject=GameObject.Find("UserObject");

		Scrollbar s=GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		Button Next=GameObject.Find("Next").GetComponent<Button>();
		Button Prev=GameObject.Find("Previous").GetComponent<Button>();

		float transparentColor=0;
		float translucentColor1=0.25F;
		float translucentColor2=0.5F;
		float opaqueColor=1;

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

	// Update is called once per frame
	void Update()
	{
		if(PlayerPrefs.GetString("Recieved")!=""){
			string message=PlayerPrefs.GetString("Recieved");
			if(message=="ClearAll"){
				StartCoroutine(PlayAudio("Click"));
				ClearAll();
				InitialPlayCards();
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="PlayCards"){
				PlayCards.AddRange(message.Split(':').ToList()[1].Split(',').ToList());
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="activeDistribute"){
				activeDistribute=!activeDistribute;
				if(activeDistribute){
					if(message.Split(':').ToList()[1]=="Discard")
						GameObject.Find("Script").AddComponent<Table>();
					else if(message.Split(':').ToList()[1]=="Keep"){
						PlayerPrefs.SetString("PlayCards",string.Join(",",PlayCards.ToArray()));
						GameObject.Find("Script").AddComponent<Rummy>();
					}
					foreach (Transform child in TableObject.transform)
						Destroy(child.gameObject);
				}else
					GameObject.Find("Script").AddComponent<Cards>();
				PlayCards.Clear();
				PlayerPrefs.SetString("Recieved","");
			}else if(message.Split(':').ToList()[0]=="Distribute"){
				StartCoroutine(PlayAudio("PlayCard"));
				string name=message.Split(':').ToList()[1];
				string num=message.Split(':').ToList()[2];
				int n=0;
				int p=1;
				for(int i=num.Length-1;i>-1;i--){
					n=n+((int)num[i]-48)*p;
					p=p*10;
				}
				for(int i=0;i<n;i++)
					Distribute(name);
				PlayerPrefs.SetString("Recieved","");
			}
		}

		if(distribution && Count!=PlayCards.Count && TableObject.transform.childCount>1){
			GameObject.Find("T"+(TableObject.transform.childCount-2)).GetComponent<Button>().enabled=false;
			GameObject.Find("T"+(TableObject.transform.childCount-2)).GetComponent<Button>().interactable=false;
			GameObject.Find("Keep").GetComponent<Button>().enabled=false;
			GameObject.Find("Keep").GetComponent<Button>().interactable=false;
			GameObject.Find("Discard").GetComponent<Button>().enabled=false;
			GameObject.Find("Discard").GetComponent<Button>().interactable=false;
			GameObject.Find("ShareAll").GetComponent<Button>().enabled=false;
			GameObject.Find("ShareAll").GetComponent<Button>().interactable=false;
		}
		else if(distribution && TableObject.transform.childCount>1){
			GameObject.Find("T"+(TableObject.transform.childCount-2)).GetComponent<Button>().enabled=true;
			GameObject.Find("T"+(TableObject.transform.childCount-2)).GetComponent<Button>().interactable=true;
			GameObject.Find("Keep").GetComponent<Button>().enabled=true;
			GameObject.Find("Keep").GetComponent<Button>().interactable=true;
			GameObject.Find("Discard").GetComponent<Button>().enabled=true;
			GameObject.Find("Discard").GetComponent<Button>().interactable=true;
			GameObject.Find("ShareAll").GetComponent<Button>().enabled=true;
			GameObject.Find("ShareAll").GetComponent<Button>().interactable=true;
		}

		if(distribution && TableObject.transform.childCount==1)
			DiscardDistribute();

		if(GameObject.Find("Scrollbar").GetComponent<Scrollbar>().enabled)
			Advertisement.Banner.Hide();
		else if(Advertisement.IsReady("banner")){
			Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
			Advertisement.Banner.Show("banner");  
		}
	}

	public void ClearAll(){
		foreach (Transform child in TableObject.transform)
			Destroy(child.gameObject);
		foreach (Transform child in UserObject.transform)
			Destroy(child.gameObject);
		foreach (Transform child in GameObject.Find("VirtualUserObject").transform)
			Destroy(child.gameObject);
		foreach (Transform child in GameObject.Find("Temp").transform)
			Destroy(child.gameObject);
		activeDistribute=true;
		distribution=false;
		PlayCards.Clear();
		Count=PlayCards.Count;
		GameObject GO=new GameObject("Script");
		GO.transform.SetParent(GameObject.Find("Temp").transform,false);

		Scrollbar s=GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		Button Next=GameObject.Find("Next").GetComponent<Button>();
		Button Prev=GameObject.Find("Previous").GetComponent<Button>();

		float transparentColor=0;
		float translucentColor1=0.25F;
		float translucentColor2=0.5F;
		float opaqueColor=1;

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
		string message="Distribute:"+PlayerPrefs.GetString("User")+":";
		int n=PlayCards.Count;
		int a=0;
		while(n>0){
			n=n-PlayerPrefs.GetString("Players").Split(',').ToList().Count;
			a=a+1;
		}
		PlayerPrefs.SetString("Send",message+a);
	}

	public void KeepDistribute(){
		StartCoroutine(PlayAudio("Click"));
		distribution=false;
		PlayerPrefs.SetString("Send","activeDistribute:Keep"+";DisplayTable");
		Destroy(GameObject.Find("ShareAll"));
		Destroy(GameObject.Find("Keep"));
		Destroy(GameObject.Find("Discard"));
	}

	public void DiscardDistribute(){
		StartCoroutine(PlayAudio("Click"));
		distribution=false;
		PlayerPrefs.SetString("Send","activeDistribute:Discard"+";DisplayTable");
		Destroy(GameObject.Find("ShareAll"));
		Destroy(GameObject.Find("Keep"));
		Destroy(GameObject.Find("Discard"));
	}

	public void clickDistribute(){
		if(activeDistribute){
			StartCoroutine(PlayAudio("Click"));
			GameObject Keep=new GameObject("Keep");
			Keep.transform.SetParent(UserObject.transform,false);
			Button KeepBtn = Keep.AddComponent<Button>() as Button;
			Image KeepImage=Keep.AddComponent<Image>() as Image;
			Texture2D SpriteKeep = Resources.Load<Texture2D>("Done");				
			Sprite KeepSprite = Sprite.Create(SpriteKeep, new Rect(0, 0, SpriteKeep.width, SpriteKeep.height),new Vector2(0,0),1);
			KeepImage.sprite=KeepSprite;
			KeepBtn.image=KeepImage;
			KeepBtn.onClick.AddListener(delegate { KeepDistribute(); });
			RectTransform KeeprectTransform = KeepBtn.GetComponent<RectTransform>();
			KeeprectTransform.localPosition = new Vector3(0, 160, 0);
			KeeprectTransform.sizeDelta = new Vector2(100, 70);

			GameObject Discard=new GameObject("Discard");
			Discard.transform.SetParent(TableObject.transform,false);
			Button DiscardBtn = Discard.AddComponent<Button>() as Button;
			Image DiscardImage=Discard.AddComponent<Image>() as Image;
			Texture2D SpriteDiscard = Resources.Load<Texture2D>("Cross");				
			Sprite DiscardSprite = Sprite.Create(SpriteDiscard, new Rect(0, 0, SpriteDiscard.width, SpriteDiscard.height),new Vector2(0,0),1);
			DiscardImage.sprite=DiscardSprite;
			DiscardBtn.image=DiscardImage;
			DiscardBtn.onClick.AddListener(delegate { DiscardDistribute(); });
			RectTransform DiscardrectTransform = DiscardBtn.GetComponent<RectTransform>();
			DiscardrectTransform.localPosition = new Vector3(57.5F, 82.5F+237.5F, 0);
			DiscardrectTransform.sizeDelta = new Vector2(50, 50);
			
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
			List<string> messages=new List<string>();
			int n=PlayCards.Count;
			int i=0;
			while(n>0){
				int j=52;
				if(n-52<0)
					j=n;
				messages.Add("PlayCards:"+string.Join(",",PlayCards.GetRange(i*52,j).ToArray()));
				i=i+1;
				n=n-52;
			}
			PlayerPrefs.SetString("Send","activeDistribute:None;"+string.Join(";",messages.ToArray()));
			distribution=true;
		}else if(distribution)
			PlayerPrefs.SetString("Send","Distribute:"+PlayerPrefs.GetString("User")+":1");
	}

	List<string> TempUserCards=new List<string>();

	public void Distribute(string distributor){
		List<string> players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
		int i=players.IndexOf(distributor);
		int k=players.IndexOf(PlayerPrefs.GetString("User"));
		for(int j=1;j<=players.Count && TableObject.transform.childCount>1;j++){
			if((i+j)%players.Count==k){
				GameObject child=TableObject.transform.GetChild(TableObject.transform.childCount-2).gameObject;
				TempUserCards.Add(PlayCards[TableObject.transform.childCount-2]);
				PlayCards.RemoveAt(TableObject.transform.childCount-2);
				Count=PlayCards.Count;
				Vector3 TempPosition=TableObject.transform.GetChild(0).position;
				child.transform.SetParent(UserObject.transform,false);
				child.transform.position=TempPosition;
				StartCoroutine (SmoothLerpCreate(0.5f,child));
			}else{
				GameObject child=TableObject.transform.GetChild(TableObject.transform.childCount-2).gameObject;
				PlayCards.RemoveAt(TableObject.transform.childCount-2);
				Count=PlayCards.Count;
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
			int i=0;
			foreach(string group in deck.Split('.').ToList()){
				int a=0;
				int p=1;
				for(int k=group.Length-1;k>-1;k--){
					a=a+((int)group[k]-48)*p;
					p=p*10;
				}
				for(int j=0;j<13;j++){
					if(a%2==1)
						PlayCards.Add(Cards[i*13+j]);
					a=a/2;
				}
				i=i+1;
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
		Count=PlayCards.Count;
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

	private IEnumerator PlayAudio (string name)
	{
		AudioSource audio = GameObject.Find(name).GetComponent<AudioSource>();
		audio.Play();
		yield return new WaitForSeconds(audio.clip.length);
	}
}
