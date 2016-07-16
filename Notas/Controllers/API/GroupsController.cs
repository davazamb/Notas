using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Notas.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Notas.Controllers.API
{
    [RoutePrefix("API/Groups")]
    public class GroupsController : ApiController
    {
        private NotasContext db = new NotasContext();

        [HttpPost]
        [Route("SaveNotes")]
        public IHttpActionResult SaveNotes(JObject form)
        {
            var myStudentsResponse = JsonConvert.DeserializeObject<MyStudentsResponse>(form.ToString());            
            return Ok(true);
        }

        [HttpGet]
        [Route("GetStudents/{groupId}")]
        public IHttpActionResult GetStudents(int groupId)
        {
            var students = db.GroupDetails.Where(gd => gd.GroupId == groupId).ToList();
            var myStudents = new List<object>();
            foreach (var student in students)
            {
                var myStudent = db.Users.Find(student.UserId);
                myStudents.Add(new
                {
                    GroupDetailId = student.GroupDetailId,
                    GroupId = student.GroupId,
                    Student = myStudent,
                });
            }
            return Ok(myStudents);
        }

        [HttpGet]
        [Route("GetGroups/{userId}")]
        public IHttpActionResult GetGroups(int userId)
        {
            var groups = db.Groups.Where(g => g.UserId == userId).ToList();
            var subjects = db.GroupDetails.Where(gd => gd.UserId == userId).ToList();
            var matters = new List<object>();
            foreach (var subject in subjects)
            {
                var teacher = db.Users.Find(subject.Group.UserId);
                matters.Add(new
                {
                    GroupId = subject.GroupId,
                    Description = subject.Group.Description,
                    teacher = teacher,
                });
            }

            var response = new
            {
                MyGroups = groups,
                MySubjects = matters,
            };
            return Ok(response);
        }

        
        // GET: api/Groups
        [HttpGet]
        public IQueryable<Group> GetGroups()
        {
            return db.Groups;
        }

        // GET: api/Groups/5
        [HttpGet]
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // PUT: api/Groups/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGroup(int id, Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.GroupId)
            {
                return BadRequest();
            }

            db.Entry(group).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Groups
        [HttpPost]
        [ResponseType(typeof(Group))]
        public IHttpActionResult PostGroup(Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(group);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = group.GroupId }, group);
        }

        // DELETE: api/Groups/5
        [HttpDelete]
        [ResponseType(typeof(Group))]
        public IHttpActionResult DeleteGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            db.SaveChanges();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}