using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    public AudioSource backgroundMusic;

    void Start()
    {
        
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}



