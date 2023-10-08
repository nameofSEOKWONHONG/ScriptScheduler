using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ScriptScheduler.Core.Base;

public class ScriptBaseBackgroundService : BackgroundService
{
    public ScriptBaseBackgroundService()
    {
        
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
    
    protected void ChangeFileDoneToIng(string scriptPath, string typePath)
    {
        var now = DateTime.Now;
        //AM 00
        if (now.Hour is >= 0 and < 1) 
        {
            var dirs = Directory.GetDirectories(scriptPath).Where(m => m.Contains(typePath));
            foreach (var dir in dirs)
            {
                var files = Directory.GetFiles(dir, "*.done", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    File.Move(file, file.Replace(".done", string.Empty));
                }
            }
        }
    }
}