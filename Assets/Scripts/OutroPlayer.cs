using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OutroPlayer : MonoBehaviour
{
    private bool active;
    protected Animator animator;
    protected Fighter winner;
    protected Fighter loser;
    protected PlayerStatus status;
    protected PlayerHat hat;
    private int inactiveHandle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartOutroForHat(PlayerHat defenderHat, Fighter winner, Fighter loser)
    {
        this.winner = winner;
        status = winner.GetComponent<PlayerStatus>();
        this.loser = loser;
        hat = defenderHat;
        active = true;
        OnOutroStart();
    }

    protected virtual void OnOutroStart()
    {
        inactiveHandle = status.AddStatusEffect(StatusType.HatInactive);
        winner.ToggleWinFaceAnimation(true);
    }

    public void EndOutroForHat()
    {
        OnOutroEnd();
        active = false;
    }

    protected virtual void OnOutroEnd()
    {
        winner.GetComponent<PlayerStatus>().RemoveStatusEffect(inactiveHandle);
        winner.ToggleWinFaceAnimation(false);
    }

    public bool IsInactive()
    {
        return !active;
    }
}