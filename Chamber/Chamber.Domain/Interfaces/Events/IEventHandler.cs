using Chamber.Domain.Events;

namespace Chamber.Domain.Interfaces.Events
{
    public interface IEventHandler
    {
        void RegisterHandlers(EventManager theEventManager);
    }
}
