using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using System;

public class QuitUI : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {
        // Define the voice command and its corresponding action
        actions.Add("quit", QuitGame);

        // Initialize the KeywordRecognizer
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Recognized: {speech.text}");
        if (actions.ContainsKey(speech.text))
        {
            actions[speech.text].Invoke();
        }
    }

    // Method to load the game scene via button click or voice command
    public void QuitGame()
    {
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Only execute inside editor
    #endif
    }

    private void OnDestroy()
    {
        // Stop and dispose of the keyword recognizer to release resources
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}