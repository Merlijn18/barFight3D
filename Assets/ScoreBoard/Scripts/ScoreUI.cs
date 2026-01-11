using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    void Update()
    {
        scoreText.text = ScoreManager.instance.score.ToString();
    }
}
