using UnityEngine;

public class Slot : MonoBehaviour
{
    public Turret currentTurret;

    [Header("Chỉ dành cho Combat Slot")]
    // Mặc định là -1 (nghĩa là ô này nằm ở dưới MergeZone, không phải ô chiến đấu)
    // Nếu ô này là 5 cái ô trên CombatZone, bạn gán số từ 0 đến 4 trên Inspector.
    public int slotLaneID = -1;

    public bool IsEmpty()
    {
        return currentTurret == null;
    }
}