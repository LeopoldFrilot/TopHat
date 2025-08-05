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
        Debug.Log($"{transform.root.gameObject}: {faceRight}");
        sprite.transform.localScale = new Vector3(
            (faceRight ? 1f : -1f) * Mathf.Abs(sprite.transform.localScale.x), 
            sprite.transform.localScale.y, 
            sprite.transform.localScale.z);
    }
}