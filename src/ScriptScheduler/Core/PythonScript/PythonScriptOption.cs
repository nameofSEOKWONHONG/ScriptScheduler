namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptOption
{
    public string ScriptPath { get; set; }
    public int Interval { get; set; }
    public IList<string> PipList { get; set; }
}