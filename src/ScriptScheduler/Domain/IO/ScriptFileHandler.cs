using System;
using System.IO;
using ScriptScheduler.Domain.Enums;

namespace ScriptScheduler.Domain.IO;

public class ScriptFileHandler
{
    public ScriptFileInfo GetFileInfo(string script)
    {
        //[REPEAT_TYPE].[execute time].[rolename].[extension(py)]
        // ex) C.0.http-get.py
        // ex) D.1200.http-get.py
        var scriptFile = Path.GetFileName(script);
        var infos = scriptFile.Split(".");
        ScriptFileInfo fileInfo = new ScriptFileInfo()
        {
            RepeatType = infos[0] == "D" ? ENUM_REPEAT_TYPE.DAILY : ENUM_REPEAT_TYPE.CONTINUE,
            ExecuteTime = infos[0] == "D" ? TimeSpan.Parse($"{infos[1].Substring(0, 2)}:{infos[1].Substring(2, 2)}") : TimeSpan.Zero,
            RoleName = infos[2],
            Extension = infos[3],
            FullPath = script
        };
        return fileInfo;
    }

    public static ScriptFileHandler Create()
    {
        return new ScriptFileHandler();
    }
}