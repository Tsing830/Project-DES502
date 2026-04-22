using UnityEngine;
using UnityEngine.SceneManagement;

// Level Manager
// Charlotte Bigham
// 28/11/2024

public class LevelManager : MonoBehaviour

{    // Creates a string input to allow the name of the desired scene to be entered
    public string sceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene()
    {   // Loads scene input into sceneName
        SceneManager.LoadScene(sceneName);
    }
}
