using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;
using VivoxUnity;

public class Game : MonoBehaviour
{
	string[] Cards=new string[]{"SA","SK","SQ","SJ","S10","S9","S8","S7","S6","S5","S4","S3","S2",
						"HA","HK","HQ","HJ","H10","H9","H8","H7","H6","H5","H4","H3","H2",
						"CA","CK","CQ","CJ","C10","C9","C8","C7","C6","C5","C4","C3","C2",
						"DA","DK","DQ","DJ","D10","D9","D8","D7","D6","D5","D4","D3","D2"};
	GameObject CardsObject;
	GameObject TableObject;
	Scrollbar s;
	Button Next;
	Button Prev;
	Button Restart;
	int MaxCards=13;
	float CardWidth=45;
	int Count;
	int count;
	
	float width=120;
	float height=180;
	float start;

	int row=6;
	int col=6;
	float startX=-250;
	float startY=500;
	float TableCardWidth=75;
	float TableCardHeight=112.5F;
	float ColWidth=100;
	float RowHeigth=132.5F;
	float RectWidth=170;
	float RectHeight=150;
	float RectRotation=90;

	List<int> CardNums=new List<int>();
	bool updateNav=true;
	
	float startTime=0;
	float holdTime=0.5F;
	bool LongPressCard=false;
	bool Decision=false;
	string buttonName="";
	bool DoubleTapCard=false;
	List<bool> CardBacks=new List<bool>();

	int CardNumSwap=-1;
	bool swap=false;

	bool openClose=false;

	float transparentColor=0;
	float translucentColor1=0.25F;
	float translucentColor2=0.5F;
	float opaqueColor=1;

    // Start is called before the first frame update

	List<string> UserCards;
	List<string> players;

    Client _client = new Client();
	
	string _tokenIssuer="hardik8714-ta23-dev";
	string _tokenDomain="mt1s.vivox.com";
	Uri _serverUri=new Uri("https://mt1s.www.vivox.com/api2");
	string _tokenKey="puck557";
	TimeSpan _tokenExpiration=new TimeSpan(0,0,90);

	ILoginSession _loginSession;
	IChannelSession _channelSession;
	bool LoggedIn=false;

	string SelectedTableCard="";
	bool Collect=false;
	int Collected=0;
	float escapeTime=0;
	// Start is called before the first frame update
    void Start()
    {
        _client.Initialize();
        GameObject.Find("Logout").GetComponent<Button>().enabled=false;
        GameObject.Find("Logout").GetComponent<Button>().interactable=false;
        GameObject.Find("ChannelName").GetComponent<InputField>().enabled=false;
        GameObject.Find("ChannelName").GetComponent<InputField>().interactable=false;
        GameObject.Find("Create").GetComponent<Button>().enabled=false;
        GameObject.Find("Create").GetComponent<Button>().interactable=false;
        GameObject.Find("Delete").GetComponent<Button>().enabled=false;
        GameObject.Find("Delete").GetComponent<Button>().interactable=false;
        GameObject.Find("Start").GetComponent<Button>().enabled=false;
        GameObject.Find("Start").GetComponent<Button>().interactable=false;

        if(PlayerPrefs.GetString("Mode")=="Connect"){
        	GameObject.Find("CreateText").GetComponent<Text>().text="Join";
        	GameObject.Find("DeleteText").GetComponent<Text>().text="Leave";
        	GameObject.Find("Start").GetComponent<RectTransform>().sizeDelta=new Vector2(0,0);
        }

        CardsObject=GameObject.Find("CardsGameObject");
	    TableObject=GameObject.Find("TableGameObject");
	    s=GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
	    Next=GameObject.Find("Next").GetComponent<Button>();
	    Prev=GameObject.Find("Previous").GetComponent<Button>();
	    Restart=GameObject.Find("Restart").GetComponent<Button>();
    }

    private void onLoginSessionPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
	{
	    if ("State" == propertyChangedEventArgs.PropertyName)
	    {
	        switch ((sender as ILoginSession).State)
	        {
	            case LoginState.LoggingIn:
	                // Operations as needed 
	            break;
	            
	            case LoginState.LoggedIn:
	            	GameObject.Find("Login").GetComponent<Button>().enabled=false;
			        GameObject.Find("Login").GetComponent<Button>().interactable=false;
			        GameObject.Find("UserName").GetComponent<InputField>().enabled=false;
			        GameObject.Find("UserName").GetComponent<InputField>().interactable=false;
			        GameObject.Find("Logout").GetComponent<Button>().enabled=true;
			        GameObject.Find("Logout").GetComponent<Button>().interactable=true;

			        GameObject.Find("ChannelName").GetComponent<InputField>().enabled=true;
			        GameObject.Find("ChannelName").GetComponent<InputField>().interactable=true;
			        GameObject.Find("Create").GetComponent<Button>().enabled=true;
			        GameObject.Find("Create").GetComponent<Button>().interactable=true;
			        GameObject.Find("Delete").GetComponent<Button>().enabled=false;
			        GameObject.Find("Delete").GetComponent<Button>().interactable=false;
			        LoggedIn=true;
	                // Operations as needed 
	            break;	
	
	            case LoginState.LoggedOut:
	                // Operations as needed 
	            break;
	            default:
	            break;
	        }	
	    }
	}

    public void Login(){
    	string username=GameObject.Find("UserNameText").GetComponent<Text>().text;
    	AccountId accountId = new AccountId(_tokenIssuer, username, _tokenDomain);
	    _loginSession = _client.GetLoginSession(accountId);
	    _loginSession.PropertyChanged += onLoginSessionPropertyChanged;
	    _loginSession.BeginLogin(_serverUri, _loginSession.GetLoginToken(_tokenKey,_tokenExpiration), ar => 
        {
            try
            {
                _loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                // Handle error 
                return;
            }
            // At this point, login is successful and other operations can be performed.
        });
    }

