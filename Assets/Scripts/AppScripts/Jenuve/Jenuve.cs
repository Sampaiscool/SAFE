using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Jenuve : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public Image baseImage;
        public Image highlightImage;
    }

    public Cell[] cells; // 9 cells in the grid
    public Transform player;
    public float moveSpeed = 5f;

    public float easyTime = 300f;   // 5 minutes
    public float mediumTime = 210f; // 3.5 minutes
    public float hardTime = 120f;   // 2 minutes
    public float crazyTime = 60f;   // 1 minute

    // Glitch mode times
    public float easyGlitchTime = 10f;
    public float mediumGlitchTime = 8f;
    public float hardGlitchTime = 6f;
    public float crazyGlitchTime = 4f;

    public Difficulties difficulty;

    private float timer;
    private int targetIndex;
    private int playerIndex;
    private Vector3 targetPlayerPos;

    private bool glitchActive = false;
    private float glitchTimer;
    private HashSet<int> glitchCells = new HashSet<int>();

    void Start()
    {
        difficulty = GameManager.Instance.difficulty;
        timer = GetMaxTime();

        MovePlayerInstant(4);
        RandomizeTarget(-1);
    }

    void Update()
    {
        HandleInput();

        player.position = Vector3.MoveTowards(player.position, targetPlayerPos, moveSpeed * Time.deltaTime);

        if (glitchActive)
        {
            GlitchUpdate();
            return; // Pause normal minigame while glitch is active
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (playerIndex == targetIndex)
            {
                ResetTimer();
                RandomizeTarget(targetIndex);
            }
            else
            {
                Debug.Log("Wrong cell!");
                ResetTimer();
                RandomizeTarget(targetIndex);
            }
        }
    }

    void HandleInput()
    {
        int row = playerIndex / 3;
        int col = playerIndex % 3;

        if (Input.GetKeyDown(KeyCode.W) && row > 0) MovePlayer(playerIndex - 3);
        if (Input.GetKeyDown(KeyCode.S) && row < 2) MovePlayer(playerIndex + 3);
        if (Input.GetKeyDown(KeyCode.A) && col > 0) MovePlayer(playerIndex - 1);
        if (Input.GetKeyDown(KeyCode.D) && col < 2) MovePlayer(playerIndex + 1);

        if (glitchActive)
            CheckGlitchCell(); // Check if player steps on glitch cell
    }

    void MovePlayer(int newIndex)
    {
        playerIndex = newIndex;
        targetPlayerPos = cells[playerIndex].baseImage.transform.position;
    }

    void MovePlayerInstant(int newIndex)
    {
        playerIndex = newIndex;
        targetPlayerPos = cells[playerIndex].baseImage.transform.position;
        player.position = targetPlayerPos;
    }

    void RandomizeTarget(int oldIndex)
    {
        do
        {
            targetIndex = Random.Range(0, cells.Length);
        }
        while (targetIndex == oldIndex);

        HighlightCell();
    }

    void HighlightCell()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].highlightImage.gameObject.SetActive(i == targetIndex);
        }
    }

    void ResetTimer()
    {
        timer = GetMaxTime();
    }

    float GetMaxTime()
    {
        return difficulty == Difficulties.Easy ? easyTime :
               difficulty == Difficulties.Medium ? mediumTime :
               difficulty == Difficulties.Hard ? hardTime : crazyTime;
    }

    public void JenuveGlitch()
    {
        glitchActive = true;
        glitchCells.Clear();

        int minCells = 2;
        int maxCells = 8;

        switch (difficulty)
        {
            case Difficulties.Easy: glitchTimer = easyGlitchTime; maxCells = 3; break;
            case Difficulties.Medium: glitchTimer = mediumGlitchTime; maxCells = 4; break;
            case Difficulties.Hard: glitchTimer = hardGlitchTime; maxCells = 6; break;
            case Difficulties.Crazy: glitchTimer = crazyGlitchTime; maxCells = 8; break;
        }

        int numCells = Random.Range(minCells, maxCells + 1);

        while (glitchCells.Count < numCells)
        {
            int idx = Random.Range(0, cells.Length);
            glitchCells.Add(idx);
        }

        // Highlight glitch cells
        for (int i = 0; i < cells.Length; i++)
            cells[i].highlightImage.gameObject.SetActive(glitchCells.Contains(i));
    }

    void GlitchUpdate()
    {
        glitchTimer -= Time.deltaTime;

        if (glitchCells.Count == 0)
        {
            Debug.Log("Glitch cleared!");
            glitchActive = false;
            ResetTimer();
            RandomizeTarget(-1);
        }
        else if (glitchTimer <= 0f)
        {
            Debug.Log("Glitch failed - You Lose!");
            glitchActive = false;
            enabled = false;
        }
    }

    void CheckGlitchCell()
    {
        if (glitchCells.Contains(playerIndex))
        {
            glitchCells.Remove(playerIndex);
            cells[playerIndex].highlightImage.gameObject.SetActive(false);
        }
    }
}
