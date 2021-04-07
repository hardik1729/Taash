using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    private string gameID = "4074129";
    private string bannerID = "banner";
    GameObject dialog = null;

    void Start()
    {
        Advertisement.Initialize(gameID, true);
        if(!Permission.HasUserAuthorizedPermission(Permission.Microphone)){
            Permission.RequestUserPermission(Permission.Microphone);
            GameObject dialog=new GameObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Advertisement.IsReady(bannerID)){
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(bannerID);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    void OnGUI(){
        if(!Permission.HasUserAuthorizedPermission(Permission.Microphone)){
            dialog.AddComponent<PermissionsRationaleDialog>();
            return;
        }else if(dialog!=null){
            Destroy(dialog);
        }
    }

    private IEnumerator PlayAudio (string name)
    {
        AudioSource audio = GameObject.Find("Click").GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        SceneManager.LoadScene(name);
    }

    public void SceneChangeToRoom() {
        PlayerPrefs.SetString("Mode","Connect");
        StartCoroutine(PlayAudio("RoomScene"));
    }

    public void SceneChangeToCreate() {
        PlayerPrefs.SetString("Mode","Create");
        StartCoroutine(PlayAudio("CreateScene"));
    }
}
