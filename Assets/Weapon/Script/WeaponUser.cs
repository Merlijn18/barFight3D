using UnityEngine;

public class WeaponUser : MonoBehaviour
{
    public Weapon weapon;

    public void OnFire()
    {
        if (weapon != null)
            weapon.Shoot();
    }
}


