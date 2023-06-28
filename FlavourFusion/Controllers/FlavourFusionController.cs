using System.Data.SqlClient;
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
    public class FlavourFusionController : Controller
    {
        FlavourFusionEntities3 db = new FlavourFusionEntities3();
        public ActionResult Index(int? page)
        {
            var latestRecipes = db.Tbl_Recipe.OrderByDescending(x => x.recipe_id).ToList();
            var categories = db.Tbl_Recipe_Category.ToList();

            var viewModel = new IndexViewModel
            {
                Recipes = latestRecipes,
                Categories = categories
            };

            return View(viewModel);
        }

        public ActionResult CategoryPage(int? page)
        {
            int pageSize = 8;
            int pageIndex = 1;

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = db.Tbl_Recipe_Category.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<Tbl_Recipe_Category> cateList = list.ToPagedList(pageIndex, pageSize);

            return View(cateList);
        }

        public ActionResult Announcement()
        {
            var announcements = db.Tbl_Announcement.OrderByDescending(x => x.Annoucement_id).ToList();
            return View(announcements);
        }

        public ActionResult Recipies(int? id, int? page, string search)
        {
            List<Tbl_Recipe> recipes;

            if (Session["u_id"] != null)
            {
                int userId = Convert.ToInt32(Session["u_id"]);
                Tbl_Users user = db.Tbl_Users.Find(userId);
                int membershipPlanId = user?.Tbl_Subscriptions.FirstOrDefault()?.Tbl_Membership_Plans?.plan_id ?? 0;

                if (membershipPlanId == 1) 
                {
                    recipes = db.Tbl_Recipe.Where(r => r.FK_Category_Recipe == id).OrderByDescending(x => x.recipe_id).Take(20).ToList();
                }
                else if (membershipPlanId == 2) 
                {
                    recipes = db.Tbl_Recipe.Where(r => r.FK_Category_Recipe == id).OrderByDescending(x => x.recipe_id).ToList();
                }
                else 
                {
                    recipes = db.Tbl_Recipe.Where(r => r.FK_Category_Recipe == id).OrderByDescending(x => x.recipe_id).Take(3).ToList();
                }

                ViewBag.MembershipPlanId = membershipPlanId;
            }
            else 
            {
                recipes = db.Tbl_Recipe.Where(r => r.FK_Category_Recipe == id).OrderByDescending(x => x.recipe_id).Take(3).ToList();
                ViewBag.MembershipPlanId = 0; 
            }

            var categoryName = string.Empty;
            if (id != null)
            {
                var category = db.Tbl_Recipe_Category.SingleOrDefault(c => c.category_id == id);
                if (category != null)
                {
                    categoryName = category.category_name;
                }
            }

            ViewBag.CategoryName = categoryName;
            ViewBag.ShowLoginMessage = Session["u_id"] == null;

            return View(recipes);
        }

        public ActionResult RecipeDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Tbl_Recipe recipe = db.Tbl_Recipe.Find(id);
            if (recipe == null)
            {
                return RedirectToAction("Index");
            }

            var categories = db.Tbl_Recipe_Category.ToList();

            var viewModel = new RecipeDetailViewModel
            {
                Recipe = recipe,
                Categories = categories,
                recipe_tags = recipe.recipe_tags
            };

            ViewBag.CategoryName = recipe.Tbl_Recipe_Category.category_name;
            ViewBag.ShowLoginMessage = Session["u_id"] == null;

            var comments = db.Tbl_Comments.Where(c => c.recipe_id == id).ToList();
            viewModel.Comments = comments;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddComment(int recipeId, string commentText)
        {
            int userId = Convert.ToInt32(Session["u_id"]);

            Tbl_Comments comment = new Tbl_Comments
            {
                recipe_id = recipeId,
                user_id = userId,
                comment_text = commentText,
                comment_date = DateTime.Now
            };

            db.Tbl_Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("RecipeDetails", new { id = recipeId });
        }

        [HttpPost]
        public ActionResult AddReply(int commentId, string replyText)
        {
            int userId = Convert.ToInt32(Session["u_id"]);

            Tbl_Replies reply = new Tbl_Replies
            {
                comment_id = commentId,
                user_id = userId,
                reply_text = replyText,
                reply_date = DateTime.Now
            };

            
            Tbl_Comments comment = db.Tbl_Comments.Find(commentId);
            if (comment != null)
            {
                reply.Tbl_Comments = comment;
                db.Tbl_Replies.Add(reply);
                db.SaveChanges();

                return RedirectToAction("RecipeDetails", new { id = comment.recipe_id });
            }

            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            Tbl_Comments ca = db.Tbl_Comments.Where(x => x.comment_id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult Delete(int? id, Tbl_Recipe_Category cat)
        {
            Tbl_Comments ca = db.Tbl_Comments.Include("Tbl_Replies").SingleOrDefault(x => x.comment_id == id);

            foreach (var reply in ca.Tbl_Replies.ToList())
            {
                db.Tbl_Replies.Remove(reply);
            }
            
            db.Tbl_Comments.Remove(ca);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult DeleteReply(int? id)
        {
            Tbl_Replies ca = db.Tbl_Replies.Where(x => x.reply_id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteReply(int? id, Tbl_Recipe_Category cat)
        {
            Tbl_Replies reply = db.Tbl_Replies.Include("Tbl_Comments").SingleOrDefault(x => x.reply_id == id);

            db.Tbl_Replies.Remove(reply);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Search(int? id, int? page, string searchTerm)
        {
            int pageSize = 6;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<Tbl_Recipe> productList;

            if (Session["u_id"] != null)
            {
                int userId = Convert.ToInt32(Session["u_id"]);
                Tbl_Users user = db.Tbl_Users.Find(userId);
                int membershipPlanId = user?.Tbl_Subscriptions.FirstOrDefault()?.Tbl_Membership_Plans?.plan_id ?? 0;

                var query = db.Tbl_Recipe.Where(r => r.recipe_name.ToLower().Contains(searchTerm.ToLower()) || r.recipe_description.ToLower().Contains(searchTerm.ToLower()));

                if (id.HasValue)
                {
                    query = query.Where(r => r.FK_Category_Recipe == id);
                }

                if (membershipPlanId == 1) 
                {
                    productList = query.OrderByDescending(x => x.recipe_id).Take(20).ToList();
                }
                else if (membershipPlanId == 2) 
                {
                    productList = query.OrderByDescending(x => x.recipe_id).ToList();
                }
                else 
                {
                    productList = query.OrderByDescending(x => x.recipe_id).Take(3).ToList();
                }
            }
            else 
            {
                var query = db.Tbl_Recipe.Where(r => r.recipe_name.ToLower().Contains(searchTerm.ToLower()) || r.recipe_description.ToLower().Contains(searchTerm.ToLower()));

                if (id.HasValue)
                {
                    query = query.Where(r => r.FK_Category_Recipe == id);
                }

                productList = query.OrderByDescending(x => x.recipe_id).Take(3).ToList();
            }

            IPagedList<Tbl_Recipe> pro = productList.ToPagedList(pageIndex, pageSize);

            int totalProductsFound = productList.Count;
            ViewBag.TotalProductsFound = totalProductsFound;
            ViewBag.ShowLoginMessage = Session["u_id"] == null;
            ViewBag.SearchTerm = searchTerm;

            return View(pro);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Tbl_Users uvm)
        {
            Tbl_Users us = db.Tbl_Users.FirstOrDefault(x => x.user_name == uvm.user_name && x.user_password == uvm.user_password);
            if (us != null)
            {
                Session["u_id"] = us.user_id.ToString();
                Session["u_name"] = us.user_name;
                TempData["ToastMessage"] = "Hi, " + us.user_name + " You Successfully Logged In!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Invalid Username or Password";
            }
            return View();
        }


        [HttpGet]
        public ActionResult Register(int? planId)
        {
            if (planId == null)
            {
                return RedirectToAction("Pricing");
            }

            Tbl_Membership_Plans plan = db.Tbl_Membership_Plans.FirstOrDefault(p => p.plan_id == planId);
            if (plan == null)
            {
                return RedirectToAction("Pricing");
            }

            RegisterViewModel viewModel = new RegisterViewModel
            {
                PlanId = plan.plan_id,
                PlanName = plan.plan_name,
                PlanPrice = Convert.ToInt32(plan.price),
                PlanDuration = Convert.ToInt32(plan.duration)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model, HttpPostedFileBase imgfile)
        {
            if (ModelState.IsValid)
            {

                string path = uploadimage(imgfile);
                if (path == null)
                {
                    ViewBag.Error = "Image couldn't be uploaded. Please try again.";
                    return View(model);
                }

                bool isUserNameExists = db.Tbl_Users.Any(u => u.user_name == model.UserName);
                if (isUserNameExists)
                {
                    ModelState.AddModelError("UserName", "This user name is already taken.");
                    return View(model);
                }

                Tbl_Users user = new Tbl_Users
                {
                    user_name = model.UserName,
                    user_email = model.UserEmail,
                    user_password = model.UserPassword,
                    user_img = path,
                };

                Tbl_Membership_Plans plan = db.Tbl_Membership_Plans.FirstOrDefault(p => p.plan_id == model.PlanId);
                if (plan != null && (plan.plan_id == 1 || plan.plan_id == 2)) 
                {
                    user.is_member = true; 
                }
                else
                {
                    user.is_member = null; 
                }

                db.Tbl_Users.Add(user);
                db.SaveChanges();

                Tbl_Subscriptions subscription = new Tbl_Subscriptions
                {
                    user_id = user.user_id,
                    plan_id = model.PlanId,
                    start_date = DateTime.Now.Date,
                    end_date = DateTime.Now.Date.AddMonths(model.PlanDuration)
                };
                db.Tbl_Subscriptions.Add(subscription);
                db.SaveChanges();

                TempData["ToastMessage"] = "Hi, " + user.user_name + " Now Your Can Login!";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public ActionResult Contest()
        {
            if (Session["u_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var contests = db.Tbl_Contests.ToList();
            var contestViewModels = contests.Select(contest => new ContestViewModel
            {
                Contest = contest,
                Submission = new Tbl_Submissions()
            }).ToList();

            return View(contestViewModels);
        }


        [HttpGet]
        public ActionResult Participate(int? contestId)
        {
            if (contestId == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var contest = db.Tbl_Contests.FirstOrDefault(c => c.Contest_Id == contestId);
                if (contest == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.ContestId = contestId;
                ViewBag.UserId = Session["u_id"];
                List<Tbl_Recipe_Category> li = db.Tbl_Recipe_Category.ToList();
                ViewBag.categorylist = new SelectList(li, "category_id", "category_name");

                return View();
            }
        }

        [HttpPost]
        public ActionResult Participate(Tbl_Submissions submission, HttpPostedFileBase[] imgfiles)
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

            Tbl_Submissions rec = new Tbl_Submissions();
            rec.Recipe_Category = submission.Recipe_Category;
            rec.RecipeName = submission.RecipeName;
            rec.RecipeDescription = submission.RecipeDescription;
            rec.Recipe_ingredients = submission.Recipe_ingredients;
            rec.Recipe_tags = submission.Recipe_tags;
            rec.SubmissionDate = submission.SubmissionDate;

            if (imagePaths.Count > 0)
            {
                rec.Recipe_image = string.Join(",", imagePaths);
            }

            if (Session["u_id"] != null && int.TryParse(Session["u_id"].ToString(), out int userId))
            {
                rec.ContestId = submission.ContestId;
                rec.UserId = userId;

                db.Tbl_Submissions.Add(rec);
                db.SaveChanges();

                return RedirectToAction("Contest");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult UserProfile(int? id)
        {
            int userId = Convert.ToInt32(Session["u_id"]);

            if (id == null || userId != id)
            {
                return RedirectToAction("UserProfile", new { id = userId });
            }

            Tbl_Users profileUser = db.Tbl_Users.FirstOrDefault(u => u.user_id == id);

            if (profileUser != null)
            {
                bool? isMember = profileUser.is_member;
                string membershipStatus = (isMember!=null) ? "Yes" : "No";

                Tbl_Membership_Plans plan = db.Tbl_Membership_Plans.FirstOrDefault(p => p.plan_id == p.plan_id);
                if (plan != null && plan.plan_id == 1 && plan.plan_id == 0)
                {
                    membershipStatus = "Yes"; 
                }

                ViewBag.MembershipStatus = membershipStatus;

                return View(profileUser);
            }

            ViewBag.ProfileNotFound = true;
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Profile(int id)
        {
            Tbl_Users user = db.Tbl_Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Save_Edit(Tbl_Users user, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);
            if (path.Equals(-1))
            {
                ViewBag.Error = "Image Couldn't be Uploaded. Please try again.";
            }
            else
            {
                Tbl_Users us = db.Tbl_Users.Find(user.user_id);
                if (us != null)
                {
                    us.user_name = user.user_name;
                    us.user_email = user.user_email;
                    us.user_password = user.user_password;
                    if (imgfile != null)
                    {
                        us.user_img = path;
                    }
                    db.SaveChanges();
                    return RedirectToAction("UserProfile");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(user);
        }

        public ActionResult Pricing()
        {
            List<Tbl_Membership_Plans> plans = db.Tbl_Membership_Plans.ToList();
            return View(plans);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Faq()
        {
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