using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class VitalsRepository : BaseRepository<ServiceRequest>, IVitalsRepository
    {
        public VitalsRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<Vitals> GetServiceRequestVitals(string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var project = Builders<ServiceRequest>.Projection.Expression(
                sr => sr.Vitals
                );

            var result = await this.GetSingleNestedByFilterAndProject(filter, project);

            return result;
        }

        public async Task UpdateVitals(Vitals vitals, string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.Set(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            update = update.Set(sr => sr.Vitals.BloodPressure, vitals.BloodPressure);

            update = update.Set(sr => sr.Vitals.BloodSugar, vitals.BloodSugar);

            update = update.Set(sr => sr.Vitals.RespiratoryRate, vitals.RespiratoryRate);

            update = update.Set(sr => sr.Vitals.BadHabit, vitals.BadHabit);

            update = update.Set(sr => sr.Vitals.VitalsId, vitals.VitalsId);

            update = update.Set(sr => sr.Vitals.Height, vitals.Height);

            update = update.Set(sr => sr.Vitals.Weight, vitals.Weight);

            update = update.Set(sr => sr.Vitals.Pulse, vitals.Pulse);

            update = update.Set(sr => sr.Vitals.Saturation, vitals.Saturation);

            update = update.Set(sr => sr.Vitals.Temperature, vitals.Temperature);

            await this.Upsert(filter, update);
        }
    }
}
