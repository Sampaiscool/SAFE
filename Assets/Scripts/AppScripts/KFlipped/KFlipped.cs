using UnityEngine;
using UnityEngine.UI;

public class GridFlip : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public Image baseImage;      // Normal image
        public Image highlightImage; // Highlight overlay
        public Button button;        // Clickable button
        public bool isHighlighted;   // State tracking
    }

    public Cell[] cells; // 25 cells (5x5)
    public Difficulties difficulty;

    private float timer;
    private int gridSize = 5;
    private bool puzzleSolved = false;

    // Overlay sprite
    public Sprite solvedSprite;   // drag your green cell sprite here
    private Image[] solvedOverlays; // generated at runtime

    // Timer values
    public float easyTime = 300f;
    public float mediumTime = 210f;
    public float hardTime = 120f;
    public float crazyTime = 60f;

    // Predefined formations
    private int[][] formations = new int[][]
    {
        new int[] { 12 },
        new int[] { 6, 7, 8, 11, 13, 16, 17, 18 },
        new int[] { 2, 7, 12, 17, 22 },
        new int[] { 10, 11, 12, 13, 14 },
        new int[] { 2, 6, 12, 18, 22},
        new int[] { 4, 9, 10, 12, 14, 15, 20},
        new int [] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 }
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

        // Generate overlays
        solvedOverlays = new Image[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            GameObject overlayGO = new GameObject("SolvedOverlay_" + i);
            overlayGO.transform.SetParent(cells[i].baseImage.transform, false);

            Image overlay = overlayGO.AddComponent<Image>();
            overlay.sprite = solvedSprite;
            overlay.color = Color.green; // tint it green if needed
            overlay.raycastTarget = false; // make sure it doesn't block clicks
            overlay.gameObject.SetActive(false);

            // Stretch overlay to cover cell
            RectTransform rt = overlay.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            solvedOverlays[i] = overlay;
        }

        ResetTimer();
        ApplyRandomFormation();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (puzzleSolved)
            {
                ResetTimer();
                ApplyRandomFormation();
                puzzleSolved = false;
                HideSolvedOverlays();
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
        foreach (var cell in cells)
            SetCellHighlight(cell, false);

        HideSolvedOverlays();

        int[] formation = formations[Random.Range(0, formations.Length)];
        foreach (int index in formation)
            SetCellHighlight(cells[index], true);
    }

    void OnCellClicked(int index)
    {
        if (puzzleSolved) return;

        FlipCell(index);
        FlipCell(index - gridSize);
        FlipCell(index + gridSize);
        if (index % gridSize != 0) FlipCell(index - 1);
        if (index % gridSize != gridSize - 1) FlipCell(index + 1);

        if (CheckIfSolved())
            OnPuzzleSolved();
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
            cell.highlightImage.gameObject.SetActive(state);
    }

    bool CheckIfSolved()
    {
        foreach (var cell in cells)
            if (!cell.isHighlighted) return false;
        return true;
    }

    void OnPuzzleSolved()
    {
        puzzleSolved = true;
        ShowSolvedOverlays();
        Debug.Log("Puzzle Solved!");
    }

    void ShowSolvedOverlays()
    {
        foreach (var overlay in solvedOverlays)
            overlay.gameObject.SetActive(true);
    }

    void HideSolvedOverlays()
    {
        foreach (var overlay in solvedOverlays)
            overlay.gameObject.SetActive(false);
    }
    public void InstantSolveOneRound()
    {
        if (!puzzleSolved)
        {
            // Highlight all cells
            foreach (var cell in cells)
                SetCellHighlight(cell, true);

            // Show solved overlays
            ShowSolvedOverlays();

            puzzleSolved = true;
            Debug.Log("Puzzle instantly solved for this round!");
        }
    }
}
