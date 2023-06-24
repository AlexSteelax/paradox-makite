using Microsoft.CodeAnalysis;

namespace MakItE.Core.Processors
{
    public class Diagnostic
    {
        public readonly string Description;
        public readonly DiagnosticSeverity Severity;

        Diagnostic(string description, DiagnosticSeverity severity)
        {
            Description = description;
            Severity = severity;
        }

        public static Diagnostic Create(string description, DiagnosticSeverity severity = DiagnosticSeverity.Info) => new(description, severity);
    }
}