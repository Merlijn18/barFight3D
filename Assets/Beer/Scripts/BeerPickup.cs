using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class BeerPickup : MonoBehaviour
{
    [Header("UI")]
    public Text promptText;

    [Header("Health")]
    public int healAmount = 50;

    private bool inRange = false;
    private PlayerBeerSystem player;
    private PlayerHealth playerHealth;

    private void Reset()
    {
        // Zorg dat de collider trigger is
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Update()
    {
        if (inRange && player != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            player.DrinkBeer();

            if (playerHealth != null)
                playerHealth.Heal(healAmount);

            if (BeerSpawnManager.Instance != null)
                BeerSpawnManager.Instance.RespawnBeer(transform.position, transform.rotation, 10f);

            if (promptText != null)
                promptText.text = "";

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        PlayerHealth h = other.GetComponent<PlayerHealth>();

        if (p != null)
        {
            inRange = true;
            player = p;
            playerHealth = h;

            if (promptText != null)
                promptText.text = $"Druk [E] om bier te drinken (+{healAmount} HP)";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        if (p != null)
        {
            inRange = false;
            player = null;
            playerHealth = null;

            if (promptText != null)
                promptText.text = "";
        }
    }
}
