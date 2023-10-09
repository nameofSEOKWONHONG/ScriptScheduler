using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Options;
using ScriptScheduler.Domain.Enums;
using ScriptScheduler.Domain.IO;

namespace ScriptScheduler.Core.Base;

public abstract class ScriptExecutorBase<T> : IScriptExecutor
where T : ScriptOptionBase
{
    protected Serilog.ILogger Logger;
    protected T Option;
    protected IOptionsMonitor<T> OptionsMonitor;

    protected ScriptExecutorBase(Serilog.ILogger logger, IOptionsMonitor<T> optionsMonitor)
    {
        this.Logger = logger;
        this.OptionsMonitor = optionsMonitor;
        this.OptionsMonitor.OnChange(ChangeOption);
        this.Option = this.OptionsMonitor.CurrentValue;
    }

    private void ChangeOption(T obj)
    {
        this.Option = obj;
    }
    
    public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var dirs =  Directory.GetDirectories(Option.ScriptPath)
            .Where(m => m.Contains(Option.ExecutorPathName))
            .ToArray();

        await Parallel.ForEachAsync(dirs, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = this.Option.MaxDegreeOfParallelism,
            TaskScheduler = TaskScheduler.Default
        }, async (dir, token) =>
        {
            var files = Directory.GetFiles(dir, $"*.{this.Option.FileExtension}", SearchOption.AllDirectories);
            foreach (var file in files.Where(m => !m.Contains(".done")))
            {
                await ExecuteCoreAsync(file, cancellationToken);
                
                var sFileInfo = ScriptFileHandler.Create().GetFileInfo(file);
                if (sFileInfo.RepeatType == ENUM_REPEAT_TYPE.ONCE)
                {
                    File.Delete(sFileInfo.FullPath);
                }
                else if (sFileInfo.RepeatType == ENUM_REPEAT_TYPE.DAILY)
                {
                    File.Move(sFileInfo.FullPath, $"{sFileInfo.FullPath}.done");
                }
            }
        });
    }

    protected abstract Task ExecuteCoreAsync(string file, CancellationToken cancellationToken);
}