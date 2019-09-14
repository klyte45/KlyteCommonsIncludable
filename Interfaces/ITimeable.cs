using System;

namespace Klyte.Commons.Interfaces
{
    public interface ITimeable
    {
        TimeSpan TimeOfDay { get; set; }
    }
}