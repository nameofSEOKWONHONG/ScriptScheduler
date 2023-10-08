using System.Text;
using CliWrap;
using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.PythonScript;

public class PythonSetup
{
    private PythonScriptOption _scriptOption;
    private readonly Serilog.ILogger _logger;
    public PythonSetup(Serilog.ILogger logger
        , IOptionsMonitor<PythonScriptOption> options)
    {
        _logger = logger;
        _scriptOption = options.CurrentValue;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = new())
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
        foreach (var item in _scriptOption.PipList)
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