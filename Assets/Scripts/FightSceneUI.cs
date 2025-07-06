using System;
using TMPro;
using UnityEngine;
public class FightSceneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1PointsText;
    [SerializeField] private TextMeshProUGUI player2PointsText;

    public void UpdatePointsText(int p1points, int p2points)
    {
        player1PointsText.text = "Hats topped: " + p1points;
        player2PointsText.text = "Hats topped: " + p2points;
    }
}