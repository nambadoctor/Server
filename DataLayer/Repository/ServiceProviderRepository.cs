using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class ServiceProviderRepository : BaseRepository<ServiceProvider>, IServiceProviderRepository
    {
        public ServiceProviderRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<ServiceProvider> GetServiceProviderFromPhoneNumber(string phoneNumber)
        {
            var spFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);

            var serviceProvider = await this.GetSingleByFilter(spFilter);

            return serviceProvider;
        }


        public async Task<List<ServiceProviderProfile>> GetServiceProviderProfilesOfOrg(string organisationId)
        {
            var organisationFilter = Builders<ServiceProvider>.Filter.ElemMatch(
                sp => sp.Profiles,
                profile => profile.OrganisationId == organisationId
            );
            
            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Profiles.Where(profile => profile.OrganisationId == organisationId)
            );

            var result = await this.GetListByFilterAndProject(organisationFilter, project);

            return result.ToList();
        }

        public async Task<ServiceProviderProfile> GetServiceProviderProfile(string serviceProviderId, string organisationId)
        {
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Profiles.Where(profile => profile.OrganisationId == organisationId)
                );

            var serviceProviderProfile = await this.GetSingleByFilterAndProject(spFilter, project);

            return serviceProviderProfile;
        }

        public async Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(string organisationId, List<string> serviceProviderIds)
        {
            var serviceProviderIdList = new List<ObjectId>();

            foreach (var serviceProviderId in serviceProviderIds)
            {
                serviceProviderIdList.Add(new ObjectId(serviceProviderId));
            }

            var organisationAppointmentFilter = Builders<ServiceProvider>.Filter.ElemMatch(
               sp => sp.Profiles,
               profile => profile.OrganisationId == organisationId
               );

            var serviceProviderFilter = Builders<ServiceProvider>.Filter.In(
                sp => sp.ServiceProviderId,
                serviceProviderIdList
                );

            FilterDefinition<ServiceProvider> combinedFilter;
            if (serviceProviderIds.Count == 0)
            {
                combinedFilter = organisationAppointmentFilter;
            }
            else
            {
                combinedFilter = organisationAppointmentFilter & serviceProviderFilter;
            }

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Profiles.Where(profile => profile.OrganisationId == organisationId)
                );

            var result = await this.GetListByFilterAndProject(combinedFilter, project);

            return result.ToList();
        }

        public async Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(List<string> serviceProviderIds)
        {
            var serviceProviderIdsList = new List<ObjectId>();

            foreach (var serviceProviderId in serviceProviderIds)
            {
                serviceProviderIdsList.Add(new ObjectId(serviceProviderId));
            }

            var filter = Builders<ServiceProvider>.Filter.In(
                sp => sp.ServiceProviderId,
                serviceProviderIdsList
                );

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Profiles.Where(_ => true)
                );

            var result = await this.GetListByFilterAndProject(filter, project);

            return result.ToList();
        }
    }
}