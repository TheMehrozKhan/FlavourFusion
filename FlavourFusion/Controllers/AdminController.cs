using System.Web.Mvc;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using FlavourFusion.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace FlavourFusion.Controllers
{
    public class AdminController : Controller
    {
        FlavourFusionEntities4 db = new FlavourFusionEntities4();
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

            var announcements = db.Tbl_Announcement.ToList();
            var comments = db.Tbl_Comments.ToList();
            var contests = db.Tbl_Contests.ToList();
            var recipes = db.Tbl_Recipe.ToList();
            var replies = db.Tbl_Replies.ToList();
            var submissions = db.Tbl_Submissions.ToList();
            var users = db.Tbl_Users.ToList();

            ViewBag.Announcements = announcements;
            ViewBag.Comments = comments;
            ViewBag.Contests = contests;
            ViewBag.Recipes = recipes;
            ViewBag.Replies = replies;
            ViewBag.Submissions = submissions;
            ViewBag.Users = users;
            ViewBag.ToastMessage = TempData["ToastMessage"];

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

        [HttpGet]
        public ActionResult CreateContest()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateContest(Tbl_Contests contest, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);
            if (path.Equals(-1))
            {
                ViewBag.Error = "Image Couldn't Uploaded Try Again";
            }
            else
            {
                Tbl_Contests ca = new Tbl_Contests();
                ca.Contest_Id = contest.Contest_Id;
                ca.ContestName = contest.ContestName;
                ca.ContestDescription = contest.ContestDescription;
                ca.StartDate = contest.StartDate;
                ca.EndDate = contest.EndDate;
                db.Tbl_Contests.Add(ca);
                db.SaveChanges();
                return RedirectToAction("ViewContest");
            }
            return View();
        }

        public ActionResult ViewContest()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            List<Tbl_Contests> userList = db.Tbl_Contests.ToList();
            return View(userList);
        }

        [HttpGet]
        public ActionResult DeleteContest(int? id)
        {
            Tbl_Contests ca = db.Tbl_Contests.Where(x => x.Contest_Id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteContest(int? id, Tbl_Contests cat)
        {
            Tbl_Contests ca = db.Tbl_Contests.Where(x => x.Contest_Id == id).SingleOrDefault();
            db.Tbl_Contests.Remove(ca);
            db.SaveChanges();
            return RedirectToAction("ViewContest");
        }

        [HttpGet]
        public ActionResult ViewSubmissions()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            List<Tbl_Submissions> submissions = db.Tbl_Submissions.ToList();
            return View(submissions);
        }

        [HttpGet]
        public ActionResult DeleteSubmission(int? id)
        {
            Tbl_Submissions ca = db.Tbl_Submissions.Where(x => x.Submission_Id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteSubmission(int? id, Tbl_Submissions cat)
        {
            Tbl_Submissions ca = db.Tbl_Submissions.Where(x => x.Submission_Id == id).SingleOrDefault();
            db.Tbl_Submissions.Remove(ca);
            db.SaveChanges();
            return RedirectToAction("ViewSubmissions");
        }

        public void SendWinnerNotificationEmail(string recipientEmail)
        {
            string senderEmail = "khanmehroz245@gmail.com";
            string apiKey = "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";
            string subject = "Congratulations! You're The Winner of FlavourFusion Contest!";
            string body = "Dear participant, congratulations! Your recipe has been selected as the winner of the contest.";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail);
            var to = new EmailAddress(recipientEmail);
            var plainTextContent = body;
            var htmlContent = "<strong>" + body + "</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg).Result;

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine($"Failed to send email: {response.StatusCode}");
            }
        }


        [HttpGet]
        public ActionResult SelectWinner(int submissionId)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var submission = db.Tbl_Submissions.FirstOrDefault(s => s.Submission_Id == submissionId);
            if (submission != null)
            {
                var contestId = submission.ContestId;
                var contestSubmissions = db.Tbl_Submissions.Where(s => s.ContestId == contestId).ToList();

                if (submission.WinnerStatus ?? false)
                {
                    TempData["ErrorMessage"] = "You have already declared this recipe as the winner.";
                }
                else
                {
                    submission.WinnerStatus = true;
                    db.SaveChanges();

                    foreach (var contestSubmission in contestSubmissions)
                    {
                        if (contestSubmission.Submission_Id != submissionId)
                        {
                            contestSubmission.WinnerStatus = false;
                        }
                    }
                    db.SaveChanges();

                    var user = db.Tbl_Users.FirstOrDefault(u => u.user_id == submission.UserId);
                    if (user != null)
                    {
                        try
                        {
                            SendWinnerNotificationEmail(user.user_email);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email: {ex.Message}");
                        }
                    }
                }
            }

            if (submission != null)
            {
                return RedirectToAction("ViewSubmissions", new { contestId = submission.ContestId });
            }
            else
            {
                return RedirectToAction("ViewContests");
            }
        }


        [HttpGet]
        public ActionResult Add_Annoucement()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add_Annoucement(Tbl_Announcement ann)
        {

            Tbl_Announcement an = new Tbl_Announcement();
            an.Annoucement_Text = ann.Annoucement_Text;
            an.Annoucement_Description = ann.Annoucement_Description;
            an.Annoucement_Date = ann.Annoucement_Date;
            db.Tbl_Announcement.Add(an);
            db.SaveChanges();

            return View();
        }

        public ActionResult View_Annoucement(int? id)
        {
            List<Tbl_Announcement> announcements = db.Tbl_Announcement.ToList();
            return View(announcements);
        }

        [HttpGet]
        public ActionResult DeleteAnnoucement(int? id)
        {
            Tbl_Announcement ca = db.Tbl_Announcement.Where(x => x.Annoucement_id == id).SingleOrDefault();
            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteAnnoucement(int? id, Tbl_Announcement cat)
        {
            Tbl_Announcement ca = db.Tbl_Announcement.Where(x => x.Annoucement_id == id).SingleOrDefault();
            db.Tbl_Announcement.Remove(ca);
            db.SaveChanges();
            return RedirectToAction("View_Annoucement");
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
            rec.recipe_direction = HttpUtility.HtmlDecode(rc.recipe_direction);
            rec.recipe_tutorial_video = rc.recipe_tutorial_video;

            
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