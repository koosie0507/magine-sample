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
        }

        public string Title { get; private set; }

        public string ImageUri { get; private set; }
    }
}