    public void Logout(){
    	_loginSession.Logout();
    	_loginSession.PropertyChanged -= onLoginSessionPropertyChanged;

    	GameObject.Find("Login").GetComponent<Button>().enabled=true;
    	GameObject.Find("Login").GetComponent<Button>().interactable=true;
        GameObject.Find("UserName").GetComponent<InputField>().enabled=true;
        GameObject.Find("UserName").GetComponent<InputField>().interactable=true;
        GameObject.Find("Logout").GetComponent<Button>().enabled=false;
        GameObject.Find("Logout").GetComponent<Button>().interactable=false;

        GameObject.Find("ChannelName").GetComponent<InputField>().enabled=false;
        GameObject.Find("ChannelName").GetComponent<InputField>().interactable=false;
        GameObject.Find("Create").GetComponent<Button>().enabled=false;
        GameObject.Find("Create").GetComponent<Button>().interactable=false;
        GameObject.Find("Delete").GetComponent<Button>().enabled=false;
        GameObject.Find("Delete").GetComponent<Button>().interactable=false;
    }

    private void SourceOnChannelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
	    // This example only checks for TextState changes. 
	    if (propertyChangedEventArgs.PropertyName == "AudioState") 
	    {
	        switch (_channelSession.AudioState) 
	        {
	            case ConnectionState.Connecting:
	            	Debug.Log("Audio connecting in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Connected:
	            	Debug.Log("Audio connected in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Disconnecting:
	            	Debug.Log("Audio disconnecting in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Disconnected:
	            	Debug.Log("Audio disconnected in " + _channelSession.Key.Name); 
	            break;
	        }
	    }
	    if (propertyChangedEventArgs.PropertyName == "TextState") 
	    {
	        switch (_channelSession.TextState) 
	        {
	            case ConnectionState.Connecting:
	            	Debug.Log("Text connecting in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Connected:
					GameObject.Find("Login").GetComponent<Button>().enabled=false;
			    	GameObject.Find("Login").GetComponent<Button>().interactable=false;
			        GameObject.Find("UserName").GetComponent<InputField>().enabled=false;
			        GameObject.Find("UserName").GetComponent<InputField>().interactable=false;
				    GameObject.Find("Logout").GetComponent<Button>().enabled=false;
			        GameObject.Find("Logout").GetComponent<Button>().interactable=false;

			        GameObject.Find("ChannelName").GetComponent<InputField>().enabled=false;
			        GameObject.Find("ChannelName").GetComponent<InputField>().interactable=false;
			        GameObject.Find("Create").GetComponent<Button>().enabled=false;
			        GameObject.Find("Create").GetComponent<Button>().interactable=false;
			        GameObject.Find("Delete").GetComponent<Button>().enabled=true;
			        GameObject.Find("Delete").GetComponent<Button>().interactable=true;
			        
			        if(PlayerPrefs.GetString("Mode")=="Create"){
				        GameObject.Find("Start").GetComponent<Button>().enabled=true;
	        			GameObject.Find("Start").GetComponent<Button>().interactable=true;
	        		}
		            Debug.Log("Text connected in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Disconnecting:
	            	Debug.Log("Text disconnecting in " + _channelSession.Key.Name); 
	            break;
	            
				case ConnectionState.Disconnected:
	            	Debug.Log("Text disconnected in " + _channelSession.Key.Name); 
	            break;
	        }
	    }
	}

    public void Create(){
    	string channelname=GameObject.Find("ChannelNameText").GetComponent<Text>().text;
    	ChannelId channelId = new ChannelId(_tokenIssuer, channelname, _tokenDomain, ChannelType.NonPositional);
	    _channelSession = _loginSession.GetChannelSession(channelId);
	    _channelSession.PropertyChanged += SourceOnChannelPropertyChanged;
	    _channelSession.BeginConnect(true, true, true, _channelSession.GetConnectToken(_tokenKey,_tokenExpiration), ar => 
        {
            try
            {
                _channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                // Handle error 
                return;
            }
            // At this point, login is successful and other operations can be performed.
        });
        BindChannelSessionHandlers(true);
    }

    public void Delete(){
    	string channelname=GameObject.Find("ChannelNameText").GetComponent<Text>().text;
    	ChannelId channelId = new ChannelId(_tokenIssuer, channelname, _tokenDomain, ChannelType.NonPositional);
    	_channelSession.Disconnect();
    	_loginSession.DeleteChannelSession(channelId);
    	_channelSession.PropertyChanged -= SourceOnChannelPropertyChanged;
    	BindChannelSessionHandlers(false);
    	GameObject.Find("players").GetComponent<Text>().text="";

    	GameObject.Find("Login").GetComponent<Button>().enabled=false;
    	GameObject.Find("Login").GetComponent<Button>().interactable=false;
        GameObject.Find("UserName").GetComponent<InputField>().enabled=false;
        GameObject.Find("UserName").GetComponent<InputField>().interactable=false;
    	GameObject.Find("Logout").GetComponent<Button>().enabled=true;
        GameObject.Find("Logout").GetComponent<Button>().interactable=true;

        GameObject.Find("ChannelName").GetComponent<InputField>().enabled=true;
        GameObject.Find("ChannelName").GetComponent<InputField>().interactable=true;
        GameObject.Find("Create").GetComponent<Button>().enabled=true;
        GameObject.Find("Create").GetComponent<Button>().interactable=true;
        GameObject.Find("Delete").GetComponent<Button>().enabled=false;
        GameObject.Find("Delete").GetComponent<Button>().interactable=false;

        if(PlayerPrefs.GetString("Mode")=="Create"){
        	GameObject.Find("Start").GetComponent<Button>().enabled=false;
        	GameObject.Find("Start").GetComponent<Button>().interactable=false;
        }
    }

    private static void ValidateArgs(object[] objs)			
	{			
	    foreach (var obj in objs)		
	    {	
	        if (obj == null)
	            throw new ArgumentNullException(obj.GetType().ToString(), "Specify a non-null/non-empty argument.");
	    }
	}

    private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
	{
	    ValidateArgs(new object[] { sender, keyEventArg });
		
	    var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;	
	    var participant = source[keyEventArg.Key];
	    var username = participant.Account.Name;
	    var channel = participant.ParentChannelSession.Key;
	    var channelSession = participant.ParentChannelSession;
	    //Do what you want with the information
	    Text playersText=GameObject.Find("players").GetComponent<Text>();
	    if(playersText.text=="")
	    	playersText.text=username;
	    else
	    	playersText.text=playersText.text+","+username;
	}

	private void OnParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
	{
	    ValidateArgs(new object[] { sender, keyEventArg });
				
	    var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;		
	    var participant = source[keyEventArg.Key];
	    var username = participant.Account.Name;
	    var channel = participant.ParentChannelSession.Key;
	    var channelSession = participant.ParentChannelSession;
	    Text playersText=GameObject.Find("players").GetComponent<Text>();
	    List<string> playersTextList=playersText.text.Split(',').ToList();
	    playersTextList.Remove(username);
	    playersText.text=string.Join( ",", playersTextList.ToArray() );	
	    if (participant.IsSelf)		
	    {
	        Delete();
	    }else if(GameObject.Find("players").GetComponent<RectTransform>().sizeDelta==new Vector2(0,0)){
	    	Destroy(GameObject.Find(username));
	    	players.Remove(username);
	    }
	}

	private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
	{
	    ValidateArgs(new object[] { sender, valueEventArg }); //see code from earlier in post
				
	    var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;			
	    var participant = source[valueEventArg.Key];			
				
	    string username = valueEventArg.Value.Account.Name;
	    ChannelId channel = valueEventArg.Value.ParentChannelSession.Key;
	    string property = valueEventArg.PropertyName;
		
	    switch (property)
	    {
	        case "LocalMute":
	        {
	            if (username != GameObject.Find("UserNameText").GetComponent<Text>().text) //can't local mute yourself, so don't check for it
	            {
	                //update their muted image
	            }else{

	            }
	            break;
	        }
	        default:		
	            break;
	    }
	}

	private IEnumerator SmoothLerpCreate (float time, GameObject child)
    {
	    Vector3 startingPos  = child.transform.localPosition;
	    Vector3 finalPos = new Vector3(0,0,0);
	    float elapsedTime = 0;
	    
	    while (elapsedTime <= time)
	    {
	        child.transform.localPosition = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
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
	        elapsedTime += Time.deltaTime;
	        if(time-elapsedTime<0.01){
	        	elapsedTime=time;
	        	Destroy(child);
	        }
	        yield return null;
	    }
    }

	void OnChannelMessageReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs)
    {
        var channelName = queueItemAddedEventArgs.Value.ChannelSession.Channel.Name; 
        var senderName = queueItemAddedEventArgs.Value.Sender.Name;
        var message = queueItemAddedEventArgs.Value.Message;

        if(senderName!=PlayerPrefs.GetString("User") && message=="Connect"){
        	PlayerPrefs.SetString("Mode","Connect");
        	destroyTable();
        	destroyCards();
        	Escape();
        	Collected=0;
        	destroyNames();
        }else if(PlayerPrefs.GetString("Mode")=="Connect" && message=="GetIn")
        	onStart();
        else if("Cards"==message.Split(':').ToList()[0])
        	PlayerPrefs.SetString("Cards",message.Split(':').ToList()[1]);
        else if("Order"==message.Split(':').ToList()[0]){
        	GameObject.Find("players").GetComponent<Text>().text=message.Split(':').ToList()[1];
        	PlayerPrefs.SetString("Players",GameObject.Find("players").GetComponent<Text>().text);
        	players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
        	displayNames();
        }else if(PlayerPrefs.GetString("User")==message.Split(':').ToList()[0]){
        	UserCards=new List<string>(message.Split(':').ToList()[1].Split(',').ToList());
            displayCards();
        }else if(message.Split(':').ToList()[0]=="Table"){
        	GameObject rc=GameObject.Find(message.Split(':').ToList()[1]);
        	foreach(string c in message.Split(':').ToList()[2].Split(';').ToList()){
				GameObject child=new GameObject(message.Split(':').ToList()[1]+rc.transform.childCount);
				child.transform.SetParent(rc.transform,false);
				if(senderName!=PlayerPrefs.GetString("User")){
	        		child.transform.position=GameObject.Find(senderName).transform.position;
	        	}else{
	        		child.transform.position=CardsObject.transform.position;
	        	}
				Image img = child.AddComponent<Image>() as Image;
				Texture2D SpriteTexture = Resources.Load<Texture2D>(c.Split(',').ToList()[0]);
				if(c.Split(',').ToList()[1]=="1"){
					SpriteTexture=Resources.Load<Texture2D>("CardBack");
				}
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
				img.sprite = NewSprite;
				RectTransform rectTransform = img.GetComponent<RectTransform>();
		        rectTransform.sizeDelta = new Vector2(TableCardWidth, TableCardHeight);	
		        StartCoroutine (SmoothLerpCreate (0.2f,child));
        	}
        	onTableCard();
        }else if(message.Split(':').ToList()[0]=="Collect"){
        	GameObject rc=GameObject.Find(message.Split(':').ToList()[1]);
        	if(SelectedTableCard==message.Split(':').ToList()[1]){
				Image BRCOld=GameObject.Find(SelectedTableCard).transform.GetChild(0).gameObject.GetComponent<Image>();
				Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
				Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
				BRCOld.sprite = SpriteOld;
				for(int i=1;i<GameObject.Find(SelectedTableCard).transform.childCount;i++){
					Destroy(GameObject.Find(SelectedTableCard+"V"+(i-1)));
				}
				SelectedTableCard="";
				Collect=false;
				Destroy(GameObject.Find("Collect"));
			}
			for(int i=1;i<=rc.transform.childCount;i++){
				GameObject child=GameObject.Find(message.Split(':').ToList()[1]+i);
				if(senderName!=PlayerPrefs.GetString("User")){
	        		child.transform.SetParent(GameObject.Find(senderName).transform,false);
	        		child.transform.position=rc.transform.position;
	        	}else{
	        		child.transform.SetParent(CardsObject.transform,false);
	        		child.transform.position=rc.transform.position;
	        	}
        		StartCoroutine (SmoothLerpDestroy (0.2f,child));
        	}
        }
        Debug.Log(channelName + ": " + senderName + ": " + message);
    }

    private void BindChannelSessionHandlers(bool doBind)
	{
	    //Subscribing to the events
	    if (doBind)
	    {
	        // Participants
	        _channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
	        _channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
	        _channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
			
	        //Messaging
	        _channelSession.MessageLog.AfterItemAdded += OnChannelMessageReceived;
	    }
		
	    //Unsubscribing to the events
	    else
	    {
	        // Participants
	        _channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
	        _channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
	        _channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
		
	        //Messaging
	        _channelSession.MessageLog.AfterItemAdded -= OnChannelMessageReceived;
	    }
	}

	void SendGroupMessage(string message)
    {   
        _channelSession.BeginSendText(message, ar => 
        {
            try
            {
                _channelSession.EndSendText(ar);
            }
            catch (Exception e)
            {
                // Handle error 
                return;
            }
        });
    }

    public void onRestart(){
    	PlayerPrefs.SetString("Mode","Create");
    	SendGroupMessage("Connect");
		destroyTable();
		destroyCards();
		Escape();
		Collected=0;
		destroyNames();
    	onStart();
    }

    public void onStart()
    {
    	PlayerPrefs.SetString("User",GameObject.Find("UserNameText").GetComponent<Text>().text);
		PlayerPrefs.SetString("Channel",GameObject.Find("ChannelNameText").GetComponent<Text>().text);
		PlayerPrefs.SetInt("NumOfPlayers",GameObject.Find("players").GetComponent<Text>().text.Split(',').ToList().Count);
		PlayerPrefs.SetString("Players",GameObject.Find("players").GetComponent<Text>().text);

		GameObject.Find("UserName").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("Login").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("Logout").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("ChannelName").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("Create").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("Delete").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("players").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		GameObject.Find("Start").GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		
		if(PlayerPrefs.GetString("Mode")=="Create"){
			SendGroupMessage("GetIn");
			SendGroupMessage("Cards:"+PlayerPrefs.GetString("Cards"));
			SendGroupMessage("Order:"+PlayerPrefs.GetString("Players"));
			players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
	    	string cards=PlayerPrefs.GetString("Cards");
	    	int NumOfPlayers=PlayerPrefs.GetInt("NumOfPlayers");
	    	List<string> decks=new List<String>(cards.Split(',').ToList());
	    	List<string> PlayCards=new List<string>();
	    	foreach(string deck in decks){
	    		for(int i=0;i<deck.Length;i++){
	    			if(deck[i]=='1'){
	    				PlayCards.Add(Cards[i]);
	    			}
	    		}
	    	}
	    	System.Random ran = new System.Random();
	    	int n=PlayCards.Count;
	    	while (n > 1) {
	    		n--;
	    		int k=ran.Next(n+1);
	    		string value=PlayCards[k];
	    		PlayCards[k]=PlayCards[n];
	    		PlayCards[n]=value;
	    	}
		    for(int i=0;i<NumOfPlayers;i++){
		    	int PlayerCardSize=PlayCards.Count/NumOfPlayers;
		    	if(i<PlayCards.Count%NumOfPlayers)
		    		PlayerCardSize+=1;
		    	List<string> PlayerCards=new List<string>();
		    	for(int j=0;j<PlayerCardSize;j++){
		    		PlayerCards.Add(PlayCards[i*PlayerCardSize+j]);
		    	}
		    	PlayerCards=PlayerCards.OrderBy(x => Array.IndexOf(Cards,x)).ToList();
				SendGroupMessage(players[i]+":"+string.Join( ",", PlayerCards.ToArray() ));
		    }
		}
		displayTable();
    }

    void destroyCards(){
    	for(int i=0;i<count;i++){
    		Destroy(GameObject.Find("C"+i));
    	}
    	updateNav=true;
    }

    void displayCards(){
		int idx=0;
		Count=UserCards.Count;
		start=0;
		count=Count;

		s.value=0;
    	if(count>MaxCards){
			count=MaxCards;
			s.size=1.0F*count/Count;
		}else{
			s.size=1;
		}
		if(count%2==0)
			start=CardWidth/2+(count/2-1)*CardWidth;
		else if(count!=1)
			start=CardWidth*(count-1)/2;
		start=start*(-1);
		foreach(string card in UserCards){
			string name="";
			if(card.Length==2)
				name=name+card[1]+card[0];
			else
				name=name+card[1]+card[2]+card[0];
			GameObject c=new GameObject("C"+idx);
			c.transform.SetParent(CardsObject.transform,false);
			Button btn = c.AddComponent<Button>() as Button;
			Image img = c.AddComponent<Image>() as Image;
			Texture2D SpriteTexture = Resources.Load<Texture2D>(name);
			float loc=start+CardWidth*idx;
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			img.sprite = NewSprite;
			btn.image=img;
			// btn.onClick.AddListener(delegate { onClick(); });
			RectTransform rectTransform = btn.GetComponent<RectTransform>();
        	rectTransform.localPosition = new Vector3(loc, 0, 0);
        	rectTransform.sizeDelta = new Vector2(width, height);
			idx=idx+1;
			if(idx==MaxCards)
				break;
		}
		updateNav=true;
    }

    void destroyTable(){
    	for(int r=0;r<row;r++){
			for(int c=0;c<col;c++){
				Destroy(GameObject.Find("RC"+r+""+c));
			}
		}
    }

    void displayTable(){
    	for(int r=0;r<row;r++){
			for(int c=0;c<col;c++){
				GameObject rc=new GameObject("RC"+r+""+c);
				rc.transform.SetParent(TableObject.transform,false);
				Button btn = rc.AddComponent<Button>() as Button;
				Image img = rc.AddComponent<Image>() as Image;
				Texture2D SpriteTexture = Resources.Load<Texture2D>("Transparent");
				float locX=startX+r*ColWidth;
				float locY=startY-c*RowHeigth;
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
			}
		}
    }

    void destroyNames(){
    	foreach(string name in players){
    		Destroy(GameObject.Find(name));
    	}
    }

    void displayNames(){
    	int i=players.IndexOf(PlayerPrefs.GetString("User"));
    	switch (players.Count)
	    {
	        case 2:
	            for(int j=1;j<players.Count;j++){
	            	GameObject player=new GameObject(players[(i+j)%players.Count]);
	            	player.transform.SetParent(TableObject.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		        	rectTransform.localPosition = new Vector3(0, 600, 0);
		        	rectTransform.sizeDelta = new Vector2(200, 75);
		        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
	            }
	            break;
	        case 3:
	        	for(int j=1;j<players.Count;j++){
	            	GameObject player=new GameObject(players[(i+j)%players.Count]);
	            	player.transform.SetParent(TableObject.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		            switch(j){
		            	case 1:
		            		rectTransform.localPosition = new Vector3(-300, 600, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
				        	break;
				        case 2:
				        	rectTransform.localPosition = new Vector3(300, 600, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
				        	break;
				        default:
				        	break;
		            }
	            }
	        	break;
	        case 4:
	            for(int j=1;j<players.Count;j++){
	            	GameObject player=new GameObject(players[(i+j)%players.Count]);
	            	player.transform.SetParent(TableObject.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		            switch(j){
		            	case 1:
		            		rectTransform.localPosition = new Vector3(-360, 50, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 90);
				        	break;
				        case 2:
				        	rectTransform.localPosition = new Vector3(0, 600, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
				        	break;
				        case 3:
				        	rectTransform.localPosition = new Vector3(360, 50, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, -90);
				        	break;
				        default:
				        	break;
		            }
	            }
	            break;
	        default:
	            Console.WriteLine("Default case");
	            break;
	      }
    }

    // Update is called once per frame
    void Escape(){
    	if(openClose){
			openClose=false;
			LongPressCard=false;
			Destroy(GameObject.Find("Open"));
	    	Destroy(GameObject.Find("Close"));
	    	Drag();
		}
		if(LongPressCard){
    		LongPressCard=false;
    		Destroy(GameObject.Find("Swap"));
	    	Destroy(GameObject.Find("Sort"));
	    	Destroy(GameObject.Find("Randomize"));
	    	Drag();
	    }
	    if(CardNums.Count!=0){
	    	CardNums.Clear();
	    	CardBacks.Clear();
	    	Drag();
	    }
	    if(swap){
	    	Destroy(GameObject.Find("Swap"));
	    	CardNumSwap=-1;
	    	swap=false;
	    	Drag();
	    }
	    if(SelectedTableCard!=""){
	    	Image BRCOld=GameObject.Find(SelectedTableCard).transform.GetChild(0).gameObject.GetComponent<Image>();
			Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
			Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
			BRCOld.sprite = SpriteOld;
			for(int i=1;i<GameObject.Find(SelectedTableCard).transform.childCount;i++){
				Destroy(GameObject.Find(SelectedTableCard+"V"+(i-1)));
			}
			Destroy(GameObject.Find("Collect"));
			Collect=false;
			SelectedTableCard="";
	    }
    }

    void Update()
    {
	    if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.currentSelectedGameObject) {
    		Escape();	
		}

		if(GameObject.Find("players").GetComponent<RectTransform>().sizeDelta==new Vector2(0,0)){
			Restart.enabled=true;
			Image RestartImage=Restart.image;
			Color RestartImageColor=RestartImage.color;
			RestartImageColor.a=opaqueColor;
			RestartImage.color=RestartImageColor;
		}else{
			Restart.enabled=false;
			Image RestartImage=Restart.image;
			Color RestartImageColor=RestartImage.color;
			RestartImageColor.a=transparentColor;
			RestartImage.color=RestartImageColor;
		}

		if(SelectedTableCard=="" && GameObject.Find("players").GetComponent<RectTransform>().sizeDelta==new Vector2(0,0)){
			GameObject CollectedObject=GameObject.Find("Collected");
            CollectedObject.GetComponent<Text>().text="Collection : "+Collected;
            CollectedObject.GetComponent<RectTransform>().sizeDelta=new Vector2(200, 75);
		}else{
			GameObject CollectedObject=GameObject.Find("Collected");
            CollectedObject.GetComponent<Text>().text="Collection : "+Collected;
            CollectedObject.GetComponent<RectTransform>().sizeDelta=new Vector2(0, 0);
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			if(Time.time-escapeTime<1 || GameObject.Find("players").GetComponent<RectTransform>().sizeDelta!=new Vector2(0,0)){
				if(GameObject.Find("Delete").GetComponent<Button>().enabled)
					Delete();
				if(LoggedIn)
					Application.Quit();
				else if(PlayerPrefs.GetString("Mode")=="Connect")
					SceneManager.LoadScene("SampleScene");
				else if(PlayerPrefs.GetString("Mode")=="Create")
					SceneManager.LoadScene("CreateScene");
			}else{
				escapeTime=Time.time;
			}
		}

		if(GameObject.Find("players").GetComponent<RectTransform>().sizeDelta!=new Vector2(0, 0) && GameObject.Find("players").GetComponent<Text>().text!=""){
			List<string> tempList=GameObject.Find("players").GetComponent<Text>().text.Split(',').ToList();
			string tempString=GameObject.Find("UserNameText").GetComponent<Text>().text;	
			if(PlayerPrefs.GetString("Mode")=="Connect" && tempList[0]==tempString)
				Delete();
			if(PlayerPrefs.GetString("Mode")=="Create" && tempList[0]!=tempString)
				Delete();
		}

		if(!swap){
			if(Input.GetKeyDown(KeyCode.Mouse0)
				&& EventSystem.current.currentSelectedGameObject
	         	&& EventSystem.current.currentSelectedGameObject.name.Length<4
	         	&& Time.time-startTime>holdTime){
				startTime=Time.time;
				Decision=true;
				buttonName=EventSystem.current.currentSelectedGameObject.name;
				LongPressCard=true;
				DoubleTapCard=false;
			}
			if((Time.time-startTime>0 && Time.time-startTime<holdTime-0.2F && !DoubleTapCard)
				|| (Time.time-startTime>0 && Time.time-startTime<holdTime && LongPressCard)){
				if(LongPressCard && !Input.GetKey(KeyCode.Mouse0)){
					LongPressCard=false;
				}
				if(Input.GetKeyDown(KeyCode.Mouse0)
					&& EventSystem.current.currentSelectedGameObject
	         		&& EventSystem.current.currentSelectedGameObject.name==buttonName){
						DoubleTapCard=true;
	         	}
			}else if(Time.time-startTime>0 && Decision){
				if(!LongPressCard && !DoubleTapCard){
					onClick();
				}
				if(LongPressCard){
					onClick();
				}
				if(DoubleTapCard){
					onClick();
				}
				Decision=false;
			}
		}else if(Input.GetKeyDown(KeyCode.Mouse0)
			&& EventSystem.current.currentSelectedGameObject 
			&& EventSystem.current.currentSelectedGameObject.name.Length<4){
			onClick();
		}

    	if(updateNav){
    		updateNav=false;
	        if(Count>MaxCards){
	        	Next.enabled=true;
	        	Prev.enabled=true;
	        	GameObject.Find("Scrollbar").GetComponent<Scrollbar>().enabled=true;
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
				GameObject.Find("Scrollbar").GetComponent<Scrollbar>().enabled=false;
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

    public void onNext(){
    	if(Next.image.color.a==opaqueColor){
    		Shift(true);
    		updateNav=true;
    	}
    }
    
    public void onPrev(){
    	if(Prev.image.color.a==opaqueColor){
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
					if(CardNums.Contains(j) && CardBacks[CardNums.IndexOf(j)]){
						SpriteTexture=Resources.Load<Texture2D>("CardBack");
					}
					float SpriteWidth=SpriteTexture.width;
					float loc=start+CardWidth*idx;
					Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
					img.sprite = NewSprite;
					if(LongPressCard){
						btn.enabled=false;
						img.color=Color.gray;
					}else{
						btn.enabled=true;
						img.color=Color.white;
					}
					if(openClose && CardNums.Contains(j)){
						img.color=Color.gray;
					}else if(openClose){
						img.color=Color.white;	
					}
					btn.image=img;
					RectTransform rectTransform = btn.GetComponent<RectTransform>();
					rectTransform.localPosition = new Vector3(loc, 0, 0);
					if(CardNums.Contains(j))
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

    public void Sort(){
    	UserCards=UserCards.OrderBy(x => Array.IndexOf(Cards,x)).ToList();
    	LongPressCard=false;
    	Drag();
    	Destroy(GameObject.Find("Swap"));
    	Destroy(GameObject.Find("Sort"));
    	Destroy(GameObject.Find("Randomize"));
    }

    public void Randomize(){
    	System.Random ran = new System.Random();
    	int n=UserCards.Count;
    	while (n > 1) {
    		n--;
    		int k=ran.Next(n+1);
    		string value=UserCards[k];
    		UserCards[k]=UserCards[n];
    		UserCards[n]=value;
    	}
    	LongPressCard=false;
    	Drag();
    	Destroy(GameObject.Find("Swap"));
    	Destroy(GameObject.Find("Sort"));
    	Destroy(GameObject.Find("Randomize"));
    }

    public void Swap(){
    	swap=true;
    	LongPressCard=false;
    	Drag();
    	Destroy(GameObject.Find("Sort"));
    	Destroy(GameObject.Find("Randomize"));
    }
    
    public void Open(){
    	for(int i=0;i<CardBacks.Count;i++){
    		CardBacks[i]=false;
    	}
    	LongPressCard=false;
    	openClose=false;
    	Destroy(GameObject.Find("Open"));
		Destroy(GameObject.Find("Close"));
		Drag();
    }

    public void Close(){
    	for(int i=0;i<CardBacks.Count;i++){
    		CardBacks[i]=true;
    	}
    	LongPressCard=false;
    	openClose=false;
    	Destroy(GameObject.Find("Open"));
		Destroy(GameObject.Find("Close"));
		Drag();
    }

    public void onClick(){
    	int i=Location();
    	int idx;
    	string name=EventSystem.current.currentSelectedGameObject.name;
    	if(name.Length==3){
    		idx=10+(name[2]-48);
    	}else{
    		idx=name[1]-48;
    	}
    	if(swap){
	    	if(CardNumSwap!=-1)
	    	{
	    		string Card1=UserCards[CardNumSwap];
	    		string Card2=UserCards[idx+i];
	    		UserCards[CardNumSwap]=Card2;
	    		UserCards[idx+i]=Card1;
	    		CardNumSwap=-1;
	    		Drag();
	    	}else{
	    		CardNumSwap=idx+i;
	    		GameObject c=GameObject.Find(name);
				Button btn = c.GetComponent<Button>();
				RectTransform rectTransform = btn.GetComponent<RectTransform>();
				rectTransform.Translate(0,50,0);
	    	}
    	}else if(LongPressCard && CardNums.Contains(idx+i) && !openClose){
    		GameObject open=new GameObject("Open");
			open.transform.SetParent(CardsObject.transform,false);
			Button OpenBtn = open.AddComponent<Button>() as Button;
			Image OpenImg = open.AddComponent<Image>() as Image;
			Texture2D SpriteOpen = Resources.Load<Texture2D>("RectangleOutlined");				
			Sprite OpenSprite = Sprite.Create(SpriteOpen, new Rect(0, 0, SpriteOpen.width, SpriteOpen.height),new Vector2(0,0),1);
			OpenImg.sprite=OpenSprite;
			OpenBtn.image=OpenImg;
			OpenBtn.onClick.AddListener(delegate { Open(); });
			RectTransform OpenrectTransform = OpenBtn.GetComponent<RectTransform>();
        	OpenrectTransform.localPosition = new Vector3(-150, 135, 0);
        	OpenrectTransform.sizeDelta = new Vector2(82.5F, 70);
        	OpenrectTransform.localEulerAngles = new Vector3(0, 0, RectRotation);

        	GameObject close=new GameObject("Close");
			close.transform.SetParent(CardsObject.transform,false);
			Button CloseBtn = close.AddComponent<Button>() as Button;
			Image CloseImg = close.AddComponent<Image>() as Image;
			Texture2D SpriteClose = Resources.Load<Texture2D>("RectangleFilled");				
			Sprite CloseSprite = Sprite.Create(SpriteClose, new Rect(0, 0, SpriteClose.width, SpriteClose.height),new Vector2(0,0),1);
			CloseImg.sprite=CloseSprite;
			CloseBtn.image=CloseImg;
			CloseBtn.onClick.AddListener(delegate { Close(); });
			RectTransform CloserectTransform = CloseBtn.GetComponent<RectTransform>();
        	CloserectTransform.localPosition = new Vector3(150, 135, 0);
        	CloserectTransform.sizeDelta = new Vector2(75, 70);
        	CloserectTransform.localEulerAngles = new Vector3(0, 0, RectRotation);

			LongPressCard=false;
        	openClose=true;
        	Drag();
    	}else if(LongPressCard){
    		if(!Collect){
	    		CardNums.Clear();
	    		CardBacks.Clear();
	    		if(openClose){
	    			openClose=false;
			    	Destroy(GameObject.Find("Open"));
					Destroy(GameObject.Find("Close"));
	    		}
	    		Drag();
	    		GameObject CardsObject=GameObject.Find("CardsGameObject");

				GameObject sort=new GameObject("Sort");
				sort.transform.SetParent(CardsObject.transform,false);
				Button SortBtn = sort.AddComponent<Button>() as Button;
				Image SortImg = sort.AddComponent<Image>() as Image;
				Texture2D SpriteSort = Resources.Load<Texture2D>("sort");				
				Sprite SortSprite = Sprite.Create(SpriteSort, new Rect(0, 0, SpriteSort.width, SpriteSort.height),new Vector2(0,0),1);
				SortImg.sprite=SortSprite;
				SortBtn.image=SortImg;
				SortBtn.onClick.AddListener(delegate { Sort(); });
				RectTransform SortrectTransform = SortBtn.GetComponent<RectTransform>();
	        	SortrectTransform.localPosition = new Vector3(-150, 140, 0);
	        	SortrectTransform.sizeDelta = new Vector2(50, 70);
	        	SortrectTransform.localEulerAngles = new Vector3(0, 0, RectRotation);

	        	GameObject randomize=new GameObject("Randomize");
				randomize.transform.SetParent(CardsObject.transform,false);
				Button RandomizeBtn = randomize.AddComponent<Button>() as Button;
				Image RandomizeImg = randomize.AddComponent<Image>() as Image;
				Texture2D SpriteRandomize = Resources.Load<Texture2D>("randomize");				
				Sprite RandomizeSprite = Sprite.Create(SpriteRandomize, new Rect(0, 0, SpriteRandomize.width, SpriteRandomize.height),new Vector2(0,0),1);
				RandomizeImg.sprite=RandomizeSprite;
				RandomizeBtn.image=RandomizeImg;
				RandomizeBtn.onClick.AddListener(delegate { Randomize(); });
				RectTransform RandomizerectTransform = RandomizeBtn.GetComponent<RectTransform>();
	        	RandomizerectTransform.localPosition = new Vector3(150, 135, 0);
	        	RandomizerectTransform.sizeDelta = new Vector2(70, 50);

	        	GameObject swap=new GameObject("Swap");
				swap.transform.SetParent(CardsObject.transform,false);
				Button SwapBtn = swap.AddComponent<Button>() as Button;
				Image SwapImg = swap.AddComponent<Image>() as Image;
				Texture2D SpriteSwap = Resources.Load<Texture2D>("swap");				
				Sprite SwapSprite = Sprite.Create(SpriteSwap, new Rect(0, 0, SpriteSwap.width, SpriteSwap.height),new Vector2(0,0),1);
				SwapImg.sprite=SwapSprite;
				SwapBtn.image=SwapImg;
				SwapBtn.onClick.AddListener(delegate { Swap(); });
				RectTransform SwaprectTransform = SwapBtn.GetComponent<RectTransform>();
	        	SwaprectTransform.localPosition = new Vector3(0, 135, 0);
	        	SwaprectTransform.sizeDelta = new Vector2(70, 50);
	        }else{
	        	LongPressCard=false;
	        }
	    }else if(DoubleTapCard && CardNums.Contains(idx+i)){
	    		CardBacks[CardNums.IndexOf(idx+Location())]=!CardBacks[CardNums.IndexOf(idx+Location())];
	    		Drag();
	    		DoubleTapCard=false;
	    }else{
    		if(CardNums.Contains(idx+i)){
	    		CardBacks.RemoveAt(CardNums.IndexOf(idx+i));
	    		CardNums.Remove(idx+i);
	    		Drag();
	    	}else{
	    		CardNums.Add(idx+i);
	    		CardBacks.Add(false);
	    		Drag();
	    	}
	    	updateNav=true;
	    }
	}

	public void Collection(){
		Collected+=1;
		SendGroupMessage("Collect:"+SelectedTableCard);
	}

	public void onTableCard(){
		string BtnName="";
		if(EventSystem.current.currentSelectedGameObject!=null){
			BtnName=EventSystem.current.currentSelectedGameObject.name;
		}
		if(BtnName!="" && CardNums.Count>0){
			List<string> message=new List<string>();
			for(int i=0;i<CardNums.Count;i++){
				string name="";
				if(UserCards[CardNums[i]].Length==2)
					name=name+UserCards[CardNums[i]][1]+UserCards[CardNums[i]][0];
				else
					name=name+UserCards[CardNums[i]][1]+UserCards[CardNums[i]][2]+UserCards[CardNums[i]][0];
				if(CardBacks[i])
					message.Add(name+",1");
				else
					message.Add(name+",0");
				UserCards.RemoveAt(CardNums[i]);
		        for(int j=i+1;j<CardNums.Count;j++){
		        	if(CardNums[i]<CardNums[j])
		        		CardNums[j]--;
		        }
			}
			SendGroupMessage("Table:"+BtnName+":"+string.Join(";",message.ToArray()));
			CardNums.Clear();
			CardBacks.Clear();
			destroyCards();
		    displayCards();
		    // TODO : Drag();
		}else{
			if(SelectedTableCard!=""){
				Image BRCOld=GameObject.Find(SelectedTableCard).transform.GetChild(0).gameObject.GetComponent<Image>();
				Texture2D SpriteTextureOld = Resources.Load<Texture2D>("RoundedRectangle");
				Sprite SpriteOld = Sprite.Create(SpriteTextureOld, new Rect(0, 0, SpriteTextureOld.width, SpriteTextureOld.height),new Vector2(0,0),1);
				BRCOld.sprite = SpriteOld;
				for(int i=1;i<GameObject.Find(SelectedTableCard).transform.childCount;i++){
					Destroy(GameObject.Find(SelectedTableCard+"V"+(i-1)));
				}
			}	
			Image BRC=GameObject.Find(BtnName).transform.GetChild(0).gameObject.GetComponent<Image>();
			Texture2D SpriteTexture = Resources.Load<Texture2D>("RectangleOutlined");
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0),1);
			BRC.sprite = NewSprite;
			
			float Vstart=0;
			float VCardWidth=28.125F;
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
				v.transform.SetParent(CardsObject.transform,false);
				Image Vimg = v.AddComponent<Image>() as Image;
				Vimg.sprite=img.sprite;
				RectTransform rectTransform = Vimg.GetComponent<RectTransform>();
	        	rectTransform.localPosition = new Vector3(loc, 210, 0);
	        	rectTransform.sizeDelta=new Vector2(TableCardWidth,TableCardHeight);
			}
			if(BtnName!="")
				SelectedTableCard=BtnName;
			if(!Collect && SelectedTableCard!="" && GameObject.Find(SelectedTableCard).transform.childCount>1){
				GameObject collect=new GameObject("Collect");
				Collect=true;
				collect.transform.SetParent(CardsObject.transform,false);
				Button CollectBtn = collect.AddComponent<Button>() as Button;
				Image CollectImg = collect.AddComponent<Image>() as Image;
				Texture2D SpriteCollect = Resources.Load<Texture2D>("Collect");				
				Sprite CollectSprite = Sprite.Create(SpriteCollect, new Rect(0, 0, SpriteCollect.width, SpriteCollect.height),new Vector2(0,0),1);
				CollectImg.sprite=CollectSprite;
				CollectBtn.image=CollectImg;
				CollectBtn.onClick.AddListener(delegate { Collection(); });
				RectTransform CollectrectTransform = CollectBtn.GetComponent<RectTransform>();
	        	CollectrectTransform.localPosition = new Vector3(0, 125, 0);
	        	CollectrectTransform.sizeDelta = new Vector2(70, 50);
			}else if(Collect && (SelectedTableCard=="" || GameObject.Find(SelectedTableCard).transform.childCount<=1)){
				Destroy(GameObject.Find("Collect"));
				Collect=false;
			}
		}
	}
}
