using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Magine.ProgramInformationSample.Core.Model;

namespace Magine.ProgramInformationSample.Core
{
    public interface IMagineApi
    {
        Task Login(string userName, string password);

        Task<IEnumerable<Airing>> GetAirings(DateTime from, DateTime to);
    }
}