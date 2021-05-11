using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
   //constants for switching to menus
    const int MAIN_MENU = 0;
    const int HELP_MENU = 1;
    const int GAME_SCREEN = 2;

    //Set up the title image
    //public Texture2D gameTitleImg;
    public Texture2D titleBGImg;

    //set up button click SFX
    public AudioClip buttonClickSound;

    //Set up the custom skin
    public GUISkin snowboardSkin;

        
    void OnGUI()
    {
        //Buttons
        Rect startButton = new Rect(Screen.width / 1.85f, Screen.height - 70, 160, 50);
        Rect helpButton = new Rect(Screen.width / 1.45f, Screen.height - 70, 160, 50);
        Rect exitButton = new Rect(Screen.width / 1.19f, Screen.height - 70, 160, 50);

        //Apply custom GUI skin
        GUI.skin = snowboardSkin;

        //Game Title
        //Rect gameTitle = new Rect(Screen.width / 2.5f, Screen.height / 1.5f - 250, gameTitleImg.width, gameTitleImg.height);

        //Background
        Rect backgroundScreen = new Rect(0, 0, Screen.width, Screen.height);

        //Display the title
        GUI.DrawTexture(backgroundScreen, titleBGImg);

        		
        //Get button actions
        if (GUI.Button(startButton, "Begin Game"))
        {
           //buttonClickSound
            //Go to the name entry menu scene
            Application.LoadLevel(GAME_SCREEN);
       
        }

        if (GUI.Button(helpButton, "How To Play"))
        {
            //Go to the help menu scene
            Application.LoadLevel(HELP_MENU);
        }

        if (GUI.Button(exitButton, "Quit Game"))
        {
            //Quit the game
            Application.Quit();
        }
     
    }

}
