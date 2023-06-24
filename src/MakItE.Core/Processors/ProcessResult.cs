namespace MakItE.Core.Processors
{
    public class ProcessResult<TResult>
        where TResult: class
    {
        public readonly TResult? Result;

        public readonly bool Success;

        public readonly int Totals;
        public readonly int Errors;

        public readonly TimeSpan Elapsed;

        internal ProcessResult(TResult? result, bool success, int totals, int errors, TimeSpan elapsed)
        {
            Result = result;
            Success = success;
            Totals = totals;
            Errors = errors;
            Elapsed = elapsed;
        }
    }
}
