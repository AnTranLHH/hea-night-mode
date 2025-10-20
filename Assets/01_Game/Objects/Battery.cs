using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField]
    private float _addedDuration;

    public float GetBatteryDuration()
    {
        return _addedDuration;
    }
}
