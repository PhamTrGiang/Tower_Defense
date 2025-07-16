using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class UI_EnemiesKilledText : MonoBehaviour
{
    private TextMeshPro myText;
    private GameManager gameManager;

    private void Awake()
    {
        myText = GetComponent<TextMeshPro>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        myText.text = "Enemies killed: "+gameManager.EnemiesKilled;
    }
}
