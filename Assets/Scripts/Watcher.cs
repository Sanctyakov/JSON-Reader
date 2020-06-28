using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Watcher : MonoBehaviour //A class that contains variables and methods related to the FileSystemWatcher.
{
    private FileSystemWatcher watcher; //Whenever the json is edited, the table will show any changes accordingly.
   
    void Start ()
    {
        watcher = new FileSystemWatcher (Application.streamingAssetsPath, "*.json");

        watcher.Renamed += new RenamedEventHandler (OnRenamed);
        watcher.Changed += new FileSystemEventHandler (OnChanged);
        watcher.Created += new FileSystemEventHandler (OnChanged);
        watcher.Deleted += new FileSystemEventHandler (OnChanged);
        watcher.Error += new ErrorEventHandler(OnError);
       
        watcher.EnableRaisingEvents = true;
    }
   
    private void OnChanged (object source, FileSystemEventArgs e)
    {
        ReloadScene();
    }
   
    private void OnRenamed (object source, RenamedEventArgs e)
    {
        ReloadScene();
    }
   
    private void OnError(object source, ErrorEventArgs e)
    {
        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}