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
using WebAPIFinal.Models;

namespace WebAPIFinal.Controllers
{
    public class Tables1Controller : ApiController
    {
        private LoginInfoDBEntities db = new LoginInfoDBEntities();

        // GET: api/Tables1
        public IQueryable<Table> GetTables()
        {
            return db.Tables;
        }

        // GET: api/Tables1/5
        [ResponseType(typeof(Table))]
        public IHttpActionResult GetTable(int id)
        {
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return NotFound();
            }

            return Ok(table);
        }

        // PUT: api/Tables1/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTable(int id, Table table)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != table.AppId)
            {
                return BadRequest();
            }

            db.Entry(table).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableExists(id))
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

        // POST: api/Tables1
        [ResponseType(typeof(Table))]
        public IHttpActionResult PostTable(Table table)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tables.Add(table);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TableExists(table.AppId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = table.AppId }, table);
        }

        // DELETE: api/Tables1/5
        [ResponseType(typeof(Table))]
        public IHttpActionResult DeleteTable(int id)
        {
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return NotFound();
            }

            db.Tables.Remove(table);
            db.SaveChanges();

            return Ok(table);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TableExists(int id)
        {
            return db.Tables.Count(e => e.AppId == id) > 0;
        }
    }
}