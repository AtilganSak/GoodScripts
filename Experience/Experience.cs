using UnityEngine;

[System.Serializable]
public class Experience
{
    [Tooltip("If enabled, will be save per changes")]
    public bool AutoSave;    
    public string KeyToSave;

    public float NextLevelXP = 50;
    [Tooltip("This value adding to NextLevelXP")]
    public float AddXpPerLevel = 50;
    [Tooltip("This value adding to AddXpPerLevel")]
    public float IncerementXPValue = 25;

    public float CurrentXP;
    public int CurrentLevel = 1;

    const string NEXTLEVELXPKEY = "nextlevelxpkey";
    const string ADDXPPERLEVELKEY = "addxpperlevelkey";
    const string CURRENTXPKEY = "currentxp";
    const string CURRENTLEVELKEY = "currentlevel";
     

    public void AddXP(float amount)
    {
        CurrentXP += amount;
        while (CurrentXP >= NextLevelXP)
        {
            CurrentLevel++;
            CurrentXP -= NextLevelXP;
            NextLevelXP += AddXpPerLevel;
            AddXpPerLevel += IncerementXPValue;
        }

        if (AutoSave)
            Save();
    }
    public void Save()
    {
        if (!string.IsNullOrEmpty(KeyToSave))
        {
            PlayerPrefs.SetFloat(KeyToSave + NEXTLEVELXPKEY, NextLevelXP);
            PlayerPrefs.SetFloat(KeyToSave + ADDXPPERLEVELKEY, AddXpPerLevel);
            PlayerPrefs.SetFloat(KeyToSave + CURRENTXPKEY, CurrentXP);
            PlayerPrefs.SetInt(KeyToSave + CURRENTLEVELKEY, CurrentLevel);
        }
    }
    public void Load()
    {
        if (!string.IsNullOrEmpty(KeyToSave))
        {
            if (PlayerPrefs.HasKey(KeyToSave + NEXTLEVELXPKEY))
                NextLevelXP = PlayerPrefs.GetFloat(KeyToSave + NEXTLEVELXPKEY);
            if (PlayerPrefs.HasKey(KeyToSave + ADDXPPERLEVELKEY))
                AddXpPerLevel = PlayerPrefs.GetFloat(KeyToSave + ADDXPPERLEVELKEY);
            if (PlayerPrefs.HasKey(KeyToSave + CURRENTXPKEY))
                CurrentXP = PlayerPrefs.GetFloat(KeyToSave + CURRENTXPKEY);
            if (PlayerPrefs.HasKey(KeyToSave + CURRENTLEVELKEY))
                CurrentLevel = PlayerPrefs.GetInt(KeyToSave + CURRENTLEVELKEY);
        }
    }
}
