using System.Diagnostics.CodeAnalysis;

namespace MakItE.Core.Processors
{
    public class ExecuteResult<TResult>
        where TResult : class
    {
        public readonly TResult? Result;
        public readonly Diagnostic? Diagnostic;
        public readonly bool Success;

        ExecuteResult(TResult? result, Diagnostic? diagnostic, bool success)
        {
            Result = result;
            Diagnostic = diagnostic;
            Success = success;
        }

        public static ExecuteResult<TResult> Create([NotNull] TResult result, [NotNull] Diagnostic diagnostic, bool success = true) => new(result, diagnostic, success);
        public static ExecuteResult<TResult> Create([NotNull] TResult result, bool success = true) => new(result, null, success);
        public static ExecuteResult<TResult> Create([NotNull] Diagnostic diagnostic) => new(null, diagnostic, false);
    }
}
