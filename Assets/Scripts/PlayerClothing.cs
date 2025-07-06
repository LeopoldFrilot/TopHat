using System.Collections.Generic;
using UnityEngine;

public class PlayerClothing : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private Sprite player1ShirtSprite;
    [SerializeField] private Sprite player1InFistSprite;
    [SerializeField] private Sprite player1OutFistSprite;
    [SerializeField] private Sprite player2ShirtSprite;
    [SerializeField] private Sprite player2InFistSprite;
    [SerializeField] private Sprite player2OutFistSprite;

    public void SetPlayerIndex(int playerIndex, List<PlayerFist> playerFists)
    {
        if (playerIndex == 1)
        {
            bodySprite.sprite = player1ShirtSprite;
            playerFists[0].GetComponent<PlayerFistUI>().SetSprite(player1OutFistSprite);
            playerFists[1].GetComponent<PlayerFistUI>().SetSprite(player1InFistSprite);
        }
        else
        {
            bodySprite.sprite = player2ShirtSprite;
            playerFists[0].GetComponent<PlayerFistUI>().SetSprite(player2OutFistSprite);
            playerFists[1].GetComponent<PlayerFistUI>().SetSprite(player2InFistSprite);
        }
    }
}