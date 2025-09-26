using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Nieuwe Input System

[RequireComponent(typeof(Collider))]
public class BeerPickup : MonoBehaviour
{
    [Header("UI")]
    public Text promptText;

    private bool inRange = false;
    private PlayerBeerSystem player;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Update()
    {
        if (inRange && player != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            player.DrinkBeer();      // Start dronken effect
            Destroy(gameObject);     // Bier verdwijnt

            if (promptText != null)
                promptText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        if (p != null)
        {
            inRange = true;
            player = p;

            if (promptText != null)
                promptText.text = "Druk [E] om bier te drinken";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        if (p != null)
        {
            inRange = false;
            player = null;

            if (promptText != null)
                promptText.text = "";
        }
    }
}
