// Utils class provides utility functions for game operations.
// Utils s�n�f�, oyun i�lemleri i�in yard�mc� i�levler sa�lar.
using UnityEngine;

public static class Utils
{
    // Reference to the GameResources instance.
    // GameResources �rne�ine bir referans.
    public static GameResources gameResources;

    // Initialize the GameResources instance.
    // GameResources �rne�ini ba�lat�r.
    public static GameResources InitResources()
    {
        // Load the GameResources asset from the Resources folder and assign it to gameResources.
        // Resources klas�r�nden GameResources varl���n� y�kler ve gameResources'e atar.
        return gameResources = Resources.Load<GameResources>("GameResources");
    }

    // Get the visual sprite associated with an item by its unique identifier.
    // Bir ��enin benzersiz tan�mlay�c�s�na g�re ili�kili g�r�nt� sprite'�n� al�r.
    public static Sprite GetItemVisualById(int itemId)
    {
        // Return the sprite associated with the given itemId from the gameResources.
        // gameResources'tan verilen itemId ile ili�kilendirilmi� sprite'� d�nd�r�r.
        return gameResources.items[itemId];
    }
}
