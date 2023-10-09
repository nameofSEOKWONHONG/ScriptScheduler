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

public class PythonScriptExecutor : ScriptExecutorBase<PythonScriptOption>
{   
    public PythonScriptExecutor(Serilog.ILogger logger, IOptionsMonitor<PythonScriptOption> optionsMonitor)
        : base(logger, optionsMonitor)
    {
    }

    protected override async Task ExecuteCoreAsync(string file, CancellationToken cancellationToken)
    {
        if(cancellationToken.IsCancellationRequested)
            Logger.Information("{File} canceled", file);
        
        Logger.Information("{File} executing", file);
        
        try
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();
            
            await Cli.Wrap("python")
                .WithArguments(file)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync(cancellationToken);
            
            Logger.Information("StdOut: {StdOut}", stdOutBuffer.ToString());
            Logger.Information("StdErr: {StdErr}", stdErrBuffer.ToString());
        }
        catch (Exception e)
        {
            Logger.Error(e, "{File} Error: {Error}", file, e.Message);
            //if use elk, notification error state and shutdown.
            //if not use logging platform, direct notification to user.
#if DEBUG
            Environment.Exit(-1);
#endif
        }
        Logger.Information("{File} executed", file);
    }
}