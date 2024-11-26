using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        // Define voice commands and actions
        actions.Add("pause", TogglePause);

        // Initialize the KeywordRecognizer
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void Update()
    {
        // Pause/Unpause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
            {
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }

    #region GAME OVER
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0); // Because main menu is at index 0
    }
    
    public void Quit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Only execute inside editor
        #endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        // Pause or unpause the game
        pauseScreen.SetActive(status);

        if (status)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void TogglePause()
    {
        if (pauseScreen.activeInHierarchy)
        {
            PauseGame(false);
        }
        else
        {
            PauseGame(true);
        }
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion

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
