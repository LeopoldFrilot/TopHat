using UnityEngine;

public class PlayerFistUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    public void SetSprite(Sprite fistSprite)
    {
        sprite.sprite = fistSprite;
    }

    public void FaceRight(bool faceRight)
    {
        sprite.transform.localScale = new Vector3(1f, faceRight ? 1f : -1f, 1f);
    }
}