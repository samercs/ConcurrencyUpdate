using ConcurrencyUpdate.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ConcurrencyUpdate.Controllers
{
    public class DepartmentsController : Controller
    {
        private MyContext db = new MyContext();

        // GET: Departments
        public ActionResult Index()
        {
            return View(db.Departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Budget,StartDate,Administrator")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var department = db.Departments.Find(id.Value);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            var bindedProp = new string[] { "Name", "Budget", "StartDate", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department departmentToEdit = db.Departments.Find(id);
            if (departmentToEdit == null)
            {
                var deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, bindedProp);
                ModelState.AddModelError(String.Empty, "Unable to save changes. The department was deleted by another user.");
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(departmentToEdit, bindedProp))
            {
                try
                {
                    db.Entry(departmentToEdit).OriginalValues["RowVersion"] = rowVersion;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entity = ex.Entries.Single();
                    var clientValues = (Department)entity.Entity;
                    var databaseEntry = entity.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(String.Empty,
                            "Unable to save changes. The department was deleted by another user.");

                    }
                    else
                    {
                        var dbValues = (Department)databaseEntry.ToObject();
                        if (dbValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", "Current value: " + dbValues.Name);
                        }
                        if (dbValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", "Current value: " + dbValues.Budget);
                        }
                        if (dbValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", "Current value: " + dbValues.StartDate);
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                                               + "was modified by another user after you got the original value. The "
                                                               + "edit operation was canceled and the current values in the database "
                                                               + "have been displayed. If you still want to edit this record, click "
                                                               + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToEdit.RowVersion = dbValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(departmentToEdit);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                                                  + "was modified by another user after you got the original values. "
                                                  + "The delete operation was canceled and the current values in the "
                                                  + "database have been displayed. If you still want to delete this "
                                                  + "record, click the Delete button again. Otherwise "
                                                  + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Department department)
        {
            try
            {
                db.Entry(department).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return RedirectToAction("Delete", new {id = department.Id, concurrencyError = true});
            }
            catch(DataException e)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
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
