using System;
using System.IO;
using ScriptScheduler.Domain.Enums;

namespace ScriptScheduler.Domain.IO;

public class ScriptFileHandler
{
    public ScriptFileInfo GetFileInfo(string scriptFilePath)
    {
        //[REPEAT_TYPE].[execute time].[rolename].[extension(py)].[executed flag]
        // ex) C.0.http-get.py
        // ex) D.1200.http-get.py
        // ex) D.1200.http-get.py.x
        var sFileName = Path.GetFileName(scriptFilePath);
        var infos = sFileName.Split(".");
        ScriptFileInfo fileInfo = new ScriptFileInfo()
        {
            RepeatType = infos[0] == "D" ? ENUM_REPEAT_TYPE.DAILY : ENUM_REPEAT_TYPE.CONTINUE,
            ExecuteTime = infos[0] == "D" ? TimeSpan.Parse($"{infos[1].Substring(0, 2)}:{infos[1].Substring(2, 2)}") : TimeSpan.Zero,
            RoleName = infos[2],
            Extension = infos[3],
            FullPath = scriptFilePath,
            IsExecuted = infos.Length >= 5
        };
        return fileInfo;
    }

    public static ScriptFileHandler Create()
    {
        return new ScriptFileHandler();
    }
}