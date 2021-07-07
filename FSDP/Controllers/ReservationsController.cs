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
    public class ReservationsController : Controller
    {
        private FSDPEntities db = new FSDPEntities();
        // GET: Reservations
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Index()
        {

            var reservations = db.Reservations.Include(r => r.Location).Include(r => r.OwnerVehicle);
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.VehicleNotes = new SelectList(db.OwnerVehicles, "OwnerVehicleID", "RequestedRepairs");
            return View(reservation);
        }

        //GET: AdminCreate
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult AdminCreate()
        {
            ViewBag.Owner = new SelectList(db.UserDetails, "UserID", "FullName");
            return View();
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Admin, Employee, Owner")]
        public ActionResult Create(string Owner)
        {
            if (User.IsInRole("Owner"))
            {
                Owner = User.Identity.GetUserId();
            }

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            if (User.IsInRole("Owner"))
            {
                ViewBag.OwnerVehicleID = new SelectList(db.OwnerVehicles.Where(o => o.OwnerID == Owner), "OwnerVehicleID", "MakeAndModel");
            }
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                ViewBag.OwnerVehicleID = new SelectList(db.OwnerVehicles.Where(o => o.OwnerID == Owner), "OwnerVehicleID", "Make");
            }
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Employee, Owner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationID,OwnerVehicleID,LocationID,ReservationDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", reservation.LocationID);
            ViewBag.OwnerVehicleID = new SelectList(db.OwnerVehicles, "OwnerVehicleID", "Make", reservation.OwnerVehicleID);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", reservation.LocationID);
            ViewBag.OwnerVehicleID = new SelectList(db.OwnerVehicles, "OwnerVehicleID", "Make", reservation.OwnerVehicleID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,OwnerVehicleID,LocationID,ReservationDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", reservation.LocationID);
            ViewBag.OwnerVehicleID = new SelectList(db.OwnerVehicles, "OwnerVehicleID", "Make", reservation.OwnerVehicleID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
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
