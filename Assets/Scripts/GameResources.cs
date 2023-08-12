using UnityEngine;
using System.Collections.Generic;

// GameResources class is a ScriptableObject that holds references to in-game resources.
// GameResources s�n�f�, oyun i�i kaynaklara referanslar� tutan bir ScriptableObject't�r.
[CreateAssetMenu]
public class GameResources : ScriptableObject
{
    // List of sprites representing different items in the game.
    // Oyundaki farkl� ��eleri temsil eden sprite'lar�n listesi.
    public List<Sprite> items = new List<Sprite>();
}
