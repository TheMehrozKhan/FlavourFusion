using FlavourFusion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlavourFusion.Controllers
{
    public class FlavourFusionController : Controller
    {
        RecipeEntities db = new RecipeEntities();
        // GET: FlavourFusion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Recipies()
        {
            return View();
        }
        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Tbl_User uvm)
        {
            Tbl_User us = db.Tbl_User.Where(x => x.us_name == uvm.us_name && x.us_password == uvm.us_password).SingleOrDefault();
            if (us != null)
            {
                Session["u_id"] = us.us_id.ToString();
                Session["u_name"] = us.us_name;
                TempData["ToastMessage"] = "Hi, " + us.us_name + " You Successfully Logged In!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Invalid Username or Password";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Tbl_User us, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);
            if (path.Equals(-1))
            {
                ViewBag.Error = "Image Couldn't Uploaded Try Again";
            }
            else
            {
                Tbl_User u = new Tbl_User();
                u.us_name = us.us_name;
                u.us_email = us.us_email;
                u.us_password = us.us_password;
                u.us_img = path;
                u.us_phone = us.us_phone;
                db.Tbl_User.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View();
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try

                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }

                    catch (Exception ex)
                    {

                        path = "-1";

                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }

            }
            else

            {

                Response.Write("<script>alert('Please select a file'); </script>");

                path = "-1";

            }
            return path;
        }
}
}