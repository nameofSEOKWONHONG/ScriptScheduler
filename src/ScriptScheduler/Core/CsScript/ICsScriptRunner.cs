using System.Threading.Tasks;
using eXtensionSharp;

namespace ScriptScheduler.Core.CsScript;

public interface ICsScriptRunner
{
    Task OnProducerAsync();
    Task OnConsumerAsync();
}

public interface ICsScriptRunner<TRequest, TResult> : ICsScriptRunner
{
    TRequest Request { get; set; }
    TResult Result { get; set; }
}