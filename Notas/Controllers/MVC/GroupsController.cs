using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Notas.Models;

namespace Notas.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private NotasContext db = new NotasContext();

        public ActionResult DeleteStudent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var groupDetail = db.GroupDetails.Find(id);

            if (groupDetail == null)
            {
                return HttpNotFound();
            }

            db.GroupDetails.Remove(groupDetail);
            db.SaveChanges();
            return RedirectToAction(string.Format("Details/{0}", groupDetail.GroupId));


        }

        [HttpPost]
        public ActionResult AddStudent(GroupDetail groupDetail)
        {
            if(ModelState.IsValid)
            {
                var exists = db.GroupDetails
                    .Where(gd => gd.GroupId == groupDetail.GroupId &&
                    gd.UserId == groupDetail.UserId).FirstOrDefault();

                if (exists == null)
                {
                    db.GroupDetails.Add(groupDetail);
                    db.SaveChanges();
                    return RedirectToAction(string.Format("Details/{0}", groupDetail.GroupId));
                }

                ModelState.AddModelError(string.Empty, "Estudiante ya matriculado en este grupo");
            }

            ViewBag.UserId = new SelectList(
               db.Users.Where(u => u.IsStudent).OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
               "UserId", "FullName", groupDetail.UserId);

            return View(groupDetail);
        }

        public ActionResult AddStudent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            var groupDetail = new GroupDetail
            {
                GroupId = id.Value,
            };

            ViewBag.UserId = new SelectList(
               db.Users.Where(u => u.IsStudent).OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
               "UserId", "FullName");

            return View(groupDetail);
        }

        // GET: Groups
        public ActionResult Index()
        {
            var groups = db.Groups.Include(g => g.User);
            return View(groups.ToList());
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(
                db.Users.Where(u => u.IsTeacher).OrderBy(u => u.FirstName).ThenBy(u=> u.LastName), 
                "UserId", "FullName");
            return View();
        }

        // POST: Groups/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Description,UserId")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(
                db.Users.Where(u => u.IsTeacher).OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "UserId", "FullName");
            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(
                db.Users.Where(u => u.IsTeacher).OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "UserId", "FullName", group.UserId);
            return View(group);
        }

        // POST: Groups/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Description,UserId")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(
                db.Users.Where(u => u.IsTeacher).OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "UserId", "FullName", group.UserId);
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
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
