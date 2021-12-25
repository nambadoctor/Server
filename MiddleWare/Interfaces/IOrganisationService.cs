using DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IOrganisationService
    {
        public Task<List<Organisation>> GetOrganisationsAsync();
    }
}
