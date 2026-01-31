using System;
using System.Threading.Tasks;

public interface IInitializable
{
    int initPriority { get; }
    Type[] initDependencies { get; }
    Task InitializeAsync();
}