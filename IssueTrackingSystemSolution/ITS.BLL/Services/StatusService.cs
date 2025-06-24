using AutoMapper;
using ITS.BLL.DTOs;
using ITS.BLL.Mappings;
using ITS.DAL;
using ITS.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace ITS.BLL.Services
{
    public class StatusService
    {
        private static Mapper mapper = AutoMapperConfig.GetMapper();

        public static List<StatusDTO> GetAllStatuses()
        {
            using (var db = new ITSContext())
            {
                var statuses = db.Statuses.ToList();
                return mapper.Map<List<StatusDTO>>(statuses);
            }
        }

        public static StatusDTO GetStatusById(int id)
        {
            using (var db = new ITSContext())
            {
                var status = db.Statuses.Find(id);
                return status == null ? null : mapper.Map<StatusDTO>(status);
            }
        }

        public static bool CreateStatus(StatusDTO statusDto)
        {
            using (var db = new ITSContext())
            {
                var status = mapper.Map<Status>(statusDto);
                db.Statuses.Add(status);
                return db.SaveChanges() > 0;
            }
        }

        public static bool UpdateStatus(StatusDTO statusDto)
        {
            using (var db = new ITSContext())
            {
                var existing = db.Statuses.Find(statusDto.Id);
                if (existing == null) return false;

                existing.Name = statusDto.Name;
                return db.SaveChanges() > 0;
            }
        }

        public static bool DeleteStatus(int id)
        {
            using (var db = new ITSContext())
            {
                var existing = db.Statuses.Find(id);
                if (existing == null) return false;

                db.Statuses.Remove(existing);
                return db.SaveChanges() > 0;
            }
        }
    }
}
