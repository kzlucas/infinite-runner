using System;
using System.Threading.Tasks;

public interface IInitializable
{
    int InitPriority { get; }
    Type[] InitDependencies { get; }
    Task InitializeAsync();
}