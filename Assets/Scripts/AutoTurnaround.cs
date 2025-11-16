using UnityEngine;

public class AutoTurnaround : MonoBehaviour
{
    private FightScene fightScene;
    private Fighter _fighterRef;

    private void Awake()
    {
        fightScene = FindFirstObjectByType<FightScene>();
        _fighterRef = GetComponent<Fighter>();
    }

    private void Update()
    {
        var otherPlayer = fightScene.GetOpponent(_fighterRef);
        if (otherPlayer == null)
        {
            return;
        }

        if (!_fighterRef.CanMove())
        {
            return;
        }
        
        if (transform.position.x < otherPlayer.transform.position.x)
        {
            _fighterRef.FaceRight();
        }
        else
        {
            _fighterRef.FaceLeft();
        }
    }
}