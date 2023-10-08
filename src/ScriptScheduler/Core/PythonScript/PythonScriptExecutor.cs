using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;
using ScriptScheduler.Domain.Enums;
using ScriptScheduler.Domain.IO;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptExecutor : IScriptExecutor
{
    private readonly Serilog.ILogger _logger;
    private readonly IOptionsMonitor<PythonScriptOption> _optionsMonitor;
    private PythonScriptOption _scriptOption;
    
    public PythonScriptExecutor(Serilog.ILogger logger
    , IOptionsMonitor<PythonScriptOption> optionsMonitor)
    {
        _logger = logger;
        _scriptOption = optionsMonitor.CurrentValue;
        _optionsMonitor = optionsMonitor;
        _optionsMonitor.OnChange(OptionChange);
    }

    private void OptionChange(PythonScriptOption obj)
    {
        _scriptOption = obj;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = new ())
    {
        var dirs = Directory.GetDirectories(_scriptOption.ScriptPath).Where(m => m.Contains("python-script"));
        await Parallel.ForEachAsync(dirs, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 4,
            TaskScheduler = TaskScheduler.Default
        }, async (dir, token) =>
        {
            var files = Directory.GetFiles(dir, "*.py", SearchOption.AllDirectories);
            foreach (var file in files.Where(m => !m.Contains(".done")))
            {
                await ExecuteCoreAsync(file, cancellationToken);
            }
        });
    }

    

    private readonly Dictionary<ENUM_REPEAT_TYPE, Func<ScriptFileInfo, CancellationToken, Task<Tuple<string, string>>>> _states 
        = new()
    {
        {
            ENUM_REPEAT_TYPE.DAILY, async (fileInfo, cancellationToken) =>
            {
                var stdOutBuffer = new StringBuilder();
                var stdErrBuffer = new StringBuilder();

                if (fileInfo.IsExecuted) return new Tuple<string, string>(string.Empty, string.Empty);
                
                if (fileInfo.RepeatType == ENUM_REPEAT_TYPE.DAILY)
                {
                    if (fileInfo.ExecuteTime >= DateTime.Now.TimeOfDay)
                    {
                        await Cli.Wrap("python")
                            .WithArguments(fileInfo.FullPath)
                            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                            .ExecuteAsync(cancellationToken);

                        File.Move(fileInfo.FullPath, $"{fileInfo.FullPath}.done");
                        File.Delete(fileInfo.FullPath);
                    }
                }

                return new(stdOutBuffer.ToString(), stdErrBuffer.ToString());
            }
        },
        {
            ENUM_REPEAT_TYPE.CONTINUE, async (fileInfo, cancellationToken) =>
            {
                var stdOutBuffer = new StringBuilder();
                var stdErrBuffer = new StringBuilder();

                await Cli.Wrap("python")
                    .WithArguments(fileInfo.FullPath)
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                    .ExecuteAsync(cancellationToken);
                
                return new(stdOutBuffer.ToString(), stdErrBuffer.ToString());
            }
        },
        {
            ENUM_REPEAT_TYPE.ONCE, async (fileInfo, cancellationToken) =>
            {
                var stdOutBuffer = new StringBuilder();
                var stdErrBuffer = new StringBuilder();

                await Cli.Wrap("python")
                    .WithArguments(fileInfo.FullPath)
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                    .ExecuteAsync(cancellationToken);
                
                File.Delete(fileInfo.FullPath);
                
                return new(stdOutBuffer.ToString(), stdErrBuffer.ToString());
            }
        }
    };

    private async ValueTask ExecuteCoreAsync(string file, CancellationToken cancellationToken)
    {
        if(cancellationToken.IsCancellationRequested)
            _logger.Information("{File} canceled", file);
        
        _logger.Information("{File} executing", file);
        
        try
        {
            var fileInfo = ScriptFileHandler.Create().GetFileInfo(file);
            var result = await _states[fileInfo.RepeatType].Invoke(fileInfo, cancellationToken);
            _logger.Information("StdOut: {StdOut}", result.Item1);
            _logger.Information("StdErr: {StdErr}", result.Item2);            
        }
        catch (Exception e)
        {
            _logger.Error(e, "{File} Error: {Error}", file, e.Message);
            //if use elk, notification error state and shutdown.
            //if not use logging platform, direct notification to user.
#if DEBUG
            Environment.Exit(-1);
#endif
        }
        _logger.Information("{File} executed", file);
    }
}