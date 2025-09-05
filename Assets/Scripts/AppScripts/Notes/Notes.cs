using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notes : MonoBehaviour
{
    [System.Serializable]
    public class NoteUI
    {
        public GameObject root;
        public TMP_Text text;
        public Button editButton;
        public Button deleteButton;
    }

    public TMP_InputField newNoteInput;
    public Button addNoteButton;
    public Transform notesContainer;
    public GameObject notePrefab;

    private List<NoteUI> notes = new List<NoteUI>();

    void Start()
    {
        addNoteButton.onClick.AddListener(AddNote);
    }

    void Update()
    {
        HandleInput();
    }

    void AddNote()
    {
        if (string.IsNullOrWhiteSpace(newNoteInput.text)) return;

        // Create new note UI
        GameObject obj = Instantiate(notePrefab, notesContainer);
        NoteUI ui = new NoteUI
        {
            root = obj,
            text = obj.GetComponentInChildren<TMP_Text>(),
            editButton = obj.transform.Find("EditButton").GetComponent<Button>(),
            deleteButton = obj.transform.Find("DeleteButton").GetComponent<Button>()
        };

        ui.text.text = newNoteInput.text;

        // Button events
        ui.editButton.onClick.AddListener(() => EditNote(ui));
        ui.deleteButton.onClick.AddListener(() => DeleteNote(ui));

        notes.Add(ui);

        newNoteInput.text = "";
    }
    public void AddNoteWithText(string noteText)
    {
        if (string.IsNullOrWhiteSpace(noteText)) return;

        // Create new note UI
        GameObject obj = Instantiate(notePrefab, notesContainer);
        NoteUI ui = new NoteUI
        {
            root = obj,
            text = obj.GetComponentInChildren<TMP_Text>(),
            editButton = obj.transform.Find("EditButton").GetComponent<Button>(),
            deleteButton = obj.transform.Find("DeleteButton").GetComponent<Button>()
        };

        ui.text.text = noteText;

        // Button events
        ui.editButton.onClick.AddListener(() => EditNote(ui));
        ui.deleteButton.onClick.AddListener(() => DeleteNote(ui));

        notes.Add(ui);
    }

    void EditNote(NoteUI note)
    {
        newNoteInput.text = note.text.text;

        DeleteNote(note);
    }

    void DeleteNote(NoteUI note)
    {
        notes.Remove(note);
        Destroy(note.root);
    }

    public void HandleInput()
    {
        if (GameManager.Instance.CurrentControl == AppNames.Notes)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                AddNote();
            }
                
        }
    }
}
