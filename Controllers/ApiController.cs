using Dapper;
using ExpenseTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ExpenseTracking.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var users = connection.Query<User>("SELECT * FROM users").ToList();
            return Json(users);
        }
        public IActionResult gainIndex()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var gains = connection.Query<Gain>("SELECT * FROM gain").ToList();
            return Json(gains);
        }
        public IActionResult costIndex()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var costs = connection.Query<Cost>("SELECT * FROM cost").ToList();
            return Json(costs);
        }
        [HttpPost]
        public IActionResult userAdd(User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            model.Password = Helper.Hash(model.Password);
            using var connection = new SqlConnection(connectionString);
            var newRecordId = connection.ExecuteScalar<int>("INSERT INTO users (NickName, Name, Email, Password, CreatedDate ) VALUES (@NickName, @Name, @Email, @Password, @CreatedDate) SELECT SCOPE_IDENTITY()", model);
            model.Id = newRecordId;

            return Ok(model);
        }
        [HttpPost]
        public IActionResult userEdit(User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            model.Password = Helper.Hash(model.Password);
            using var connection = new SqlConnection(connectionString);
            var newEditId = connection.ExecuteScalar<int>("UPDATE users SET NickName = @NickName, Name = @Name, Email = @Email, Password = @Password, ImgPath = @ImgPath WHERE Id = @Id SELECT SCOPE_IDENTITY()", model);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult GainAdds(Gain model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var newRecordGain = connection.ExecuteScalar<int>("INSERT INTO gain (GainName, GainAmount, UserId) VALUES (@GainName, @GainAmount, @UserId) SELECT SCOPE_IDENTITY()",model);
            model.Id = newRecordGain;
            return Ok(model);
        }
        [HttpPost]
        public IActionResult costAdd(Cost model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var newRecordCost = connection.ExecuteScalar<int>("INSERT INTO cost (CostName, CostAmount, CategoryId, UserId) VALUES (@CostName, @CostAmount, @CategoryId, @UserId) SELECT SCOPE_IDENTITY()", model);
            model.Id = newRecordCost;
            return Ok(model);
        }
        [HttpPost]
        public IActionResult gainDel(Gain model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var delRecord = connection.ExecuteScalar<int>("DELETE FROM gain WHERE Id = @Id SELECT SCOPE_IDENTITY()", model);
            model.Id = delRecord;
            return Ok(model);
        }
        [HttpPost]
        public IActionResult costDel(Cost model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    msg = "Eksik ve hatalı bilgi girişi!"
                });
            }
            using var connection = new SqlConnection(connectionString);
            var delRecord = connection.ExecuteScalar<int>("DELETE FROM cost WHERE Id = @Id SELECT SCOPE_IDENTITY()", model);
            model.Id = delRecord;
            return Ok(model);
        }
    }
}
