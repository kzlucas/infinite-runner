using System;
using System.Threading.Tasks;

public interface IInitializable
{
    int Priority { get; }
    Type[] Dependencies { get; }
    Task InitializeAsync();
}