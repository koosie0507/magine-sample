using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

using Magine.ProgramInformationSample.Core.Model;

namespace Magine.ProgramInformationSample.Core.ViewModel
{
    public class ProgramInformationViewModel
    {
        private readonly ObservableCollection<AiringViewModel> airings = new ObservableCollection<AiringViewModel>();

        private readonly IMagineApi client;

        private readonly IRouter router;

        public ProgramInformationViewModel(IMagineApi client, IRouter router)
        {
            this.client = client;
            this.router = router;
        }

        public IEnumerable<AiringViewModel> Airings
        {
            get
            {
                return airings;
            }
        }

        public async Task LoadAiringsAsync()
        {
            airings.Clear();
            DateTime from = DateTime.Today;
            try
            {
                IEnumerable<Airing> models = await client.GetAirings(from, from.AddDays(1));
                foreach (Airing model in models)
                {
                    airings.Add(new AiringViewModel(model));
                }
            }
            catch (HttpRequestException)
            {
                router.GoTo<LoginViewModel>();
            }
        }
    }
}