using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirewallBreak : MonoBehaviour
{
    public GameObject playerCatcher; // Het object dat de speler beweegt
    public GameObject blockPrefab; // De blokjes die vallen
    public float spawnDifference = 0.5f; // Pas deze waarde aan in de Inspector
    public Transform[] spawnPoints; // 4 spawnpunten voor de blokjes
    public float[] spawnIntervals; // Interval per moeilijkheidsgraad
    public float[] fallSpeeds; // Snelheid van de blokjes per moeilijkheidsgraad
    public int[] blocksToCatch; // Aantal blokjes om te vangen per moeilijkheidsgraad

    private int currentScore;
    private Difficulties currentDifficulty;
    private Coroutine gameCoroutine;

    // Start de mini-game met de gekozen moeilijkheidsgraad
    public void StartGame()
    {
        // Reset de game staat
        currentScore = 0;
        gameObject.SetActive(true);

        GameManager.Instance.glitchManager.isFreezeActive = true;

        currentDifficulty = GameManager.Instance.difficulty;
        gameCoroutine = StartCoroutine(GameLoop());
    }

    // De hoofdlus van de game
    private IEnumerator GameLoop()
    {
        float spawnTimer = 0f;
        // Gebruik een baseInterval in plaats van een vaste spawnInterval
        float baseInterval = spawnIntervals[(int)currentDifficulty];
        float fallSpeed = fallSpeeds[(int)currentDifficulty];

        // ...

        while (currentScore < blocksToCatch[(int)currentDifficulty])
        {
            spawnTimer += Time.deltaTime;

            // Voeg een willekeurige variatie toe aan de spawnInterval
            float effectiveSpawnInterval = baseInterval + Random.Range(-spawnDifference, spawnDifference);

            if (spawnTimer >= effectiveSpawnInterval)
            {
                // Kies een willekeurig spawn-punt
                int spawnIndex = Random.Range(0, spawnPoints.Length);
                GameObject newBlock = Instantiate(blockPrefab, spawnPoints[spawnIndex].position, Quaternion.identity, spawnPoints[spawnIndex]);

                // Stel de valsnelheid van het blokje in
                newBlock.GetComponent<FallingBlock>().fallSpeed = fallSpeed;
                newBlock.GetComponent<FallingBlock>().firewallBreak = this; // Geef het script door

                // Reset de timer
                spawnTimer = 0;
            }

            yield return null; // Wacht een frame
        }

        GameWon();
    }

    // Wordt aangeroepen door de FallingBlock als een blokje gevangen is
    public void BlockCaught()
    {
        currentScore++;
    }

    public void GameOver()
    {
        Debug.Log("Player has lost to the firewall"); // noob

        GameManager.Instance.glitchManager.isFreezeActive = false;
        GameManager.Instance.EyeControl.AfterFirewallGame(false);

        // Reset the game completely
        ResetGame();
    }

    private void GameWon()
    {
        GameManager.Instance.glitchManager.isFreezeActive = false;
        GameManager.Instance.EyeControl.AfterFirewallGame(true);

        // Reset the game so it can start fresh next time
        ResetGame();
    }

    private void ResetGame()
    {
        // Stop any running coroutine
        if (gameCoroutine != null)
            StopCoroutine(gameCoroutine);

        // Reset score
        currentScore = 0;

        // Destroy all existing blocks under all spawn points
        foreach (var sp in spawnPoints)
        {
            foreach (Transform child in sp)
            {
                Destroy(child.gameObject);
            }
        }

        // Reset freeze
        GameManager.Instance.glitchManager.isFreezeActive = false;

        // Optionally hide the mini-game UI
        gameObject.SetActive(false);
    }

}