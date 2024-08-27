using Dapper;
using ExpenseTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ExpenseTracking.Controllers
{
    public class AdminController : Controller
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
            ViewData["Title"] = "İşlem Paneli";
            if (!CheckLogin())
            {
                ViewBag.ErrorMessage = "Lütfen giriş yapınız!";
                return View("Message");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Eksik veya hatalı işlem yaptın";
                return View("Message");
            }
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            ViewData["id"] = HttpContext.Session.GetInt32("id");
            var userId = HttpContext.Session.GetInt32("id");
            using var connection = new SqlConnection(connectionString);
            var cate = connection.Query<Category>("SELECT * FROM Category WHERE UserId = @UserId", new {UserId = userId}).ToList();
            var gain = connection.Query<Gain>("SELECT * FROM gain WHERE UserId = @UserId", new {UserId = userId}).ToList();
            var cost = connection.Query<Cost>("SELECT * FROM cost WHERE UserId = @UserId", new { UserId = userId }).ToList();
            var viewModel = new GainCostAndCate
            {
                Gain = gain,
                Cost = cost,
                categories = cate,
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddCate(Category model)
        {
            if (!ModelState.IsValid)
            {
                TempData["cost"] = "Eksik veya hatalı işlem yaptın";
                return RedirectToAction("index");
            }
            using var connection = new SqlConnection(connectionString);
            model.UserId = (int)HttpContext.Session.GetInt32("id");
            var cate = "INSERT INTO Category (CateName, UserId) VALUES (@CateName, @UserId)";
            var data = new
            {
                model.CateName,
                model.UserId
            };
            var rowsAffected = connection.Execute(cate, data);
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            TempData["cate"] = "Kategori eklenmiştir";
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult DelCate(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM Category WHERE Id = @id";
            var rowsAffected = connection.Execute(sql, new { id });
            TempData["cost"] = "Kategori silinmiştir";
            return RedirectToAction("index");
        }
        public int? UserIdGetir(string? email)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT Id FROM users WHERE Email = @Email";
            var userId = connection.QueryFirstOrDefault<int?>(sql, new { Email = email });
            return userId;
        }
        [HttpPost]
        public IActionResult GainAdd(Gain model)
        {
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            ViewData["id"] = HttpContext.Session.GetInt32("id");
            model.UserId = (int)HttpContext.Session.GetInt32("id");
            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO gain (GainName, GainAmount, UserId) VALUES (@GainName, @GainAmount, @UserId)";
            var data = new
            {
                model.GainName,
                model.GainAmount,
                model.UserId,
            };
            var rowsAffected = connection.Execute(sql,data);
            TempData["mesaj"] = "Gelir eklenmiştir";
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult DelGain(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM gain WHERE Id = @id";
            connection.Execute(sql, new { id });
            TempData["cost"] = "Gelir silinmiştir";
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult CostAdd(Cost model)
        {
            if (model.CategoryId == null)
            {
                TempData["cost"] = "Kategori olmadan ekleyemezsiniz";
                return RedirectToAction("index");
            }
            ViewData["nickname"] = HttpContext.Session.GetString("nickname");
            ViewData["id"] = HttpContext.Session.GetInt32("id");
            model.UserId = (int)HttpContext.Session.GetInt32("id");
            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO cost (CostName, CostAmount, CategoryId, UserId) VALUES (@CostName, @CostAmount, @CategoryId, @UserId)";
            var data = new
            {
                model.CostName,
                model.CostAmount,
                model.CategoryId,
                model.UserId,
            };
            var rowAffected = connection.Execute(sql, data);
            TempData["cost"] = "Gider eklenmiştir";
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult DelCost(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM cost WHERE Id = @id";
            connection.Execute(sql, new { id });
            TempData["cost"] = "Gider silinmiştir";
            return RedirectToAction("index");
        }
    }


}
