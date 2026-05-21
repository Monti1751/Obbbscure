using UnityEngine;

public enum PickupType { Health, Ammo }

public class Pickup
{
    public Vector2 pos;
    public PickupType type;
    public bool collected = false;

    public int healthAmount = 25;
    public int ammoAmount = 10;

    public Pickup(Vector2 pos, PickupType type)
    {
        this.pos = pos;
        this.type = type;
    }
}