using UnityEngine;
using UnityEngine.UI;

public class GridFlip : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public Image baseImage;      // Normal image
        public Image highlightImage; // Overlay
        public Button button;        // Clickable button
        public bool isHighlighted;   // State tracking
    }

    public Cell[] cells; // 25 cells (5x5)
    public Difficulties difficulty;

    private float timer;
    private int gridSize = 5;

    // Timer values
    public float easyTime = 300f;
    public float mediumTime = 210f;
    public float hardTime = 120f;
    public float crazyTime = 60f;

    // Predefined formations (you can tweak these)
    private int[][] formations = new int[][]
    {
        new int[] { 12 },                                                    // center only
        new int[] { 6, 7, 8, 11, 13, 16, 17, 18 },                           // cross around center
        new int[] { 2, 7, 12, 17, 22 },                                      // vertical line
        new int[] { 10, 11, 12, 13, 14 },                                    // horizontal line
        new int[] { 2, 6, 12, 18, 22},                                       // zigzag line
        new int[] { 4, 9, 10, 12, 14, 15, 20},                               // short line + center
        new int [] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 }         // skips
    };

    void Start()
    {
        difficulty = GameManager.Instance.difficulty;

        // Hook up button listeners
        for (int i = 0; i < cells.Length; i++)
        {
            int index = i;
            cells[i].button.onClick.AddListener(() => OnCellClicked(index));
        }

        ResetTimer();
        ApplyRandomFormation();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (CheckIfSolved())
        {
            timer = 0f;
        }

        if (timer <= 0f)
        {
            if (CheckIfSolved())
            {
                ResetTimer();
                ApplyRandomFormation();
            }
            else
            {
                GameManager.Instance.PlayerLost(DeathReason.KFlipped_NotFinished);
                enabled = false;
            }
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

    void ApplyRandomFormation()
    {
        // Clear all
        foreach (var cell in cells)
            SetCellHighlight(cell, false);

        // Pick a random formation
        int[] formation = formations[Random.Range(0, formations.Length)];

        foreach (int index in formation)
            SetCellHighlight(cells[index], true);
    }

    void OnCellClicked(int index)
    {
        // Flip self + neighbors
        FlipCell(index); // self
        FlipCell(index - gridSize); // up
        FlipCell(index + gridSize); // down
        if (index % gridSize != 0) FlipCell(index - 1); // left
        if (index % gridSize != gridSize - 1) FlipCell(index + 1); // right
    }

    void FlipCell(int index)
    {
        if (index < 0 || index >= cells.Length) return;
        SetCellHighlight(cells[index], !cells[index].isHighlighted);
    }

    void SetCellHighlight(Cell cell, bool state)
    {
        cell.isHighlighted = state;
        if (cell.highlightImage != null)
            cell.highlightImage.gameObject.SetActive(state); // enable/disable the child GameObject
    }

    bool CheckIfSolved()
    {
        foreach (var cell in cells)
            if (!cell.isHighlighted) return false;
        return true;
    }
}
