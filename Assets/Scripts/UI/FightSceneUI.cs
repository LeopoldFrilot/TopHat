using System;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightSceneUI : MonoBehaviour
{
    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;
    [SerializeField] private Image hatColorSprite;
    [SerializeField] private Color neutralColor;
    [SerializeField] private Color player1Color;
    [SerializeField] private Color player2Color;
    [SerializeField] private DOTweenAnimation flashAnimation;
    [SerializeField] private string colorFlashID;

    public void UpdatePointsText(float player1HatTime, float player2HatTime, float maxHatTime)
    {
        float clampedP1 = Mathf.Clamp(player1HatTime, 0, maxHatTime);
        float clampedP2 = Mathf.Clamp(player2HatTime, 0, maxHatTime);
        player1PointsSlider.value = clampedP1/maxHatTime;
        player2PointsSlider.value = clampedP2/maxHatTime;

        if (clampedP2 >= maxHatTime)
        {
            hatColorSprite.color = player2Color;
        }
        else if (clampedP1 >= maxHatTime)
        {
            hatColorSprite.color = player1Color;
        }
        else
        {
            hatColorSprite.color = neutralColor;
        }

        if (clampedP1/maxHatTime >= .8 || clampedP2/maxHatTime >= .8)
        {
            flashAnimation.DOPlayAllById(colorFlashID);
        }
        else
        {
            flashAnimation.DORewindAllById(colorFlashID);
            flashAnimation.DOPauseAllById(colorFlashID);
        }
    }
}