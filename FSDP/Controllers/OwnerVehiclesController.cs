using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FSDP.DATA.EF;
using Microsoft.AspNet.Identity;
using FSDP.Utilities;

namespace FSDP.Controllers
{
    public class OwnerVehiclesController : Controller
    {
        private FSDPEntities db = new FSDPEntities();

        // GET: OwnerVehicles
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            UserDetail currentUser = db.UserDetails.Where(ud => ud.UserID == userID).FirstOrDefault();

            var ownerVehicles = db.OwnerVehicles.Where(x => x.OwnerID == currentUser.UserID);
            return View(ownerVehicles.ToList());
        }

        // GET: OwnerVehicles/Details/5
        [Authorize(Roles = "Owner, Admin")]
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
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName");
            return View();
        }

        // POST: OwnerVehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Owner")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OwnerVehicleID,Make,Model,OwnerID,CarPhoto,RequestedRepairs,IsActive,DateAdded")] OwnerVehicle ownerVehicle, HttpPostedFileBase carPhoto)
        {
                #region File Upload

                string file = "NoImage.png";
                if (carPhoto != null)
                {
                    file = carPhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    if (goodExts.Contains(ext.ToLower()) && carPhoto.ContentLength <= 4194304)
                    {
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/Content/Images/VehicleImages/");                        Image convertedImage = Image.FromStream(carPhoto.InputStream);                        int maxImageSize = 500;                        int maxThumbSize = 100;                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);                        #endregion

                    }

                    ownerVehicle.CarPhoto = file;
                }

                #endregion

                ownerVehicle.OwnerID = User.Identity.GetUserId();
                ownerVehicle.DateAdded = DateTime.Now;

                db.OwnerVehicles.Add(ownerVehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
        }

        //Get: Admin Edit
        public ActionResult AdminEdit (int? id)
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

        // GET: OwnerVehicles/Edit/5
        [Authorize(Roles = "Owner, Admin")]
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

            if (User.IsInRole("Admin"))
            {
                return View("AdminEdit", ownerVehicle);
            }

            return View(ownerVehicle);
        }

        // POST: OwnerVehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Owner, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OwnerVehicleID,Make,Model,OwnerID,CarPhoto,RequestedRepairs,IsActive,DateAdded")] OwnerVehicle ownerVehicle, HttpPostedFileBase carPhoto)
        {
            if (User.IsInRole("Admin"))
            {
                db.Entry(ownerVehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LocationSelect", "Reservations");
            }

            if (ModelState.IsValid)
            {

                #region File Upload

                string file = "NoImage.png";
                if (carPhoto != null)
                {
                    file = carPhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    if (goodExts.Contains(ext.ToLower()) && carPhoto.ContentLength <= 4194304)
                    {
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/Content/Images/VehicleImages/");                        Image convertedImage = Image.FromStream(carPhoto.InputStream);                        int maxImageSize = 500;                        int maxThumbSize = 100;                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);                        #endregion

                    }

                    ownerVehicle.CarPhoto = file;
                }

                #endregion

                db.Entry(ownerVehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerID = new SelectList(db.UserDetails, "UserID", "FirstName", ownerVehicle.OwnerID);
            return View(ownerVehicle);
        }

        // GET: OwnerVehicles/Delete/5
        [Authorize(Roles = "Owner")]
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
        [Authorize(Roles = "Owner")]
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
