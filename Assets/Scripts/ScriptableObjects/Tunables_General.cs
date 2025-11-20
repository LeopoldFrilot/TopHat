using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New General Tunables", menuName = "Tunables/General", order = 0)]
    public class Tunables_General : ScriptableObject
    {
        public float maxMeter = 10f;
        
        [Header("Fighter")]
        public float speed = 5f;
        public float verticalJumpPower = 13f;
        public float horizontalJumpPower = 4f;
        public float risingGravity = 3f;
        public float fallingGravity = 8f;
        public float fistsInMotionWalkspeedScalar = .6f;
        public float blockKnockbackPower = 6;
        public float hitKnockbackPower = 13;
        public float grappleResetPower = 20f;
        public float dizzyStunTimeLength = 2.5f;
        public float hardHitThreshold = .65f;
        
        [Header("Fists")]
        public float maxWindup = 11f;
        public float windupSpeed = 8f;
        public float launchStrength = 15f;
        public float retractTime = .3f;
        public float baseWindup = 2.5f;
        public float fistsIdleFollowSpeed = 4f;
        public Vector2 windupOffset = new Vector2(1f, 0);
        public float speedThresholdToRetract = .4f;
        
        [Header("Hat")]
        public float hatIdleFollowSpeed = 4f;
        public float transitionTime = .3f;
        public float transitionArcHeight = 3f;
        public float hatSpinSpeed = 1000f;
        public float timeToChargeHat = 30f;
        
        [Header("Meter")]
        public float meterRequirementGrapple = 3f;
        public float meterForParry = 1f;
        public float meterForBlock = .5f;
        public float meterForHitBlock_Hard = .5f;
        public float meterForHit = 1f;
        public float meterForHit_Hard = 1.5f;

        [Header("Dodge")]
        public float dodgeDistance = 7f;
        public float dodgeTime = .3f;
        public float dodgeSmoothingCoef = .2f;
        public float dodgeRollCD = 0.5f;
        
        [Header("Dizzy")]
        public float maxDizziness = 5f;
        public float dizzyRecoveryRate = 2f;
        public float delayBeforeRecovery = 1.5f;
        public float blockDizzyDamageReduction = .33f;
        
        [Header("Block")]
        public float blockingTime = 0.5f;
        public float perfectBlockTime = .2f;
        public float blockingCooldown = 0.5f;
    
        [Header("Juice")]
        public float knockOffHitstop = 1f;
        public float punchHitstop = .3f;
        public float hitStrongHitstop = .45f;
        public float hitStrongestHitstop = .6f;
        public float blockHitstop = .1f;
        public float blockStrongHitstop = .15f;
        public float blockStrongestHitstop = .2f;
        public float parryHitstop = .01f;
        public float hitTimeRecoveryRate = .1f;
        public float swapTimeRecoveryRate = .03f;
    }
}