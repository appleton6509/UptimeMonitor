using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IProtocol
    {
        TaskResult Execute();
    }
}