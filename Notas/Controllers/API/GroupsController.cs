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

        //PAra Mostrar en la vista de notas en el profesor
        [HttpGet]
        [Route("GetNotesGroup/{groupId}")]
        public IHttpActionResult GetNotesGroup(int groupId)
        {
            var group = db.Groups.Find(groupId);

            if(group == null)
            {
                return BadRequest("El grupo no existe");
            }
            var students = new List<object>();

            foreach (var groupDetails in group.GroupDetails)
            {
                var notes = db.GroupDetails.Where(gd => gd.GroupId == groupId && gd.UserId == groupDetails.UserId).ToList();
                var noteDef = 0.0;

                foreach (var note in notes)
                {
                    foreach (var notes2 in note.Notes)
                    {
                        noteDef += notes2.Percentage * notes2.Qualification;

                    }
                }
                students.Add(new
                {
                    Student = groupDetails.User,
                    Note = noteDef,
                });
            }

            return Ok(students);
        }

        //Detalle de las notas de un estudiante de una materia
        [HttpGet]
        [Route("GetNoteDetail/{groupId}/{userId}")]
        public IHttpActionResult GetNoteDetail(int groupId, int userId)
        {
            //var notes = db.GroupDetails.Where(gd => gd.GroupId == groupId && gd.UserId == userId).ToList();
            //var myNotes = new List<object>();

            //foreach (var note in notes)
            //{
            //    var myNote = db.Notes.Find(note.UserId);
            //    myNotes.Add(new
            //    {
            //        GroupDetailId = note.GroupDetailId,
            //        Percentage = myNote,
            //        Qualification = myNote,
            //    });
            //}
            //return Ok(myNotes);
            var group = db.Groups.Find(groupId);

            if (group == null)
            {
                return BadRequest("El grupo no existe");
            }
            var myNotes = new List<object>();

            foreach (var groupDetails in group.GroupDetails)
            {
                var notes = db.GroupDetails.Where(gd => gd.GroupId == groupId && gd.UserId == groupDetails.UserId).ToList();
                var noteDef = 0.0;

                foreach (var note in notes)
                {
                    foreach (var notes2 in note.Notes)
                    {
                        noteDef += notes2.Percentage * notes2.Qualification;

                    }
                }
                myNotes.Add(new
                {                   
                    myNote = groupDetails.Notes,
                    Note = noteDef,
                });
            }

            return Ok(myNotes);
        }



        [HttpGet]
        [Route("GetNote/{groupId}/{userId}")]
        public IHttpActionResult GetNote(int groupId, int userId)
        {
            var noteDef = 0.0;
            var notes = db.GroupDetails.Where(gd => gd.GroupId == groupId && gd.UserId == userId).ToList();

            foreach (var note in notes)
            {
                foreach (var notes2 in note.Notes)
                {
                    noteDef += notes2.Percentage * notes2.Qualification;
                }
            }

            return this.Ok<object>(new
            {
                Note = noteDef,
            });
        }

        [HttpPost]
        [Route("SaveNotes")]
        public IHttpActionResult SaveNotes(JObject form)
        {
            var myStudentsResponse = JsonConvert.DeserializeObject<MyStudentsResponse>(form.ToString());          
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var student in myStudentsResponse.Students)
                    {
                        var note = new Note
                        {
                            GroupDetailId = student.GroupDetailId,
                            Percentage = (float)myStudentsResponse.Percentage,
                            Qualification = (float)student.Note,
                        };
                        db.Notes.Add(note);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return Ok(true);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }  
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