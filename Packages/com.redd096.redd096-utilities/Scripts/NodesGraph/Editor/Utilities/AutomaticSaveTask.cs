using System.Threading.Tasks;

/// <summary>
/// Use this to create an awaitable task for autosave
/// </summary>
public class AutomaticSaveTask
{
    private bool isCanceled;

    int timer;
    string fileName;
    string filePathInProject;
    System.Action<string, string> func;

    /// <summary>
    /// Start automatic save
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="fileName"></param>
    /// <param name="filePathInProject"></param>
    /// <param name="func"></param>
    public AutomaticSaveTask(int timer, string fileName, string filePathInProject, System.Action<string, string> func) 
    {
        this.timer = timer;
        this.fileName = fileName;
        this.filePathInProject = filePathInProject;
        this.func = func;

        Start();
    }

    /// <summary>
    /// Stop automatic save
    /// </summary>
    public void Stop()
    {
        isCanceled = true;
    }

    private async void Start()
    {
        //wait
        await Task.Delay(timer * 1000);

        //be sure this task isn't canceled
        if (isCanceled == false)
        {
            func?.Invoke(fileName, filePathInProject);
        }
    }
}
