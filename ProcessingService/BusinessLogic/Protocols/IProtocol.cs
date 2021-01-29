using ProcessingService.DTO;

namespace ProcessingService.BusinessLogic.Protocols
{
    public interface IProtocol
    {
        TaskResultDTO Execute();
    }
}