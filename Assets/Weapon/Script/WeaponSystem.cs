using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapons")]
    public Weapon fist; // altijd beschikbaar
    public Weapon currentWeapon;
    public Weapon[] weapons; // optioneel voor andere wapens

    private int currentIndex = -1; // -1 = fist

    void Start()
    {
        // Fist altijd actief
        if (fist != null)
            fist.gameObject.SetActive(true);

        currentWeapon = fist;
    }

    public void OnFire()
    {
        if (currentWeapon != null)
            currentWeapon.Shoot();
    }

    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        // deactivate current weapon if it's not fist
        if (currentWeapon != null && currentWeapon != fist)
            currentWeapon.gameObject.SetActive(false);

        currentWeapon = weapons[index];
        currentWeapon.gameObject.SetActive(true);
        currentIndex = index;
    }

    public void DropWeapon()
    {
        if (currentWeapon != fist)
        {
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = fist;
            currentIndex = -1;
        }
    }

    public void NextWeapon()
    {
        if (weapons.Length == 0)
            return;

        int nextIndex = (currentIndex + 1) % weapons.Length;
        SwitchWeapon(nextIndex);
    }
}
