// Utils class provides utility functions for game operations.
// Utils sýnýfý, oyun iþlemleri için yardýmcý iþlevler saðlar.
using UnityEngine;

public static class Utils
{
    // Reference to the GameResources instance.
    // GameResources örneðine bir referans.
    public static GameResources gameResources;

    // Initialize the GameResources instance.
    // GameResources örneðini baþlatýr.
    public static GameResources InitResources()
    {
        // Load the GameResources asset from the Resources folder and assign it to gameResources.
        // Resources klasöründen GameResources varlýðýný yükler ve gameResources'e atar.
        return gameResources = Resources.Load<GameResources>("GameResources");
    }

    // Get the visual sprite associated with an item by its unique identifier.
    // Bir öðenin benzersiz tanýmlayýcýsýna göre iliþkili görüntü sprite'ýný alýr.
    public static Sprite GetItemVisualById(int itemId)
    {
        // Return the sprite associated with the given itemId from the gameResources.
        // gameResources'tan verilen itemId ile iliþkilendirilmiþ sprite'ý döndürür.
        return gameResources.items[itemId];
    }
}
