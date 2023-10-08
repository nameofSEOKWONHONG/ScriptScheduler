using System.Threading;
using System.Threading.Tasks;

namespace ScriptScheduler.Core.Base;

public interface IScriptExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}