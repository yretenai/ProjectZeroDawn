using System;
using ZeroDawn.Core;
using ZeroDawn.Managers;

namespace ZeroDawn.Abstract
{
    public interface IDecimaStructuredFile : IDisposable
    {
        /// <summary>
        ///     Base core file
        /// </summary>
        DecimaCoreFile Core { get; set; }

        // ReSharper disable once UnusedParameter.Global
        void Work(DecimaCoreFile data, DecimaManagerCollection managers);
    }
}
