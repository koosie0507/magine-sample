using System.Reflection;

using Magine.ProgramInformationSample.Core.Model;

namespace Magine.ProgramInformationSample.Core.ViewModel
{
    public class AiringViewModel
    {
        public AiringViewModel(Airing model)
        {
            ImageUri = model.ImageUrl;
            Title = model.Title;
            Schedule = string.Format("{0:D}, {0:t} - {1:t}", model.Start, model.Stop);
        }

        public string Title { get; private set; }

        public string ImageUri { get; private set; }

        public string Schedule { get; private set; }
    }
}