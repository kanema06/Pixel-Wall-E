using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE
{
    public class RuntimeError : Exception
    {
        public RuntimeError(string message) : base(message) { }
    }
    public class ExecutionResult
{
    public List<string> Output { get; } = new List<string>();
    public List<string> Errors { get; } = new List<string>();
    
    public void AddOutput(string message) => Output.Add(message);
    public void AddError(string error) => Errors.Add(error);
}
}