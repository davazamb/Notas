using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Notas.Models;
using Notas.Clases;

namespace Notas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private NotasContext db = new NotasContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserView view)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(view.User);
                try
                {                    
                    if (view.Photo != null)
                    {
                       var pic = Utilities.UploadPhoto(view.Photo);
                        if(!string.IsNullOrEmpty(pic))
                        {
                            view.User.Photo = string.Format("~/Content/Photos/{0}", pic);
                        }

                    }
                    db.SaveChanges();

                    Utilities.CreateUserASP(view.User.UserName);

                    if(view.User.IsStudent)
                    {
                        Utilities.AddRoleToUser(view.User.UserName, "Student");
                    }                    
                    if (view.User.IsTeacher)
                    {
                        Utilities.AddRoleToUser(view.User.UserName, "Teacher");
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(view);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var view = new UserView { User = user };
            return View(view);
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserView view)
        {
            if (ModelState.IsValid)
            {
                var db2 = new NotasContext();
                var oldUser = db2.Users.Find(view.User.UserId);
                db2.Dispose();

                if (view.Photo != null)
                {
                    var pic = Utilities.UploadPhoto(view.Photo);
                    if (!string.IsNullOrEmpty(pic))
                    {
                        view.User.Photo = string.Format("~/Content/Photos/{0}", pic);
                    }

                }else
                {
                    view.User.Photo = oldUser.Photo;
                }
                
                db.Entry(view.User).State = EntityState.Modified;
                
                try
                {
                    db.SaveChanges();
                    if(oldUser!= null && oldUser.UserName != view.User.UserName)
                    {
                        Utilities.ChangeEmailUserASP(oldUser.UserName, view.User.UserName);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(view);
                }
                return RedirectToAction("Index");
            }
            return View(view);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
