using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface INoteRepository : IRepository<ServiceRequest>
    {
        public Task<List<Note>> GetAllNotes(string organisationId, string customerId);
        public Task<List<Note>> GetServiceRequestNotes(string serviceRequestId);
        public Task AddNote(Note note, string serviceRequestId);
        public Task DeleteNote(string noteId);
    }
}
