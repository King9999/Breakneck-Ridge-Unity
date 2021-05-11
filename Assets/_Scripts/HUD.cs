using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
    public Texture2D timerTxtImg;
    public Texture2D goImg;
    public Texture2D pauseTxtImg;
    public Texture2D congratsImg;
    public GUISkin snowboardSkin;

    //Set up timer variables
    int minutes = 0;
    int seconds = 0;
    int milliseconds = 0;

    //Used for pausing game
    bool paused = false;

    //Check if game is over
    public static bool gameWon = false;

    const int MAIN_MENU = 0;
    const int GAME_SCREEN = 2;

    void Awake()
    { 
        //this restarts the race and resets the timer
        gameWon = false;
    }


	// Update is called once per frame
	void Update () 
    {
        
        //At each frame, check for pause state
        CheckForPause();
        /* The timer counts up each frame as follows:
        * Every 60 milliseconds (or frames), seconds increases by 1.
        * Every 60 seconds, minutes increases by 1.
         
         
        The timer resets to 0 every time a new game is started.  When the game is won, the timer
        immediately freezes so the player can view their time.*/

        if (!paused && !gameWon)
        {
            Time.timeScale = 1;
          
            //run the timer
            Start();
            
        }
        else
        {
            //pause game and show pause menu
            Time.timeScale = 0;
        }

       
	}

    void OnGUI()
    {
        GUI.skin = snowboardSkin;

        //this is for displaying the text graphics
        Rect timerTxtDisplay = new Rect(Screen.width / 1.3f, 0, timerTxtImg.width / 2, timerTxtImg.height / 2);
        Rect goTxtDisplay = new Rect(Screen.width / 2.5f, Screen.height / 5.5f, goImg.width, goImg.height);
        Rect congratsTxtDisplay = new Rect(Screen.width / 3.5f, Screen.height / 5.5f, congratsImg.width, congratsImg.height);
        Rect pauseTxtDisplay = new Rect(Screen.width / 2.5f, Screen.height / 5f, pauseTxtImg.width, pauseTxtImg.height);

        //buttons
        Rect menuButton = new Rect(Screen.width / 1.85f, Screen.height - 70, 160, 50);
        Rect resumeButton = new Rect(Screen.width / 3, Screen.height - 70, 160, 50);

        //this is for displaying the timer
        Rect timerDisplay = new Rect(Screen.width / 1.27f, 50, 100, 100);
        Rect finalTimeDisplay = new Rect(Screen.width / 2.2f, Screen.height / 2, 100, 100);

        
        //Display the timer
        GUI.DrawTexture(timerTxtDisplay, timerTxtImg);
        GUI.Label(timerDisplay, minutes.ToString() + "'" + seconds.ToString() + "\"" + milliseconds.ToString());

        //Display "GO!" for a moment at the start of the race
        if (minutes == 0 && seconds < 2)
        {
            GUI.DrawTexture(goTxtDisplay, goImg);
        }
        
        //Display Pause menu if game is paused
        if (paused)
        {
            GUI.DrawTexture(pauseTxtDisplay, pauseTxtImg);
            if (GUI.Button(menuButton, "Main Menu"))
            {
                //Go to the name entry menu scene
                Application.LoadLevel(MAIN_MENU);

            }

            if (GUI.Button(resumeButton, "Resume Game"))
            {

                //continue game
                paused = false;

            }

            //Display congratulations + final time
            //GUI.DrawTexture(congratsTxtDisplay, congratsImg);
            //GUI.Label(finalTimeDisplay, minutes.ToString() + "'" + seconds.ToString() + "\"" + milliseconds.ToString());
        }

        if (gameWon)
        {
            //pause game and show results
            Time.timeScale = 0;

            //Display congratulations + final time
            GUI.DrawTexture(congratsTxtDisplay, congratsImg);
            GUI.Label(finalTimeDisplay, minutes.ToString() + "'" + seconds.ToString() + "\"" + milliseconds.ToString());

            if (GUI.Button(menuButton, "Main Menu"))
            {
                //Go to the name entry menu scene
                Application.LoadLevel(MAIN_MENU);

            }

            if (GUI.Button(resumeButton, "Restart"))
            {

                //restart race
                Application.LoadLevel(GAME_SCREEN);


            }
            
        }

    }

    void Start()    //start timer
    {
        //Cancel the pause
        Time.timeScale = 1;
        Time.fixedDeltaTime = 1;

        milliseconds++;
        if (milliseconds > 59)
        {
            milliseconds = 0;

            //increase seconds by 1
            seconds++;

            if (seconds > 59)
            {
                seconds = 0;
                //Increase minutes by 1
                minutes++;

                if (minutes > 59)   //Should not get here unless game is running forever
                {
                    minutes = 59;

                }
            }
        }
    }

    void Reset()    //Sets timer to 0
    {
        minutes = 0;
        seconds = 0;
        milliseconds = 0;
    }

    //void Pause()
    //{
    //    //Pause game whenever this function is called
    //    if (paused)
    //        paused = false;
    //    else
    //        paused = true;
    //}

    void CheckForPause()
    {
        //Checks for when game is paused
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Pause game whenever this function is called
            if (paused)
                paused = false;
            else
                paused = true;
        }


    }

  

}