using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FSDP.DATA.EF;
using Microsoft.AspNet.Identity;

namespace FSDP.Controllers
{
    public class OwnerVehiclesController : Controller
    {
        private FSDPEntities db = new FSDPEntities();

        // GET: OwnerVehicles
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            UserDetail currentUser = db.UserDetails.Where(ud => ud.UserID == userID).FirstOrDefault();

            var ownerVehicles = db.OwnerVehicles.Where(x => x.OwnerID == currentUser.UserID);
            return View(ownerVehicles.ToList());
        }

        // GET: OwnerVehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerVehicle ownerVehicle = db.OwnerVehicles.Find(id);
            if (ownerVehicle == null)
            {
                return HttpNotFound();
            }
            return View(ownerVehicle);
        }

        // GET: OwnerVehicles/Create
        public ActionResult Create()
        {
            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName");
            return View();
        }

        // POST: OwnerVehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OwnerVehicleID,Make,Model,OwnerID,CarPhoto,RequestedRepairs,IsActive,DateAdded")] OwnerVehicle ownerVehicle)
        {
            if (ModelState.IsValid)
            {
                db.OwnerVehicles.Add(ownerVehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName", ownerVehicle.OwnerID);
            return View(ownerVehicle);
        }

        // GET: OwnerVehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerVehicle ownerVehicle = db.OwnerVehicles.Find(id);
            if (ownerVehicle == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName", ownerVehicle.OwnerID);
            return View(ownerVehicle);
        }

        // POST: OwnerVehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OwnerVehicleID,Make,Model,OwnerID,CarPhoto,RequestedRepairs,IsActive,DateAdded")] OwnerVehicle ownerVehicle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ownerVehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName", ownerVehicle.OwnerID);
            return View(ownerVehicle);
        }

        // GET: OwnerVehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerVehicle ownerVehicle = db.OwnerVehicles.Find(id);
            if (ownerVehicle == null)
            {
                return HttpNotFound();
            }
            return View(ownerVehicle);
        }

        // POST: OwnerVehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OwnerVehicle ownerVehicle = db.OwnerVehicles.Find(id);
            db.OwnerVehicles.Remove(ownerVehicle);
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
