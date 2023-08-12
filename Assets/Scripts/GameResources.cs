using UnityEngine;
using System.Collections.Generic;

// GameResources class is a ScriptableObject that holds references to in-game resources.
// GameResources sýnýfý, oyun içi kaynaklara referanslarý tutan bir ScriptableObject'týr.
[CreateAssetMenu]
public class GameResources : ScriptableObject
{
    // List of sprites representing different items in the game.
    // Oyundaki farklý öðeleri temsil eden sprite'larýn listesi.
    public List<Sprite> items = new List<Sprite>();
}
