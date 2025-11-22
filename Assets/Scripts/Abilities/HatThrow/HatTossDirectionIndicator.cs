using UnityEngine;

public class HatTossDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Transform rotationRoot;

    public void UpdateArrowRotation(float newAngle)
    {
        rotationRoot.localRotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
    }
}