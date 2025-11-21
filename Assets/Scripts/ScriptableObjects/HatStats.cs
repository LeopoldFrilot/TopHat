using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Hat", menuName = "ScriptableObjects/Hats", order = 0)]
    public class HatStats : ScriptableObject
    {
        public string hatName;
        public string hatDescription;
        public Sprite hatIcon;
        public HatMainAbility mainAbilityPrefab;
        public HatMovementAbility movementAbilityPrefab;
    }
}