using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string objname;
    public string description;
    public Sprite image;

    public int hpGiven;
    public int speedGiven;
    public float speedDuration;

    public bool isExplosive;
    public bool isWeapon;
    public bool isKatana;

    public int ammo;

}
