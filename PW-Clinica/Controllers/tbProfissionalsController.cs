using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PW_Clinica.Models;
using Microsoft.AspNet.Identity;

namespace PW_Clinica.Controllers
{
    [Authorize]
    public class tbProfissionalsController : Controller
    {
        private ModelDB db = new ModelDB();

        // GET: tbProfissionals
        public ActionResult Index()
        {
            IQueryable<tbProfissional> tbProfissional = null;
            if( User.IsInRole("Gerente"))
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato);
            }
            else if (User.IsInRole("GerenteMédico"))
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).Where(x => x.tbContrato.tbPlano.IdPlano == 1 || x.tbContrato.tbPlano.IdPlano == 2);
            }
            else if (User.IsInRole("GerenteNutricionista"))
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).Where(x => x.tbContrato.tbPlano.IdPlano == 3);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).Where(x => x.IdUser == userId );
            }
            return View(tbProfissional.ToList());
        }

        // GET: tbProfissionals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbProfissional tbProfissional = null;

            if (User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdUser == userId && x.IdProfissional == id);
            }
            else
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdProfissional == id);
            }

            if (tbProfissional == null)
            {
                return HttpNotFound();
            }
            return View(tbProfissional);
        }

        // GET: tbProfissionals/Create
        public ActionResult Create()
        {
            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome");
            ViewBag.IdContrato = new SelectList(db.tbContrato, "IdContrato", "IdContrato");
            ViewBag.IdTipoAcesso = new SelectList(db.tbTipoAcesso, "IdTipoAcesso", "Nome");
            return View();
        }

        // POST: tbProfissionals/Create
        // Para se proteger de mais ataques, habilite as propriedades específicas às quais você quer se associar. Para 
        // obter mais detalhes, veja https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCidade,Nome,CPF,CRM_CRN,Especialidade,Logradouro,Numero,Bairro,CEP,Estado,DDD1,DDD2,Telefone1,Telefone2")] tbProfissional tbProfissional)
        {
            ModelState.Remove("IdUser");
            if (ModelState.IsValid)
            {
                db.tbProfissional.Add(tbProfissional);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbProfissional.IdCidade);
            ViewBag.IdContrato = new SelectList(db.tbContrato, "IdContrato", "IdContrato", tbProfissional.IdContrato);
            ViewBag.IdTipoAcesso = new SelectList(db.tbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: tbProfissionals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbProfissional tbProfissional = null;

            if( User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdUser == userId && x.IdProfissional == id);
            } else
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdProfissional == id);
            }

            if (tbProfissional == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbProfissional.IdCidade);
            ViewBag.IdContrato = new SelectList(db.tbContrato, "IdContrato", "IdContrato", tbProfissional.IdContrato);
            ViewBag.IdTipoAcesso = new SelectList(db.tbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // POST: tbProfissionals/Edit/5
        // Para se proteger de mais ataques, habilite as propriedades específicas às quais você quer se associar. Para 
        // obter mais detalhes, veja https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "IdCidade,Nome,CPF,CRM_CRN,Especialidade,Logradouro,Numero,Bairro,CEP,Estado,DDD1,DDD2,Telefone1,Telefone2")] tbProfissional tbProfissional)
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tbProfissional = db.tbProfissional.Find(id);

            if (TryUpdateModel(tbProfissional, "", new string[] { "IdCidade", "Nome", "CPF", "CRM_CRN", "Especialidade", "Logradouro", "Numero", "Bairro", "CEP", "Estado", "DDD1", "DDD2", "Telefone1", "Telefone2" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {

                }
            }

            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbProfissional.IdCidade);
            ViewBag.IdContrato = new SelectList(db.tbContrato, "IdContrato", "IdContrato", tbProfissional.IdContrato);
            ViewBag.IdTipoAcesso = new SelectList(db.tbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: tbProfissionals/Delete/5
        [Authorize(Roles = "Gerente, GerenteMédico, GerenteNutricionista")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbProfissional tbProfissional = null;

            if (User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdUser == userId && x.IdProfissional == id);
            }
            else
            {
                tbProfissional = db.tbProfissional.Include(t => t.tbCidade).Include(t => t.tbContrato).FirstOrDefault(x => x.IdProfissional == id);
            }

            if (tbProfissional == null)
            {
                return HttpNotFound();
            }

            var pacientes = db.tbMedico_Paciente.Where(x => x.IdProfissional == id).ToList();

            if(pacientes.Count > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é possivel excluir um Profissional com pacientes cadastrados!");
            }

            return View(tbProfissional);
        }

        // POST: tbProfissionals/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbProfissional tbProfissional = db.tbProfissional.Find(id);
            db.tbProfissional.Remove(tbProfissional);
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
