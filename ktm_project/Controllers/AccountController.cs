using Microsoft.AspNetCore.Mvc;
using ktm_project.Models;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ktm_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        public AccountController(IConfiguration config)
        {
            this.configuration = config;
        }

        IList<User> GetDbList()
        {
            IList<User> dbList = new List<User>();

            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ktmConnection"));

            string sql = @"SELECT * FROM Users";

            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dbList.Add(new User()
                    {
                        ViewId = reader.GetString(0),
                        Username = reader.GetString(2),
                        Password = reader.GetString(3),
                        Email = reader.GetString(4),

                    });
                }
            }
            catch
            {
                RedirectToAction("Error");
            }
            finally
            {
                conn.Close();
            }

            return dbList;
        }


        public IActionResult Index()
        {
            IList<User> dbList = GetDbList();

            return View(dbList);
        }

        [HttpGet]
        public IActionResult Register()
        {
            User model = new User();
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            string hashpass = model.Password;
            PBKDF2Hash hash = new PBKDF2Hash(hashpass);
            string passwordhash = hash.HashedPassword;

            if (!ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ktmConnection"));
                SqlCommand cmd = new SqlCommand("spRegisterUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", model.UserId);
                cmd.Parameters.AddWithValue("@username", model.Username);
                cmd.Parameters.AddWithValue("@passwordhash", passwordhash);
                cmd.Parameters.AddWithValue("@emailaddress", model.Email);
                

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return View(model);
                }
                finally
                {
                    conn.Close();
                }
                return View("Login");
            }
            else
                return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                User user = null;

                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ktmConnection")))
                {
                    string sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@password", model.Password); // Remember to hash the password for comparison

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            user = new User
                            {
                                ViewId = reader.GetString(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.GetString(3),
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception (not shown here) and show error message
                        ModelState.AddModelError("", "Unable to log in. Please try again.");
                        return View(model);
                    }
                }

                if (user != null)
                {
                    // For simplicity, redirecting to a success page. In a real application, you would set authentication cookies or tokens.
                    return RedirectToAction("Success");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
