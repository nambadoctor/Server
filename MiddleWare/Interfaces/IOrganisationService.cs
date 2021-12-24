using ServerDataModels.Organisation;

namespace NambaMiddleWare.Interfaces
{
    public interface IOrganisationService
    {
        public Task<List<Organisation>> GetOrganisationsAsync();
    }
}
