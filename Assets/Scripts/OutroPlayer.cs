using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OutroPlayer : MonoBehaviour
{
    private bool active;
    protected Animator animator;
    protected Fighter winner;
    protected Fighter loser;
    protected PlayerHat hat;
    private int inactiveHandle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartOutroForHat(PlayerHat defenderHat, Fighter winner, Fighter loser)
    {
        this.winner = winner;
        this.loser = loser;
        hat = defenderHat;
        active = true;
        OnOutroStart();
    }

    protected virtual void OnOutroStart()
    {
        inactiveHandle = winner.GetComponent<PlayerStatus>().AddStatusEffect(StatusType.HatInactive);
        winner.ToggleWinFaceAnimation(true);
    }

    public void EndOutroForHat()
    {
        OnOutroEnd();
        active = false;
    }

    protected void StartAnimation()
    {
        animator.StopPlayback();
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