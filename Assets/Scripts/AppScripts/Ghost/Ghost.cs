using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ghost : MonoBehaviour
{
    [Header("UI")]
    public Slider silenceMeter;      // De meter die de stilte weergeeft

    [Header("Settings per Difficulty (Easy=0, Medium=1, Hard=2, Crazy=3)")]
    public float[] maxMeters = new float[4];        // Max waarde van de meter
    public float[] durationSeconds = new float[4];  // Hoeveel seconden tot max 
    public float[] glitchAmounts = new float[4];     // Hoeveel de meter wordt tidjens een glitch

    private float maxMeter;
    private float currentMeter;
    private float decayRate;
    private float glitchAmount;
    private Difficulties currentDifficulty;
    private Coroutine gameCoroutine;
    private bool gameActive = false;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentDifficulty = GameManager.Instance.difficulty;

        // Init maxMeter en decayRate
        maxMeter = maxMeters[(int)currentDifficulty];
        decayRate = maxMeter / durationSeconds[(int)currentDifficulty];  // Bereken decay automatisch
        currentMeter = 0f;
        silenceMeter.value = currentMeter / maxMeter;

        gameActive = true;

        if (gameCoroutine != null)
            StopCoroutine(gameCoroutine);

        gameCoroutine = StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (gameActive)
        {
            currentMeter += decayRate * Time.deltaTime;  // Automatische decay
            silenceMeter.value = currentMeter / maxMeter;

            if (currentMeter >= maxMeter)
            {
                GameOver();
            }

            yield return null;
        }
    }

    public void DialRotated(float rotationAmount)
    {
        currentMeter -= rotationAmount * 0.1f; // Pas factor aan om snelheid te balancen
        currentMeter = Mathf.Clamp(currentMeter, 0f, maxMeter);
        silenceMeter.value = currentMeter / maxMeter;
    }

    public void GhostGlitch()
    {
        glitchAmount = glitchAmounts[(int)currentDifficulty];

        currentMeter = glitchAmount;
        currentMeter = Mathf.Clamp(currentMeter, 0f, maxMeter);
        silenceMeter.value = currentMeter / maxMeter;
    }

    private void GameOver()
    {
        gameActive = false;
    }
}
