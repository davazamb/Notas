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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Notas.Clases;

namespace Notas.Controllers.API
{
    [RoutePrefix("API/Users")]
    public class UsersController : ApiController
    {
        private NotasContext db = new NotasContext();

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Longin(JObject form)
        {
            var email = string.Empty;
            var password = string.Empty;
            dynamic jsonObject = form;

            try
            {
                email = jsonObject.Email.Value;
                password = jsonObject.Password.Value;
            }
            catch
            {
                return this.BadRequest("Incorrect call");
            }

            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.Find(email, password);

            if (userASP == null)
            {
                return this.BadRequest("Usuario y Password incorrecto");
            }

            var user = db.Users
                .Where(u => u.UserName == email)
                .FirstOrDefault();

            if (user == null)
            {
                return this.BadRequest("Usuario y Password incorrecto");
            }

            return this.Ok(user);
        }



        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        //Cambiar el Password API
        [HttpPut]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(JObject form)
        {
            var userName = string.Empty;
            var oldPassword = string.Empty;
            var newPassword = string.Empty;
            dynamic jsonObject = form;

            try
            {
                userName = jsonObject.UserName.Value;
                oldPassword = jsonObject.OldPassword.Value;
                newPassword = jsonObject.NewPassword.Value;
            }
            catch
            {
                return this.BadRequest("Incorrect call");
            }
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.Find(userName, oldPassword);

            if (userASP == null)
            {
                return this.BadRequest("Usuario y Password incorrecto");
            }

            var response = userManager.ChangePassword(userASP.Id, oldPassword, newPassword);

            if (response.Succeeded)
            {
                return this.Ok<object>(new
                {
                    Message = "El password ha sido cambiado exitosamente"
                });             
            }
            else
            {
                return this.BadRequest(response.Errors.ToString());
            }
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }
            var db2 = new NotasContext();
            var oldUser = db2.Users.Find(id);
            db2.Dispose();

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                if (oldUser != null && oldUser.UserName != user.UserName)
                {
                    Utilities.ChangeEmailUserASP(oldUser.UserName, user.UserName);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Ok(user);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(UserPassword userPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User
            {
                Address = userPassword.Address,
                FirstName = userPassword.FirstName,
                IsStudent = true,
                IsTeacher = false,
                LastName = userPassword.LastName,
                Phone = userPassword.Phone,
                UserName = userPassword.UserName,
            };

            try
            {               
                db.Users.Add(user);
                db.SaveChanges();
                Utilities.CreateUserASP(userPassword.UserName, "Student", userPassword.Password);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            userPassword.UserId = user.UserId;
            userPassword.IsStudent = true;
            userPassword.IsTeacher = false;
            return this.Ok(userPassword);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}