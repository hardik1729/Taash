using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using VivoxUnity;

public class Room : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject LobbyObj;
    GameObject GameObj;
    
    Client _client = new Client();

    string _tokenIssuer="hardik8714-ta23-dev";
	string _tokenDomain="mt1s.vivox.com";
	Uri _serverUri=new Uri("https://mt1s.www.vivox.com/api2");
	string _tokenKey="pump117";
	TimeSpan _tokenExpiration=new TimeSpan(0,0,90);

	ILoginSession _loginSession;
	IChannelSession _channelSession;
	
	bool LoggedIn=false;
	float escapeTime=0;

    void Start()
    {
        LobbyObj=GameObject.Find("LobbyCanvas");
		GameObj=GameObject.Find("GameCanvas");
		
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
		GameObj.GetComponent<RectTransform>().localScale=new Vector3(0,0,0);
		PlayerPrefs.SetString("Send","");
		PlayerPrefs.SetString("Recieved","");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
			if(Time.time-escapeTime<1 || LobbyObj.GetComponent<RectTransform>().localScale!=new Vector3(0,0,0)){
				if(LoggedIn)
					Application.Quit();
				else{
					Advertisement.Banner.Hide();
					if(PlayerPrefs.GetString("Mode")=="Connect")
						SceneManager.LoadScene("SampleScene");
					else if(PlayerPrefs.GetString("Mode")=="Create")
						SceneManager.LoadScene("CreateScene");
				}
			}else{
				escapeTime=Time.time;
			}
		}
		if(LobbyObj.GetComponent<RectTransform>().localScale!=new Vector3(0,0,0) && GameObject.Find("players").GetComponent<Text>().text!=""){
			List<string> tempList=GameObject.Find("players").GetComponent<Text>().text.Split(',').ToList();
			string tempString=GameObject.Find("UserNameText").GetComponent<Text>().text;	
			if(PlayerPrefs.GetString("Mode")=="Connect" && tempList[0]==tempString)
				Delete();
			if(PlayerPrefs.GetString("Mode")=="Create" && tempList[0]!=tempString)
				Delete();
		}
		if(PlayerPrefs.GetString("Recieved")!=""){
			if(PlayerPrefs.GetString("Mode")=="Connect"){
				string message=PlayerPrefs.GetString("Recieved");
				if(message=="GetIn"){
					onStart();
					PlayerPrefs.SetString("Recieved","");
				}
				else if("Cards"==message.Split(':').ToList()[0]){
					PlayerPrefs.SetString("Cards",message.Split(':').ToList()[1]);
					PlayerPrefs.SetString("Recieved","");
				}
				else if("Order"==message.Split(':').ToList()[0]){
		        	GameObject.Find("players").GetComponent<Text>().text=message.Split(':').ToList()[1];
		        	destroyNames();
	    	    	PlayerPrefs.SetString("Players",GameObject.Find("players").GetComponent<Text>().text);
					displayNames();
	    	    	PlayerPrefs.SetString("Recieved","");
	    	    	PlayerPrefs.SetString("VideoAd","Yes");
	    	    }
	    	}
		}
		if(PlayerPrefs.GetString("Send")!=""){
			List<string> messages=PlayerPrefs.GetString("Send").Split(';').ToList();
			foreach(string message in messages)
				SendGroupMessage(message);
			PlayerPrefs.SetString("Send","");
		}
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
    	StartCoroutine(PlayAudio("Click"));
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
    	StartCoroutine(PlayAudio("Click"));
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
    	StartCoroutine(PlayAudio("Click"));
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
    	StartCoroutine(PlayAudio("Click"));
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
		StartCoroutine(PlayAudio("PlayCard"));
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
		StartCoroutine(PlayAudio("PlayCard"));
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
	    }else if(LobbyObj.GetComponent<RectTransform>().localScale==new Vector3(0,0,0)){
	    	List<string> players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
	    	Destroy(GameObject.Find(username));
	    	players.Remove(username);
	    	PlayerPrefs.SetString("Players",string.Join(",",players.ToArray()));
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

    void OnChannelMessageReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs)
    {
        var channelName = queueItemAddedEventArgs.Value.ChannelSession.Channel.Name; 
        var senderName = queueItemAddedEventArgs.Value.Sender.Name;
        var message = queueItemAddedEventArgs.Value.Message;

        PlayerPrefs.SetString("Recieved",message);

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

    public void onStart(){
		PlayerPrefs.SetString("User",GameObject.Find("UserNameText").GetComponent<Text>().text);
		LobbyObj.GetComponent<RectTransform>().localScale=new Vector3(0,0,0);
    	GameObj.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
		if(PlayerPrefs.GetString("Mode")=="Create"){
			PlayerPrefs.SetString("Mode","Connect");
			SendGroupMessage("ClearAll");
			SendGroupMessage("GetIn");
			SendGroupMessage("Cards:"+PlayerPrefs.GetString("Cards"));
			SendGroupMessage("Order:"+GameObject.Find("players").GetComponent<Text>().text);
		}
    }

    void displayNames(){
    	List<string> players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
    	int i=players.IndexOf(PlayerPrefs.GetString("User"));
    	switch (players.Count)
	    {
	        case 2:
	            for(int j=1;j<players.Count;j++){
	            	GameObject player=new GameObject(players[(i+j)%players.Count]);
	            	player.transform.SetParent(GameObj.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		        	rectTransform.localPosition = new Vector3(0, 620, 0);
		        	rectTransform.sizeDelta = new Vector2(200, 75);
		        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
	            }
	            break;
	        case 3:
	        	for(int j=1;j<players.Count;j++){
	            	GameObject player=new GameObject(players[(i+j)%players.Count]);
	            	player.transform.SetParent(GameObj.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		            switch(j){
		            	case 1:
		            		rectTransform.localPosition = new Vector3(-320, 620, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
				        	break;
				        case 2:
				        	rectTransform.localPosition = new Vector3(320, 620, 0);
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
	            	player.transform.SetParent(GameObj.transform,false);
	            	Text playerText=player.AddComponent<Text>() as Text;
		            playerText.fontSize=32;
		            playerText.color=Color.black;
		            playerText.alignment = TextAnchor.MiddleCenter;
		            playerText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
		            playerText.text=players[(i+j)%players.Count];
		            RectTransform rectTransform = playerText.GetComponent<RectTransform>();
		            switch(j){
		            	case 1:
		            		rectTransform.localPosition = new Vector3(-380, 237.5F, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 90);
				        	break;
				        case 2:
				        	rectTransform.localPosition = new Vector3(0, 620, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, 0);
				        	break;
				        case 3:
				        	rectTransform.localPosition = new Vector3(380, 237.5F, 0);
				        	rectTransform.sizeDelta = new Vector2(200, 75);
				        	rectTransform.localEulerAngles = new Vector3(0, 0, -90);
				        	break;
				        default:
				        	break;
		            }
	            }
	            break;
	        default:
	            // Console.WriteLine("Default case");
	            break;
	      }
    }

    void destroyNames(){
    	List<string> players=new List<string>(PlayerPrefs.GetString("Players").Split(',').ToList());
    	foreach(string name in players){
    		if(name!=PlayerPrefs.GetString("User"))
    			Destroy(GameObject.Find(name));
    	}
    }

    private IEnumerator PlayAudio (string name)
    {
        AudioSource audio = GameObject.Find(name).GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
