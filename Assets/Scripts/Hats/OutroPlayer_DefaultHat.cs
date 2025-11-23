using System.Collections;
using MilkShake;
using UnityEngine;

public class OutroPlayer_DefaultHat : OutroPlayer
{
    [SerializeField] private Transform hatRoot;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private ShakePreset laserBeamCameraShake;
    [SerializeField] private float delayBeforeLaserScreenShake = .5f;
    [SerializeField] private float screenShakeFadeTime = .1f;

    private ShakeInstance laserShake1;
    private ShakeInstance laserShake2;
        
    private GameObject spawnedLaser;
    protected override void OnOutroStart()
    {
        base.OnOutroStart();
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        hatRoot.transform.position = winner.GetHatLocation().position;
        yield return null;
        animator.SetTrigger("Start");
    }

    public void StartFireLaser()
    {
        Vector3 direction = (loser.GetArtRoot().position - winner.GetHatLocation().position).normalized;
        float angle = Quaternion.Angle(Quaternion.LookRotation(Vector3.right, Vector3.forward),
            Quaternion.LookRotation(direction, Vector3.forward));
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, -angle)) ;
        spawnedLaser = Instantiate(laserPrefab, winner.GetHatLocation().position, rotation, transform);
        laserShake1 = Shaker.ShakeAll(laserBeamCameraShake);
        StartCoroutine(StartShake2());
    }

    private IEnumerator StartShake2()
    {
        yield return new WaitForSeconds(delayBeforeLaserScreenShake);
        laserShake2 = Shaker.ShakeAll(laserBeamCameraShake);
    }
        
    public void FinishFireLaser()
    {
        laserShake1.Stop(screenShakeFadeTime, true);
        laserShake2.Stop(screenShakeFadeTime, true);
        Destroy(spawnedLaser);
        loser.HidePlayer();
    }
}