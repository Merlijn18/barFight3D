using UnityEngine;

public class DoubleDoorTrigger : MonoBehaviour
{
    public DoorController doorLeft;       // Link naar linker deur
    public DoorController doorRight;      // Link naar rechter deur

    private int enemiesPassed = 0;         // Telt vijanden die door trigger gaan
    private int enemiesToPass = 0;         // Aantal vijanden dat moet passeren (wave grootte)

    // Wordt door WaveManager aangeroepen om aantal vijanden te zetten
    public void SetEnemiesToPass(int count)
    {
        enemiesToPass = count;
        enemiesPassed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesPassed++;
            Debug.Log("Enemy passed door trigger: " + enemiesPassed + "/" + enemiesToPass);

            if (enemiesPassed >= enemiesToPass)
            {
                doorLeft.CloseDoor();
                doorRight.CloseDoor();
                Debug.Log("Alle vijanden gepasseerd, deuren sluiten");
            }
        }
    }

    // Open beide deuren tegelijk
    public void OpenDoors()
    {
        doorLeft.OpenDoor();
        doorRight.OpenDoor();
        Debug.Log("Deuren openen");
    }
}
