using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioSource backgroundMusic;

    void Start()
    {
        // ✅ Zorgt dat alles opnieuw werkt
        Time.timeScale = 1f;

        // ✅ Cursor altijd zichtbaar en vrij in menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ✅ Muziek correct starten (geen dubbele)
        if (backgroundMusic != null)
        {
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.loop = true;
                backgroundMusic.Play();
            }
        }
    }

    public void StartGame()
    {
        // ✅ Cursor locken voor gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
