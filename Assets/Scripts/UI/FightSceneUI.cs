using System;
using TMPro;
using UnityEngine;
public class FightSceneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1PointsText;
    [SerializeField] private TextMeshProUGUI player2PointsText;

    public void UpdatePointsText(float player1HatTime, float player2HatTime, float maxHatTime)
    {
        player1PointsText.text = $"{Mathf.FloorToInt(Mathf.Clamp(player1HatTime, 0, maxHatTime))}/{maxHatTime}";
        player2PointsText.text = $"{Mathf.FloorToInt(Mathf.Clamp(player2HatTime, 0, maxHatTime))}/{maxHatTime}";
    }
}