using BusinessLogic.DAL;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class MeetingRoomService
    {
        protected readonly IGenericRepository<MeetingRoom> _genericRepository;

        public MeetingRoomService(IGenericRepository<MeetingRoom> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<MeetingRoom>> GetAll()
        {
            return await _genericRepository.GetAllAsync();
        }

        public async Task<MeetingRoom> GetById(int id)
        {
            return await _genericRepository.GetByIdAsync(id);
        }

        public async Task<MeetingRoom> GetByName(string name)
        {
            return await GetById(_genericRepository.Query().Where(x => x.Name == name)
                .Select(x => x.Id).FirstOrDefault());
        }

        public async Task<MeetingRoom> Add(MeetingRoom meetingRoom)
        {
            return await _genericRepository.AddAsync(meetingRoom);
        }

        public async Task<MeetingRoom> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }

        public async Task<MeetingRoom> Update(MeetingRoom meetingRoom)
        {
            return await _genericRepository.UpdateAsync(meetingRoom);
        }
    }
}
