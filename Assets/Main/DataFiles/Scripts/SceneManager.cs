using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SceneManager")]
public class SceneManager : ScriptableObject
{
    private Stack<int> loadedLevels;

    [System.NonSerialized]
    private bool initialized;

    private void Init()
    {
        loadedLevels = new Stack<int>();
        initialized = true;
    }

    public UnityEngine.SceneManagement.Scene GetActiveScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadScene(int buildIndex)
    {
        if (!initialized) Init();
        loadedLevels.Push(GetActiveScene().buildIndex);
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void LoadScene(string sceneName)
    {
        if (!initialized) Init();
        loadedLevels.Push(GetActiveScene().buildIndex);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (!initialized)
        {
            Debug.LogError("You haven't used the LoadScene functions of the scriptable object. Use them instead of the LoadScene functions of Unity's SceneManager.");
        }
        if (loadedLevels.Count > 0)
        {

            
            UnityEngine.SceneManagement.SceneManager.LoadScene(loadedLevels.Pop());
        }
        else
        {
            Debug.LogError("No previous scene loaded");
            // If you want, you can call `Application.Quit()` instead
        }
    }
}