using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core
{
    public interface IMagineApi
    {
        Task Login(string userName, string password);
    }
}