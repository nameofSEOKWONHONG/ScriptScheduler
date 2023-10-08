using System;
using ScriptScheduler.Domain.Enums;

namespace ScriptScheduler.Domain.IO;

public class ScriptFileInfo
{
    public ENUM_REPEAT_TYPE RepeatType { get; set; }
    public TimeSpan ExecuteTime { get; set; }
    public string RoleName { get; set; }
    public string Extension { get; set; }
    public string FullPath { get; set; }
    public bool IsExecuted { get; set; }
}