using System.Collections.Generic;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptOption : ScriptOptionBase
{
    public IList<string> PipList { get; set; }
}