using UnityEngine;
using System.Collections;

public class HelpMenu : MonoBehaviour
{
    //images
    public Texture2D tutorialScreenImg;
    public Texture2D screenImg1;
    public Texture2D screenImg2;
    public Texture2D screenImg3;
    public Texture2D screenImg4;

    const int MAIN_MENU = 0;

    public GUISkin snowboardSkin;

    void OnGUI()
    {
        Rect helpTitleLabel = new Rect(0, 0, Screen.width, Screen.height);

        //Labels for in-game screenshots
        Rect screenshotLabel1 = new Rect(Screen.width / 1.5f, Screen.height / 4.9f, screenImg1.width / 2, screenImg1.height / 2);
        Rect screenshotLabel2 = new Rect(Screen.width / 1.5f, Screen.height / 2f, screenImg2.width / 2, screenImg2.height / 2);
        Rect screenshotLabel3 = new Rect(Screen.width / 2.3f, Screen.height / 2.5f, screenImg3.width / 2, screenImg3.height / 2);
        Rect screenshotLabel4 = new Rect(Screen.width / 2.3f, Screen.height / 1.5f, screenImg4.width / 2, screenImg4.height / 2);
        Rect backButton = new Rect(Screen.width / 1.45f, Screen.height - 70, 160, 50);
       
        //Set up custom GUI
        GUI.skin = snowboardSkin;

        //Display tutorial
        GUI.DrawTexture(helpTitleLabel, tutorialScreenImg);

        //Display screenshots
        GUI.DrawTexture(screenshotLabel1, screenImg1);  //screenshot for "Learn to move"
        GUI.DrawTexture(screenshotLabel2, screenImg2);  //screenshot for "Don't crash"
        GUI.DrawTexture(screenshotLabel3, screenImg3);  //screenshot for "Keep track of time"
        GUI.DrawTexture(screenshotLabel4, screenImg4);  //screenshot for "Be brave." Pic may be changed to something more appropriate


        if (GUI.Button(backButton, "Back"))
        {
            Application.LoadLevel(MAIN_MENU);
        }
    }
}
