using System.Collections;
using UnityEngine;

namespace Hats
{
    public class OutroPlayer_DefaultHat : OutroPlayer
    {
        [SerializeField] private Transform hatRoot;
        [SerializeField] private GameObject laserPrefab;
        
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
        }
        
        public void FinishFireLaser()
        {
            Destroy(spawnedLaser);
            loser.HidePlayer();
        }
    }
}