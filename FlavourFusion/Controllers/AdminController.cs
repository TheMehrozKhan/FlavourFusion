using FlavourFusion.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace FlavourFusion.Controllers
{
    public class AdminController : Controller
    {
        FlavourFusionEntities2 db = new FlavourFusionEntities2();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Tbl_Admin adm)
        {
            Tbl_Admin ad = db.Tbl_Admin.Where(x => x.username == adm.username && x.password == adm.password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.admin_id.ToString();
                Session["ad_name"] = ad.username;
                TempData["ToastMessage"] = "Hi, " + ad.username + " You Successfully Logged In!";
                return RedirectToAction("Admin_Panel");
            }
            else
            {
                ViewBag.Error = "Invalid Username or Password";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Admin_Panel()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Add_Recipe_Category()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add_Recipe_Category(Tbl_Recipe_Category cat, HttpPostedFileBase imgfile)
        {
            Tbl_Admin ad = new Tbl_Admin();
            string path = uploadimage(imgfile);
            if (path.Equals(-1))
            {
                ViewBag.Error = "Image Couldn't Uploaded Try Again";
            }
            else
            {
                Tbl_Recipe_Category ca = new Tbl_Recipe_Category();
                ca.category_name = cat.category_name;
                ca.category_image = path;
                ca.category_status = 1;
                ca.FK_Admin_Category = Convert.ToInt32(Session["ad_id"].ToString());
                db.Tbl_Recipe_Category.Add(ca);
                db.SaveChanges();
                return RedirectToAction("View_Category");
            }
            return View();
        }

        public ActionResult View_Category(int? page)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            int pageSize = 6;
            int pageIndex = 1;

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = db.Tbl_Recipe_Category.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<Tbl_Recipe_Category> cateList = list.ToPagedList(pageIndex, pageSize);

            return View(cateList);
        }

        [HttpGet]
        public ActionResult Recipe_Add()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            List<Tbl_Recipe_Category> li = db.Tbl_Recipe_Category.ToList();
            ViewBag.categorylist = new SelectList(li, "category_id", "category_name");
            return View();
        }

        [HttpPost]
        public ActionResult Recipe_Add(Tbl_Recipe rc, HttpPostedFileBase[] imgfiles)
        {
            List<Tbl_Recipe_Category> li = db.Tbl_Recipe_Category.ToList();
            ViewBag.categorylist = new SelectList(li, "category_id", "category_name");
            List<string> imagePaths = new List<string>();

            if (imgfiles != null && imgfiles.Length > 0)
            {
                foreach (HttpPostedFileBase imgfile in imgfiles)
                {
                    string path = uploadimage(imgfile);

                    if (path.Equals(-1))
                    {
                        ViewBag.Error = "Image Couldn't Uploaded Try Again";
                        return View();
                    }

                    imagePaths.Add(path);
                }
            }

            Tbl_Recipe rec = new Tbl_Recipe();
            rec.recipe_name = rc.recipe_name;
            rec.recipe_description = rc.recipe_description;
            rec.FK_Category_Recipe = rc.FK_Category_Recipe;
            rec.recipe_ingredients = rc.recipe_ingredients;
            rec.recipe_duration = rc.recipe_duration;
            rec.recipe_serving_people = rc.recipe_serving_people;
            rec.recipe_tags = rc.recipe_tags;
            rec.recipe_publish_date = rc.recipe_publish_date;

            if (imagePaths.Count > 0)
            {
                rec.recipe_image = string.Join(",", imagePaths);
            }

            db.Tbl_Recipe.Add(rec);
            db.SaveChanges();

            return View();
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            Tbl_Recipe_Category ca = db.Tbl_Recipe_Category.Where(x => x.category_id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult Delete(int? id, Tbl_Recipe_Category cat)
        {
            Tbl_Recipe_Category ca = db.Tbl_Recipe_Category.Where(x => x.category_id == id).SingleOrDefault();
            db.Tbl_Recipe_Category.Remove(ca);
            db.SaveChanges();
            return RedirectToAction("View_Category");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Edit_Category(int id)
        {
            Tbl_Recipe_Category category = db.Tbl_Recipe_Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult Save_Category(Tbl_Recipe_Category category, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);
            if (path.Equals(-1))
            {
                ViewBag.Error = "Image Couldn't Uploaded Try Again";
            }
            else
            {
                Tbl_Recipe_Category ca = db.Tbl_Recipe_Category.Find(category.category_id);
                ca.category_name = category.category_name;
                if (imgfile != null)
                {
                    ca.category_image = path;
                }
                db.SaveChanges();
                return RedirectToAction("View_Category");
            }
            return View(category);
        }

        public ActionResult ViewAllRecipes()
        {
            List<Tbl_Recipe> recipeList = db.Tbl_Recipe.ToList();

            List<RecipeViewModel> recipeViewModelList = recipeList.Select(x => new RecipeViewModel
            {
                recipe_id = x.recipe_id,
                recipe_name = x.recipe_name,
                recipe_description = x.recipe_description,
                recipe_image = x.recipe_image,
                category_id = x.Tbl_Recipe_Category.category_id,
                category_name = x.Tbl_Recipe_Category.category_name,
                admin_id = Convert.ToInt16(x.Tbl_Recipe_Category.FK_Admin_Category),
            }).ToList();

            return View(recipeViewModelList);
        }

        [HttpGet]
        public ActionResult DeleteRecipe(int? id)
        {
            Tbl_Recipe ca = db.Tbl_Recipe.Where(x => x.recipe_id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteRecipe(int? id, Tbl_Recipe_Category cat)
        {
            Tbl_Recipe ca = db.Tbl_Recipe.Where(x => x.recipe_id == id).SingleOrDefault();
            db.Tbl_Recipe.Remove(ca);
            db.SaveChanges();
            return RedirectToAction("ViewAllRecipes");
        }

        public ActionResult RegisteredUser(int? id)
        {
            List<Tbl_Users> userList = db.Tbl_Users.ToList();

            foreach (var user in userList)
            {
                bool isMember = user.is_member ?? false;
                user.MembershipStatus = isMember ? "Yes" : "No";
            }

            return View(userList);
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