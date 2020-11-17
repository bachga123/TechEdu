
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    public class ClassesController : ApiController
    {
        private TechEduEntities db = new TechEduEntities();
        //GET api/Classes
        public IQueryable<tb_Class> GetClasses()
        {
            return db.tb_Class;     
        }
        [ResponseType(typeof(tb_Class))]
        public IHttpActionResult GetClass(int id)
        {
            tb_Class customer = db.tb_Class.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult PutClass(int id,tb_Class cls)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id!=cls.ClassId)
            {
                return BadRequest();
            }
            db.Entry(cls).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                {
                    return NotFound();
                }
                else
                    throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [ResponseType(typeof(tb_Class))]
        public IHttpActionResult PostClass(tb_Class clas)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.tb_Class.Add(clas);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = clas.ClassId }, clas);
        }
        [ResponseType(typeof(tb_Class))]
        public IHttpActionResult DeleteClass(int id)
        {
            tb_Class clas = db.tb_Class.Find(id);
            if(clas == null)
            {
                return NotFound();
            }
            db.tb_Class.Remove(clas);
            db.SaveChanges();
            return Ok(clas);
        }
        private bool ClassExists(int id)
        {
            return db.tb_Class.Count(m => m.ClassId == id) > 0;
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
