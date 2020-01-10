using System.Threading.Tasks;

namespace MindMatrix.EventSourcing.Tests
{
    public interface IUserService
    {
        Task<IAggregate<User>> Create(CreateUser user);
    }


}