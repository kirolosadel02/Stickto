using System.Reflection;

namespace Stickto.Shared.Infrastructure
{
    /// <summary>
    /// Provides a reference to the assembly in which this class is defined.
    /// </summary>
    public static class AssemblyReference
    {
        /// <summary>
        /// Gets the assembly in which this class is defined.
        /// </summary>
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
