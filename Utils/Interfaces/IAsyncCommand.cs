using System.Threading.Tasks;
using System.Windows.Input;

namespace Utils.Interfaces
{
    /// <summary>
    /// Provides an asynchronous command behavior.
    /// </summary>
    internal interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Command execution.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task ExecuteAsync(object parameter);
    }
}
