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
    public class tbMedico_PacienteController : Controller
    {
        private ModelDB db = new ModelDB();

        // GET: tbMedico_Paciente
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).Where(x => x.tbProfissional.IdUser == userId);
            return View(tbMedico_Paciente.ToList());
        }

        // GET: tbMedico_Paciente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbMedico_Paciente tbMedico_Paciente = null; 

            if (User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).FirstOrDefault(x => x.tbProfissional.IdUser == userId && x.IdMedico_Paciente == id);
            }
            else
            {
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).FirstOrDefault(x => x.IdMedico_Paciente == id);
            }

            if (tbMedico_Paciente == null)
            {
                return HttpNotFound();
            }
            return View(tbMedico_Paciente);
        }

        // GET: tbMedico_Paciente/Create
        public ActionResult Create()
        {
            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome");
            ViewBag.IdPaciente = new SelectList(db.tbPaciente, "IdPaciente", "Nome");
            ViewBag.IdProfissional = new SelectList(db.tbProfissional, "IdProfissional", "IdUser");
            return View();
        }

        // POST: tbMedico_Paciente/Create
        // Para se proteger de mais ataques, habilite as propriedades específicas às quais você quer se associar. Para 
        // obter mais detalhes, veja https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPaciente,IdProfissional,InformacaoResumida")] tbMedico_Paciente tbMedico_Paciente, [Bind(Include = "Nome,RG,CPF,DataNascimento,NomeResponsavel,Sexo,Etnia,Endereco,Bairro,IdCidade,TelResidencial,TelCelular")] tbPaciente tbPaciente)
        {
            ModelState.Remove("IdMedico_Paciente");
            ModelState.Remove("tbPaciente.IdPaciente");
            if (ModelState.IsValid)
            {
                //Profissional
                var userId = User.Identity.GetUserId();
                var tbProfissional = db.tbProfissional.First(x => x.IdUser == userId);

                //Paciente
                db.tbPaciente.Add(tbPaciente);
                db.SaveChanges();


                tbMedico_Paciente.tbProfissional = tbProfissional;
                tbMedico_Paciente.tbPaciente = tbPaciente;
                db.tbMedico_Paciente.Add(tbMedico_Paciente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbPaciente.IdCidade);
            ViewBag.IdPaciente = new SelectList(db.tbPaciente, "IdPaciente", "Nome", tbMedico_Paciente.IdPaciente);
            ViewBag.IdProfissional = new SelectList(db.tbProfissional, "IdProfissional", "IdUser", tbMedico_Paciente.IdProfissional);
            return View(tbMedico_Paciente);
        }

        // GET: tbMedico_Paciente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbMedico_Paciente tbMedico_Paciente = null;

            if (User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).FirstOrDefault(x => x.tbProfissional.IdUser == userId && x.IdMedico_Paciente == id);
            }
            else
            {
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).FirstOrDefault(x => x.IdMedico_Paciente == id);
            }
            if (tbMedico_Paciente == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbMedico_Paciente.tbPaciente.IdCidade);
            ViewBag.IdPaciente = new SelectList(db.tbPaciente, "IdPaciente", "Nome", tbMedico_Paciente.IdPaciente);
            ViewBag.IdProfissional = new SelectList(db.tbProfissional, "IdProfissional", "IdUser", tbMedico_Paciente.IdProfissional);
            return View(tbMedico_Paciente);
        }

        // POST: tbMedico_Paciente/Edit/5
        // Para se proteger de mais ataques, habilite as propriedades específicas às quais você quer se associar. Para 
        // obter mais detalhes, veja https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tbMedico_Paciente = db.tbMedico_Paciente.Include(x => x.tbPaciente).FirstOrDefault(x => x.IdMedico_Paciente == id);

            if (TryUpdateModel(tbMedico_Paciente, "", new string[] { "InformacaoResumida", "tbPaciente" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            ViewBag.IdCidade = new SelectList(db.tbCidade, "IdCidade", "nome", tbMedico_Paciente.tbPaciente.IdCidade);
            ViewBag.IdPaciente = new SelectList(db.tbPaciente, "IdPaciente", "Nome", tbMedico_Paciente.IdPaciente);
            ViewBag.IdProfissional = new SelectList(db.tbProfissional, "IdProfissional", "IdUser", tbMedico_Paciente.IdProfissional);
            return View(tbMedico_Paciente);
        }

        // GET: tbMedico_Paciente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbMedico_Paciente tbMedico_Paciente = null;

            if (User.IsInRole("Médico") || User.IsInRole("Nutricionista"))
            {
                var userId = User.Identity.GetUserId();
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).FirstOrDefault(x => x.tbProfissional.IdUser == userId && x.IdMedico_Paciente == id);
            }
            else
            {
                tbMedico_Paciente = db.tbMedico_Paciente.Include(t => t.tbPaciente).Include(t => t.tbProfissional).FirstOrDefault(x => x.IdMedico_Paciente == id);
            }
            if (tbMedico_Paciente == null)
            {
                return HttpNotFound();
            }
            return View(tbMedico_Paciente);
        }

        // POST: tbMedico_Paciente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbMedico_Paciente tbMedico_Paciente = db.tbMedico_Paciente.Find(id);
            db.tbMedico_Paciente.Remove(tbMedico_Paciente);
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
