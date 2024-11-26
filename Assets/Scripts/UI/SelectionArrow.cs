using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;
using System;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    [SerializeField] private AudioClip changeSound; // When up and down
    [SerializeField] private AudioClip interactSound; // When select

    private RectTransform rect;
    private int currentPosition;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        // Define voice commands and their corresponding actions
        actions.Add("up", () => ChangePosition(-1));
        actions.Add("down", () => ChangePosition(1));
        actions.Add("select", Interact);

        // Initialize the KeywordRecognizer
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void Update()
    {
        // Keyboard Controls
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(1);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void ChangePosition(int _change)
    {
        currentPosition += _change;

        if (_change != 0)
        {
            SoundManager.instance.PlaySound(changeSound);
        }

        // Loop around the options
        if (currentPosition < 0)
        {
            currentPosition = options.Length - 1;
        }
        else if (currentPosition > options.Length - 1)
        {
            currentPosition = 0;
        }

        rect.position = new Vector3(rect.position.x, options[currentPosition].position.y, 0);
    }

    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);

        // Access the button at the current position
        options[currentPosition].GetComponent<Button>().onClick.Invoke();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Recognized: {speech.text}");
        if (actions.ContainsKey(speech.text))
        {
            actions[speech.text].Invoke();
        }
    }

    private void OnDestroy()
    {
        // Stop and dispose of the KeywordRecognizer to release resources
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}
