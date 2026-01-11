using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitScript : MonoBehaviour
{
        public void GoToMainMenu()
        {
            Time.timeScale = 1f;   //  THIS IS THE IMPORTANT LINE

        SceneManager.LoadScene("Menu");
        }
}