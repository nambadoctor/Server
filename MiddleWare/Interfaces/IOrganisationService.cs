using DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IOrganisationService
    {
        /// <summary>
        /// Get organisation based on Id
        /// </summary>
        /// <returns></returns>
        public Task<Organisation> GetOrganisationAsync(string OrganisationId);
    }
}
