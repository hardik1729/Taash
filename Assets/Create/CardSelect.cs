using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardSelect : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> cards=new List<string>(new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7","S6","S5","S4","S3","S2",
					    							 "HA","HK","HQ","HJ","H10","H9","H8","H7","H6","H5","H4","H3","H2",
					    							 "CA","CK","CQ","CJ","C10","C9","C8","C7","C6","C5","C4","C3","C2",
					    							 "DA","DK","DQ","DJ","D10","D9","D8","D7","D6","D5","D4","D3","D2"});
    List<string> cardsUpto7=new List<string>(new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7",
    													  "HA","HK","HQ","HJ","H10","H9","H8","H7",
    													  "CA","CK","CQ","CJ","C10","C9","C8","C7",
    													  "DA","DK","DQ","DJ","D10","D9","D8","D7"});
    List<string> cardsAbove7=new List<string>(new string[]{"S6","S5","S4","S3","S2",
    													   "H6","H5","H4","H3","H2",
    													   "C6","C5","C4","C3","C2",
    													   "D6","D5","D4","D3","D2"});
    List<string> FinalDecks=new List<string>();
    float width=75;
    float height=112.5F;
    float startX=-335;
    float startY=550;
    float WidthX=92.5F;
    float WidthY=132.5F;
    float ExtraWidthX=100;
    Button SelectUpto7;
    Button SelectAll;
    Button Next;
    Button NextDeck;
	Button PrevDeck;
	Text DeckNum;
	int maxDeckNum=4;

	bool updateDeck=true;
	bool updateArrow=true;
	bool updateSelectUpto7=true;
	bool updateSelectAll=true;
	bool updateNext=true;

	float transparentColor=0;
	float translucentColor1=0.25F;
	float translucentColor2=0.5F;
	float opaqueColor=1;

    void Start()
    {
    	GameObject CardsObject=GameObject.Find("CardsGameObject");
    	NextDeck=GameObject.Find("NextDeck").GetComponent<Button>();
	    PrevDeck=GameObject.Find("PrevDeck").GetComponent<Button>();
	    SelectUpto7=GameObject.Find("SelectUpto7").GetComponent<Button>();
	    SelectAll=GameObject.Find("SelectAll").GetComponent<Button>();
	    Next=GameObject.Find("Next").GetComponent<Button>();
	    DeckNum=GameObject.Find("DeckNum").GetComponent<Text>();
    	for(int i=0;i<4;i++){
    		for(int j=0;j<13;j++){
    			string name="";
				if(cards[13*i+j].Length==2)
					name=name+cards[13*i+j][1]+cards[13*i+j][0];
				else
					name=name+cards[13*i+j][1]+cards[13*i+j][2]+cards[13*i+j][0];
				GameObject c=new GameObject(cards[13*i+j]);
				c.transform.SetParent(CardsObject.transform,false);
				Button btn = c.AddComponent<Button>() as Button;
				Image img = c.AddComponent<Image>() as Image;
				Texture2D SpriteTexture = Resources.Load<Texture2D>(name);

				GameObject Tick=new GameObject("Tick"+cards[13*i+j]);
				Tick.transform.SetParent(c.transform,false);
				Image TickImage=Tick.AddComponent<Image>() as Image;
				Texture2D SpriteTick = Resources.Load<Texture2D>("Tick");				
				Sprite TickSprite = Sprite.Create(SpriteTick, new Rect(0, 0, SpriteTick.width, SpriteTick.height),new Vector2(0,0),1);
				TickImage.sprite=TickSprite;

				float locX;
				float locY;
				if(j<8){
					locX=startX+i*WidthX+i*ExtraWidthX;
					locY=startY-WidthY*j;
				}else{
					locX=startX+(i+1)*WidthX+i*ExtraWidthX;
					locY=startY-WidthY*(j%8);
				}
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
				img.sprite = NewSprite;
				btn.image=img;
				btn.onClick.AddListener(delegate { onCard(); });
				RectTransform rectTransform = btn.GetComponent<RectTransform>();
	        	rectTransform.localPosition = new Vector3(locX, locY, 0);
	        	rectTransform.sizeDelta = new Vector2(width, height);
    		}
    	}
    	foreach(string card in cards){
			Selection(card,false);
		}
		for(int i=0;i<maxDeckNum;i++){
			FinalDecks.Add("");
		}
    }

    // Update is called once per frame
    void Update()
    {
    	if (Input.GetKeyDown(KeyCode.Escape)) {
		    SceneManager.LoadScene("SampleScene");
		}
    	if(updateSelectAll){
    		updateSelectAll=false;
		    bool highlight=true;
		    foreach(string CardName in cards){
		    	Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
				Color color = card.color;
				if(color.a==opaqueColor){
					highlight=false;
					break;
				}
			}
			Image button=SelectAll.image;
			Color colorButton=button.color;
			if(highlight){
				colorButton.a=opaqueColor;
				button.color=colorButton;
			}else{
				colorButton.a=translucentColor1;
				button.color=colorButton;
			}
    	}
    	if(updateSelectUpto7){
    		updateSelectUpto7=false;
			bool highlight=true;
			foreach(string CardName in cardsUpto7){
				Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
				Color color = card.color;
				if(color.a==opaqueColor){
					highlight=false;
					break;
				}
			}
			if(highlight){
				foreach(string CardName in cardsAbove7){
				   	Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
					Color color = card.color;
					if(color.a==translucentColor2){
						highlight=false;
						break;
					}
				}
			}
			Image button=SelectUpto7.image;
			Color colorButton=button.color;
			Text text=GameObject.Find("Seven").GetComponent<Text>();
			Color colorText=text.color;
			if(highlight){
				colorButton.a=opaqueColor;
				button.color=colorButton;
				colorText.a=opaqueColor;
				text.color=colorText;
			}else{
				colorButton.a=translucentColor1;
				button.color=colorButton;
				colorText.a=translucentColor1;
				text.color=colorText;
			}
    	}
    	if(updateArrow){
    		updateArrow=false;
        	if(DeckNum.text[0]-48==1){
				Image imgP=PrevDeck.image;
				Color colorP=imgP.color;
				colorP.a=translucentColor1;
				imgP.color=colorP;
			}else{
				Image imgP=PrevDeck.image;
				Color colorP=imgP.color;
				colorP.a=opaqueColor;
				imgP.color=colorP;
			}
			if(DeckNum.text[0]-48==maxDeckNum){
				Image imgN=NextDeck.image;
				Color colorN=imgN.color;
				colorN.a=translucentColor1;
				imgN.color=colorN;
			}else{
				Image imgN=NextDeck.image;
				Color colorN=imgN.color;
				colorN.a=opaqueColor;
				imgN.color=colorN;
			}
		}
		if(updateDeck){
			updateDeck=false;
			FinalDecks[DeckNum.text[0]-49]="";
			foreach(string card in cards){
				if(GameObject.Find("Tick"+card).GetComponent<Image>().color.a==opaqueColor){
					FinalDecks[DeckNum.text[0]-49]+="1";
				}else{
					FinalDecks[DeckNum.text[0]-49]+="0";
				}
			}
		}
		if(updateNext){
    		updateNext=false;
		    bool highlight=false;
		    foreach(string deck in FinalDecks){
				foreach(char c in deck){
					if(c=='1'){
						highlight=true;
						break;
					}
				}
				if(highlight)
					break;
			}
			Image button=Next.image;
			Color colorButton=button.color;
			if(highlight){
				colorButton.a=opaqueColor;
				button.color=colorButton;
			}else{
				colorButton.a=translucentColor1;
				button.color=colorButton;
			}
    	}
    }
    
    public void onNext(){
		string combinedString = string.Join( ",", FinalDecks.ToArray() );
		if(Next.image.color.a==opaqueColor){
			PlayerPrefs.SetString("Mode","Create");
			PlayerPrefs.SetString("Cards",combinedString);
			SceneManager.LoadScene("RoomScene");
		}
    }

    public void onSelectAll(){
	    bool allSelected=true;
	    foreach(string CardName in cards){
		   	Image card=GameObject.Find(CardName).GetComponent<Button>().image;
			Color color = card.color;
			Image cardTick=GameObject.Find("Tick"+CardName).GetComponent<Image>(); 
			Color colorTick = cardTick.color;
			if(color.a==opaqueColor){
				color.a=translucentColor2;
				colorTick.a=opaqueColor;
				allSelected=false;
			}
			card.color=color;
			cardTick.color=colorTick;
		}
		if(allSelected){
			foreach(string card in cards){
		    	Selection(card,false);
			}
		}
		updateDeck=true;
		updateNext=true;
		updateSelectUpto7=true;
		updateSelectAll=true;
    }

    public void onSelectUpto7(){
	    bool selected7=true;
	    foreach(string CardName in cardsUpto7){
	    	Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
			Color color = card.color;
			if(color.a==opaqueColor)
				selected7=false;
		}
		foreach(string CardName in cardsAbove7){
		   	Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
			Color color = card.color;
			if(color.a==translucentColor2)
				selected7=false;
		}
		
	    if(selected7){
	    	foreach(string card in cardsUpto7){
		   		Selection(card,false);
			}
			foreach(string card in cardsAbove7){
			    Selection(card,false);
			}
    	}else{
	    	foreach(string card in cardsUpto7){
			  	Selection(card,true);
			}
			foreach(string card in cardsAbove7){
			   	Selection(card,false);
			}
		}
		updateDeck=true;
		updateNext=true;
		updateSelectUpto7=true;
		updateSelectAll=true;
	}

    public void onNextDeck(){
    	if(NextDeck.image.color.a==opaqueColor){
    		foreach(string card in cards){
				Selection(card,false);
			}
			DeckNum.text=(DeckNum.text[0]-48+1).ToString();
			for(int i=0;i<FinalDecks[DeckNum.text[0]-49].Length;i++){
				if(FinalDecks[DeckNum.text[0]-49][i]=='1'){
					Selection(cards[i],true);
				}else{
					string name=cards[i];
					Selection(name,false);
				}
			}
			updateSelectUpto7=true;
			updateSelectAll=true;
			updateArrow=true;
    	}
    }

    public void onPrevDeck(){
    	if(PrevDeck.image.color.a==opaqueColor){
    		foreach(string card in cards){
				Selection(card,false);
			}
			DeckNum.text=(DeckNum.text[0]-48-1).ToString();
			for(int i=0;i<FinalDecks[DeckNum.text[0]-49].Length;i++){
				if(FinalDecks[DeckNum.text[0]-49][i]=='1'){
					Selection(cards[i],true);
				}else{
					Selection(cards[i],false);
				}
			}
			updateSelectUpto7=true;
			updateSelectAll=true;
			updateArrow=true;
    	}
    }

    public void Selection(string CardName,bool select){
    	if(select){
    		Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
			Color color = card.color;
			color.a=translucentColor2;
			card.color=color;
			
			Image cardTick=GameObject.Find("Tick"+CardName).GetComponent<Image>(); 
			Color colorTick = cardTick.color;
			colorTick.a=opaqueColor;
			cardTick.color=colorTick;
		}else{
			Image card=GameObject.Find(CardName).GetComponent<Button>().image; 
			Color color = card.color;
			color.a=opaqueColor;
			card.color=color;
			
			Image cardTick=GameObject.Find("Tick"+CardName).GetComponent<Image>(); 
			Color colorTick = cardTick.color;
			colorTick.a=transparentColor;
			cardTick.color=colorTick;
		}
    }

    public void onCard(){
    	string name=EventSystem.current.currentSelectedGameObject.name;
    	Image card=GameObject.Find(name).GetComponent<Button>().image; 
		Color color = card.color;
		if(color.a==opaqueColor)
			color.a=translucentColor2;
		else
			color.a=opaqueColor;
		card.color=color;
		
		Image cardTick=GameObject.Find("Tick"+name).GetComponent<Image>(); 
		Color colorTick = cardTick.color;
		if(colorTick.a==opaqueColor)
			colorTick.a=transparentColor;
		else
			colorTick.a=opaqueColor;
		cardTick.color=colorTick;
		updateDeck=true;
		updateNext=true;
		updateSelectUpto7=true;
		updateSelectAll=true;
    }
}
