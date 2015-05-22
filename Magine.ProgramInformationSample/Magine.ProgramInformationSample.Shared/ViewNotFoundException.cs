using System;

namespace Magine.ProgramInformationSample
{
    public class ViewNotFoundException : Exception
    {
        public ViewNotFoundException(Exception exception)
            : base("View not found for specified viewmodel type.", exception)
        {
            throw new NotImplementedException();
        }
    }
}