using Logic.DTO;

namespace Logic.QueueSender
{
    public interface IQueueSender
    {
        void SendMessage(MessageDto message);
    }
}
