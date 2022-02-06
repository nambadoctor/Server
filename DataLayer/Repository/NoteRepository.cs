using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class NoteRepository : BaseRepository<ServiceRequest>, INoteRepository
    {
        public NoteRepository(IMongoContext context) : base(context)
        {
        }

        public async Task AddNote(Note note, string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.AddToSet(sr => sr.Notes, note);

            await this.AddToSet(filter, update);
        }

        public async Task UpdateNote(Note note, string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter;

            var nestedFilter = Builders<ServiceRequest>.Filter.ElemMatch(sr => sr.Notes, notes => notes.NoteId.Equals(note.NoteId));

            var update = Builders<ServiceRequest>.Update.Set(sp => sp.ServiceRequestId, new ObjectId(serviceRequestId));

            update = update.Set("Notes.$.NoteText", note.NoteText);

            update = update.Set("Appointments.$.LastModifiedTime", note.LastModifiedTime);

            await this.Upsert(nestedFilter, update);
        }

        public async Task DeleteNote(string noteId)
        {
            var filter = Builders<ServiceRequest>.Filter.ElemMatch(
                sr => sr.Notes, note => note.NoteId.Equals(new ObjectId(noteId)));

            var update = Builders<ServiceRequest>.Update.PullFilter(
                sr => sr.Notes,
                note => note.NoteId == new ObjectId(noteId)
                );

            await this.RemoveFromSet(filter, update);
        }

        public async Task<List<ServiceRequest>> GetAllNotes(string organisationId, string customerId)
        {
            var organisationFilter = Builders<ServiceRequest>.Filter.Eq(sr => sr.OrganisationId, organisationId);

            var customerFilter = Builders<ServiceRequest>.Filter.Eq(sr => sr.CustomerId, customerId);

            var combinedFilter = organisationFilter & customerFilter;

            var projection = Builders<ServiceRequest>.Projection
                .Include(sr => sr.ServiceRequestId)
                .Include(sr => sr.Notes)
                .Include(sr => sr.AppointmentId);

            var result = await this.GetProjectedListByFilterAndProject(filter: combinedFilter, project: projection);

            return result.ToList();
        }

        public async Task<List<Note>> GetServiceRequestNotes(string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var project = Builders<ServiceRequest>.Projection.Expression(
                sr => sr.Notes.Where(_ => true)
                );

            var result = await this.GetListByFilterAndProject(filter, project);

            return result.ToList();
        }
    }
}
