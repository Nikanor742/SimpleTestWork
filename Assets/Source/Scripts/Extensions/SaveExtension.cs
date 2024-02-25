using UnityEngine;

public class SaveExtension
{
    public static PlayerData player;
    public const string playerDataKey = "PlayerData";

    public static void SavePlayerData()
    {
        var @string = JsonUtility.ToJson(player);
        PlayerPrefs.SetString(playerDataKey, @string);
    }

    public static void Init()
    {
        player = new PlayerData();
        if (PlayerPrefs.HasKey(playerDataKey))
        {
            var @string = PlayerPrefs.GetString(playerDataKey);
            JsonUtility.FromJsonOverwrite(@string, player);
        }
    }
}