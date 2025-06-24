using AutoMapper;
using ITS.BLL.DTOs;
using ITS.BLL.Mappings;
using ITS.DAL;
using ITS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.BLL.Services
{
    public class IssueService
    {
        private static Mapper mapper = AutoMapperConfig.GetMapper();

        public static bool CreateIssue(IssueDTO issueDto)
        {
            using (var db = new ITSContext())
            {
                // Check if referenced User and Status exist
                var userExists = db.Users.Any(u => u.Id == issueDto.CreatedByUserId);
                var statusExists = db.Statuses.Any(s => s.Id == issueDto.StatusId);
                if (!userExists || !statusExists) return false;

                var issue = mapper.Map<Issue>(issueDto);

                if (issue.CreatedAt == default)
                    issue.CreatedAt = DateTime.Now;

                db.Issues.Add(issue);
                return db.SaveChanges() > 0;
            }
        }

        public static List<IssueDTO> GetAllIssues()
        {
            using (var db = new ITSContext())
            {
                var issues = db.Issues.ToList();
                return mapper.Map<List<IssueDTO>>(issues);
            }
        }

        public static IssueDTO GetIssueById(int id)
        {
            using (var db = new ITSContext())
            {
                var issue = db.Issues.Find(id);
                return issue == null ? null : mapper.Map<IssueDTO>(issue);
            }
        }

        public static bool UpdateIssue(IssueDTO issueDto)
        {
            using (var db = new ITSContext())
            {
                var existing = db.Issues.Find(issueDto.Id);
                if (existing == null) return false;

                // Detect status change
                bool statusChanged = existing.StatusId != issueDto.StatusId;

                // Update fields
                existing.Title = issueDto.Title;
                existing.Description = issueDto.Description;
                existing.StatusId = issueDto.StatusId;
                existing.Type = issueDto.Type;

                var result = db.SaveChanges() > 0;

                if (result && statusChanged)
                {
                    var user = db.Users.Find(existing.UserId);
                    var status = db.Statuses.Find(existing.StatusId);

                    if (user != null && !string.IsNullOrEmpty(user.Email) && status != null)
                    {
                        string subject = $"Issue '{existing.Title}' Status Updated";
                        string body = $"Hello {user.Name},<br/><br/>" +
                            $"The status of your issue titled '<b>{existing.Title}</b>' has been updated to '<b>{status.Name}</b>'.<br/><br/>" +
                            $"Thank you,<br/>Issue Tracking System";

                        // Send email (can wrap in try-catch in real app)
                        EmailService.SendEmail(user.Email, subject, body);
                    }
                }

                return result;
            }
        }


        public static bool DeleteIssue(int id)
        {
            using (var db = new ITSContext())
            {
                var existing = db.Issues.Find(id);
                if (existing == null) return false;

                db.Issues.Remove(existing);
                return db.SaveChanges() > 0;
            }
        }


        public static List<IssueDTO> SearchIssues(IssueFilterDTO filter)
        {
            using (var db = new ITSContext())
            {
                var query = db.Issues.AsQueryable();

                if (!string.IsNullOrEmpty(filter.TitleKeyword))
                {
                    query = query.Where(i => i.Title.Contains(filter.TitleKeyword));
                }

                if (filter.StatusId.HasValue)
                {
                    query = query.Where(i => i.StatusId == filter.StatusId.Value);
                }

                if (filter.CreatedByUserId.HasValue)
                {
                    query = query.Where(i => i.UserId == filter.CreatedByUserId.Value);
                }

                if (!string.IsNullOrEmpty(filter.Type))
                {
                    query = query.Where(i => i.Type == filter.Type);
                }

                if (filter.CreatedAfter.HasValue)
                {
                    query = query.Where(i => i.CreatedAt >= filter.CreatedAfter.Value);
                }

                if (filter.CreatedBefore.HasValue)
                {
                    query = query.Where(i => i.CreatedAt <= filter.CreatedBefore.Value);
                }

                var issues = query.ToList();

                return AutoMapperConfig.GetMapper().Map<List<IssueDTO>>(issues);
            }
        }

    }
}
