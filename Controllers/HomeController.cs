using ExpenseTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Dapper;
using Microsoft.Win32;

namespace ExpenseTracking.Controllers
{
    public class HomeController : Controller
    {

        public bool CheckLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("nickname")))
            {
                return false;
            }
            return true;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Ana Sayfa";

            if (!CheckLogin())
            {
                return View("giris");
            }

            using var connection = new SqlConnection(connectionString);

            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            ViewData["id"] = HttpContext.Session.GetInt32("id");
            var userId = HttpContext.Session.GetInt32("id");

            var cate = connection.Query<Category>("SELECT * FROM Category WHERE UserId = @UserId", new { UserId = userId }).ToList();
            var gain = connection.Query<Gain>("SELECT * FROM gain WHERE UserId = @UserId", new { UserId = userId }).ToList();
            var cost = connection.Query<Cost>("SELECT * FROM cost WHERE UserId = @UserId", new { UserId = userId }).ToList();

            var totalIncome = gain.Sum(g => g.GainAmount);
            var totalExpense = cost.Sum(c => c.CostAmount);
            var remainingIncome = totalIncome - totalExpense;

            ViewBag.Cost = cost;
            ViewBag.Gain = gain;
            ViewBag.categories = cate;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.RemainingIncome = remainingIncome;

            return View();
        }

        public IActionResult Kayit()
        {
            ViewData["Title"] = "Kayıt";
            return View();
        }
        [HttpPost]
        public IActionResult Kayit(User model)
        {
            if (!ModelState.IsValid)
            {
                TempData["cost"] = "Eksik alan bulunuyor!";
                return View(model);
            }
            if (!string.Equals(model.Password, model.PassRepeat))
            {
                TempData["cate"] = "Şifreler uyuşmuyor!";
                return View(model);
            }
            using var connection = new SqlConnection(connectionString);
            var register = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE NickName = @NickName", new { model.NickName });
            if (register != null)
            {
                ViewBag.ErrorMessage = "Bu NickName kullanılmakta";
                return View(model);
            }
            var checkMail = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE Email = @Email", new { model.Email });
            if (checkMail != null)
            {
                ViewBag.ErrorMessage = "Bu Mail kullanılmakta";
                return View(model);
            }
            model.CreatedDate = DateTime.Now;
            var sql = "INSERT INTO users (NickName, Name, Email, Password, CreatedDate ) VALUES (@NickName, @Name, @Email, @Password, @CreatedDate)";
            var data = new
            {
                model.NickName,
                model.Name,
                model.Email,
                Password = Helper.Hash(model.Password),
                model.CreatedDate
            };
            var rowsAffected = connection.Execute(sql, data);
            TempData["mesaj"] = "Hoş geldiniz giriş yaparak mali durumunuzu kontrol altına alabilirsiniz";
            return View("giris");


        }

        public IActionResult Giris(string? nickname)
        {
            ViewData["Title"] = "Giriş";
            return View(new User());
        }
        [HttpPost]
        public IActionResult Giris(User model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                TempData["cost"] = "Mail veya şifre boş olamaz!";
                return RedirectToAction("giris");
            }
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM users WHERE Email = @Email AND Password = @Password";
            var user = connection.QuerySingleOrDefault<User>(sql, new { model.Email, Password = Helper.Hash(model.Password) });
            if (user != null)
            {
                HttpContext.Session.SetString("email", user.Email);
                HttpContext.Session.SetString("nickname",user.NickName);
                HttpContext.Session.SetInt32("id", user.Id);
                return RedirectToAction("Index");
            }
            TempData["cost"] = "Mail veya şifre hatalı";
            return RedirectToAction("giris");
        }

        public string? UserNickGetir(string? email)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT NickName FROM users WHERE Email = @Email";
            var userNick = connection.QueryFirstOrDefault<string>(sql, new { Email = email });
            return userNick;
        }
        public int? UserIdGetir(string? email)
        {
            using var connection = new SqlConnection (connectionString);
            var sql = "SELECT Id FROM users WHERE Email = @Email";
            var userId = connection.QueryFirstOrDefault<int?>(sql, new {Email = email});
            return userId;
        }
        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("index");
        }

        public IActionResult Profil(string? email)
        {
            ViewData["Title"] = "Profil";
            var sessionnickname = HttpContext.Session.GetString("nickname");
            if (!CheckLogin())
            {
                ViewBag.ErrorMessage = "Bro?? Login ol!!";
                return View("Message");
            }

            if (string.IsNullOrEmpty(email))
            {
                email = HttpContext.Session.GetString("email");
            }

            string? userNick = UserNickGetir(email);
            if (userNick != sessionnickname)
            {
                ViewBag.ErrorMessage = "Herkes kendi sayfasına hocam :)";
                return View("Message");
            }

            using var connection = new SqlConnection(connectionString);
            var user = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE Email = @Email", new { Email = email });
            return View(user);
        }

        [HttpPost]
        [Route("NickEdit/{id}")]
        public IActionResult NickEdit(User model)
        {
            using var connection = new SqlConnection(connectionString);

            var login = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE NickName = @NickName", new {model.NickName});
            if (login != null)
            {
                ViewData["nickname"] = HttpContext.Session.GetString("nickname");
                TempData["cost"] = "Bu isim mevcut";
                return RedirectToAction("profil");

            }
            var sql = "UPDATE users SET NickName = @NickName WHERE Id = @Id";
            var param = new
            {
                model.NickName,
                model.Id
            };
            var rowAffected = connection.Execute(sql, param);
            HttpContext.Session.SetString("nickname", model.NickName);
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            TempData["mesaj"] = "Nick güncellenmiştir";
            return RedirectToAction("profil");


        }
        [HttpPost]
        [Route("NameEdit/{id}")]
        public IActionResult NameEdit(User model)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = "UPDATE users SET Name = @Name WHERE Id = @Id";
            var param = new
            {
                model.Name,
                model.Id
            };
            var rowAffected = connection.Execute(sql, param);
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            TempData["mesaj"] = "İsim güncellenmiştir";
            return RedirectToAction("profil");
        }

        [HttpPost]
        [Route("PassEdit/{id}")]
        public IActionResult PassEdit(User model)
        {
            if (model.Password != model.PassRepeat)
            {
                ViewData["nickname"] = HttpContext.Session.GetString("nickname");
                TempData["cost"] = "Şifreler uyuşmuyor.";
                return RedirectToAction("profil",model);

            }
            using var connection = new SqlConnection(connectionString);
            var sql = "UPDATE users SET Password = @Password WHERE Id = @Id";
            model.Password = Helper.Hash(model.Password);
            var data = new
            {
                model.Password,
                model.Id
            };
            var rowAffected = connection.Execute(sql, data);
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            TempData["mesaj"] = "Şifreniz güncellenmiştir";
            return RedirectToAction("profil");
        }
        [HttpPost]
        [Route("FotoEdit/{id}")]
        public IActionResult FotoEdit(User model)
        {
            if (model.Img == null || model.Img.Length == 0)
            {
                return RedirectToAction("Profil", new { model.NickName });
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "UPDATE users SET ImgPath = @ImgPath WHERE Id = @Id";
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Img.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.Img.CopyTo(fileStream);
            model.ImgPath = fileName;

            var data = new
            {
                model.ImgPath,
                model.Id
            };
            var rowAffected = connection.Execute(sql, data);
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");

            TempData["mesaj"] = "Profil fotorafınız güncellenmiştir";
            return RedirectToAction("profil"); ;
        }
    }
}
