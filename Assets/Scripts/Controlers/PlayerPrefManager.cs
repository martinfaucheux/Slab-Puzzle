using UnityEngine;
using System.Collections;

public static class PlayerPrefManager {

    public static int GetLastLevel()
    {
        if (PlayerPrefs.HasKey("LastLevel"))
        {
            return PlayerPrefs.GetInt("LastLevel");
        }
        else
        {
            return 0;
        }
    }

    public static void SetLastLevel(int level)
    {
        PlayerPrefs.SetInt("LastLevel", level);
    }

    public static int GetCurrentLevel()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            return PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            return 0;
        }
    }

    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
    }



    // story the current player state info into PlayerPrefs
    public static void SavePlayerState(int currentLevel, int lastLevel)
    {
        // save currentscore and lives to PlayerPrefs for moving to next level
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("LastLevel", lastLevel);
    }

    // reset stored player state and variables back to defaults
    public static void ResetPlayerState(bool resetUnlockedLevel = false) {
		Debug.Log ("Player State reset.");

        int lastLevel = GetLastLevel();

        if (resetUnlockedLevel)
        {
            for(int i = 1; i < lastLevel; i++)
            {
                PlayerPrefs.SetInt("Level_" + i, 0);
            }
        }

        PlayerPrefs.SetInt("LastLevel",0);
        PlayerPrefs.SetInt("CurrentLevel", 0);
    }

    // init the playerPref
    public static void InitPlayerState(int startLevel = 0)
    {
        SetCurrentLevel(startLevel);
        SetLastLevel(startLevel);

        for (int i = 0; i < startLevel; i++)
            UnlockLevel(i);
    }

    // store a key for the name of the current level to indicate it is unlocked
    public static void UnlockLevel(int level) {
		// get current scene
		PlayerPrefs.SetInt("Level_"+level,1);
	}

	// determine if a levelname is currently unlocked (i.e., it has a key set)
	public static bool LevelIsUnlocked(int level) {
		if(PlayerPrefs.HasKey("Level_" + level))
        {
            return (PlayerPrefs.GetInt("Level_" + level) == 1);
        }
        return false;
	}

    // output the defined Player Prefs to the console
    public static void ShowPlayerPrefs()
    {
        Debug.Log("PlayerPrefs: CurrentLevel = " + GetCurrentLevel() + "; LastLevel = " + GetLastLevel());
        for(int i = 0; i < GetLastLevel()+1; i++)
        {
            if (LevelIsUnlocked(i))
            {
                Debug.Log("PlayerPrefs: level " + i + " is unlocked");
            }
        }
    }
    


}
