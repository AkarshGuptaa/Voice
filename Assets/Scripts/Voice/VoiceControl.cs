using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceControl : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();

    private PlayerMovement playerMovement;
    private Animator anim;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        // Define voice commands and their corresponding actions
        actions.Add("right", MoveRight);
        actions.Add("end", StopMoving);
        actions.Add("flip", Flip);
        actions.Add("lift", Jump);
        actions.Add("left", MoveLeft);

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

    private void MoveRight()
    {
        playerMovement.SetHorizontalInput(1f);
        anim.SetBool("run", true);
        Debug.Log("Running");
    }
    
    private void MoveLeft() // Method for moving left
    {
        playerMovement.SetHorizontalInput(-1f); // Negative value to move left
        anim.SetBool("run", true);
        Debug.Log("Running left");
    }

    private void StopMoving()
    {
        playerMovement.SetHorizontalInput(0f);
        anim.SetBool("run", false);
        Debug.Log("Stopping");
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Debug.Log("Flipping direction");
    }

    private void Jump()
    {
        if (playerMovement.isGrounded())
        {
            playerMovement.body.velocity = new Vector2(playerMovement.body.velocity.x, playerMovement.jumpPower);
            anim.SetTrigger("jump");
        }
        else if (playerMovement.onWall() && !playerMovement.isGrounded())
        {
            if (playerMovement.horizontalInput == 0)
            {
                playerMovement.body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                playerMovement.body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            playerMovement.wallJumpCooldown = 0;
        }
    }
}