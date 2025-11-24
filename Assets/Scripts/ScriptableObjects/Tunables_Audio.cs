using MilkShake;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Tunables", menuName = "ScriptableObjects/Tunables/Audio", order = 0)]
    public class Tunables_Audio : ScriptableObject
    {
        [Header("Music")] 
        public Audio mainMenu;
        public Audio context;
        public Audio tutorial;
        public Audio fight;
        
        [Header("UI")]
        public Audio buttonClick;
        public Audio buttonHover;

        [Header("Ambient")] 
        public Audio crowdLoop;
        public Audio nightAmbienceLoop;
        public Audio streetLightAmbience;
        
        [Header("SFX")]
        public Audio playerEnterSound;
        public Audio punchChargeUp;
        public Audio punchSuperCharged;
        public Audio punchReleased;
        public Audio fistHit;
        public Audio fistGotBlocked;
        public Audio firstGotParried;
        public Audio hatPickup;
        public Audio hatKnockedOff;
        public Audio playerLanded;
        public Audio playerStunned;
        
        [Header("Fighter grunts")]
        public Audio playerEnterGrunt;
        public Audio walkLoop;
        public Audio fighterHitGrunt;
        public Audio fighterBlockGrunt;
        public Audio fighterParryGrunt;
        public Audio fighterDeath;
        public Audio fighterStunnedGrunt;
        public Audio playerWin;

        [Header("Ability specific")] 
        public Audio dash;
        public Audio hatTossHold;
        public Audio hatTossRelease;
        public Audio hatTossBounce;
        public Audio hatTossCrash;
        public Audio hatTossReturn;
        public Audio dashCancel;
        public Audio grappleStart;
        public Audio grappleConnected;

        [Header("Outro specific")] 
        public Audio hatChargeLaser;
        public Audio hatReleaseLaser;
        
        [Header("Misc")]
        public Audio contextPageFlip;
        public Audio readySetGo;
        public Audio winner;
    }