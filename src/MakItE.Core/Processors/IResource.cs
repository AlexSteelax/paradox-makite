namespace MakItE.Core.Processors
{
    public interface IResource
    {
        IEnumerable<string> GetFiles();
        string Root { get; }
    }
}
