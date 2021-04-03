using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class Connect : MonoBehaviour
{
    // Start is called before the first frame update
    private string gameID = "4074129";
    private string bannerID = "banner";
    bool showBanner=false;
    void Start()
    {
        PlayerPrefs.SetString("Mode","Connect");
        Advertisement.Initialize(gameID, true);
    }

    public void SceneChange() {  
        Advertisement.Banner.Hide();
        SceneManager.LoadScene("RoomScene");  
    }
    // Update is called once per frame
    void Update()
    {
        if(!showBanner && Advertisement.IsReady(bannerID)){
            showBanner=true;
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(bannerID);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
