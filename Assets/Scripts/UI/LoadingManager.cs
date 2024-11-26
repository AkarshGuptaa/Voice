using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SceneManager.LoadScene(sceneIndex);
    }
}