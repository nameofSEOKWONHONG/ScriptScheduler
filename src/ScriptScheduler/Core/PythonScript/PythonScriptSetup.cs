using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptSetup : ScriptSetupBase<PythonScriptOption>
{
    public PythonScriptSetup(Serilog.ILogger logger, IOptionsMonitor<PythonScriptOption> optionsMonitor)
        : base(logger, optionsMonitor)
    {
        
    }
    
    public override async Task InitializeAsync(CancellationToken cancellationToken = new())
    {
        var result = await CliWrap.Cli.Wrap("python")
            .WithArguments(new[] { "--version" })
            .ExecuteAsync(cancellationToken);

        if (result.ExitCode != 0)
        {
            throw new Exception("python not installed.");
        }
        
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();
        foreach (var item in this.Option.PipList)
        {
            await CliWrap.Cli.Wrap("python")
                .WithValidation(CommandResultValidation.None)
                .WithArguments(new []{"-m", "pip", "install", "--upgrade", item})
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync(cancellationToken);
        }
    }
}