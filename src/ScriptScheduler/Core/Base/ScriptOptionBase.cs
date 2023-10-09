namespace ScriptScheduler.Core.Base;

public class ScriptOptionBase
{
    public string ScriptPath { get; set; }
    public string ExecutorPathName { get; set; }
    public int MaxDegreeOfParallelism { get; set; }
    public string FileExtension { get; set; }
    public int Interval { get; set; }
}