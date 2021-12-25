using DataModel.Client.Provider;

namespace NambaMiddleWare.Interfaces
{
    public interface IOrganisationService
    {
        public Task<List<Organisation>> GetOrganisationsAsync();
    }
}
