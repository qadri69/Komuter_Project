using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using ktm_project.Models;
using System.Data;
using ktm_project.MailSettings;
using System.Security.AccessControl;

namespace ktm_project.Controllers
{
    public class TicketController : Controller
    {
        private readonly IConfiguration configuration;
        public TicketController(IConfiguration config)
        {
            this.configuration = config;
        }

        IList<KtmKomuterModel> GetDbList()
        {
            IList<KtmKomuterModel> dbList = new List<KtmKomuterModel>();

            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ktmConnection"));

            string sql = @"SELECT * FROM KTM_eticketing";

            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dbList.Add(new KtmKomuterModel()
                    {
                        ViewId = reader.GetString(0),
                        ViewDateTime = reader.GetDateTime(1),
                        Name = reader.GetString(2),
                        IdentityPassportNumber = reader.GetString(3),
                        EmailAddress = reader.GetString(4),
                        startDestination = reader.GetInt32(5),
                        endDestination = reader.GetInt32(6),
                        TotalAmount = reader.GetDouble(7),
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
            IList<KtmKomuterModel> dbList = GetDbList();

            return View(dbList);
        }

        [HttpGet]
        public IActionResult KTMeBuy()
        {
            KtmKomuterModel mrt = new KtmKomuterModel();
            mrt.startDestination = mrt.endDestination = -1;
            return View(mrt);
        }

        [HttpPost]
        public IActionResult KTMeBuy(KtmKomuterModel ktm)
        {
            if (!ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ktmConnection"));
                SqlCommand cmd = new SqlCommand("spInsertTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ticketid", ktm.TicketId);
                cmd.Parameters.AddWithValue("@ticketdatetime", ktm.TicketDateTime);
                cmd.Parameters.AddWithValue("@name", ktm.Name);
                cmd.Parameters.AddWithValue("@icorpass", ktm.IdentityPassportNumber);
                cmd.Parameters.AddWithValue("@emailaddress", ktm.EmailAddress);
                cmd.Parameters.AddWithValue("@startdestination", ktm.startDestination);
                cmd.Parameters.AddWithValue("@enddestination", ktm.endDestination);
                cmd.Parameters.AddWithValue("@totalamount", ktm.TotalAmount);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return View(ktm);
                }
                finally
                {
                    conn.Close();
                }
                return View("KTMeReciept", ktm);
            }
            else
                return View(ktm);
        }

        public IActionResult Details(string id)
        {
            IList<KtmKomuterModel> dbList = GetDbList();

            var result = dbList.First(x => x.ViewId == id);

            return View(result);
        }

        public IActionResult SendMail(string id)
        {
            IList<KtmKomuterModel> dblist = GetDbList();
            var result = dblist.First(x => x.ViewId == id);

            var subject = "Ticket Information" + result.ViewId;
            var body = "Ticket Id : " + result.ViewId + "<br>" +
                "Date and time : " + result.ViewDateTime + "<br>" +
                "Sender name : " + result.Name + "<br>" +
                "Receiver name : " + result.IdentityPassportNumber + "<br>" +
                "Receiver address : " + result.EmailAddress + "<br>" +
                "From : " + result.DictstartDestination[result.startDestination] + "<br>" +
                "To : " + result.DictendDestination[result.endDestination] + "<br>" +
                "Amount : " + result.TotalAmount.ToString("c2");

            var mail = new Mail(configuration);

            if (mail.Send(configuration["Gmail:Username"], result.EmailAddress, subject, body))
            {
                ViewBag.Message = "Mail successfully sent to " + result.EmailAddress;
                ViewBag.Body = body;
            }
            else
            {
                ViewBag.Message = "Sent Mail Failed";
                ViewBag.Body = "";
            }
            return View(result);
        }

        public IActionResult faretable(string fareType = "")
        {
            var viewModel = new FareTableViewModel
            {
                Stations = new Dictionary<int, string>
            {
                {0, "PEL.KLANG"},
                {1, "JLN.KASTAM"},
                {2, "KG.RAJA UDA"},
                {3, "TELUK GADUNG"},
                {4, "TELUK PULAI"},
                {5, "KLANG"},
                {6, "BUKIT BADAK"},
                {7, "PADANG JAWA"},
                {8, "SHAH ALAM"},
                {9, "BATU TIGA"},
                {10, "SUBANG JAYA"},
                {11, "SETIA JAYA"},
                {12, "SERI SETIA"},
                {13, "KG.DATO HARUN"},
                {14, "JLN.TEMPLER"},
                {15, "PETALING"},
                {16, "PANTAI DALAM"},
                {17, "ANGKASAPURI"},
                {18, "KL SENTRAL"},
                {19, "KUALA LUMPUR"},
                {20, "BANK NEGARA"},
                {21, "PUTRA"},
                {22, "SENTUL"},
                {23, "BATU KENTONMEN"},
                {24, "KAMPUNG BATU"},
                {25, "TAMAN WAHYU"},
                {26, "BATU CAVES"},
            },
                Rates = new double[,]
            {
                {0.00, 1.40, 1.70, 1.80, 1.80, 2.10, 2.10, 2.50, 2.90, 3.40, 3.70, 4.00, 4.20, 4.20, 4.60, 4.70, 4.80, 5.00, 5.40, 5.60, 5.70, 5.90, 6.10, 6.10, 6.20, 6.30, 6.60},
            {1.40, 0.00, 1.20, 1.80, 1.80, 1.80, 2.10, 2.30, 2.70, 3.20, 3.50, 3.80, 3.90, 4.00, 4.30, 4.40, 4.60, 4.80, 5.20, 5.30, 5.50, 5.70, 5.90, 6.00, 6.00, 6.10, 6.40},
            {1.70, 1.20, 0.00, 1.50, 1.70, 1.70, 2.00, 2.20, 2.60, 3.00, 3.40, 3.70, 3.80, 3.90, 4.20, 4.30, 4.50, 4.70, 5.10, 5.20, 5.40, 5.50, 5.80, 6.00, 6.00, 6.00, 6.30},
            {1.80, 1.80, 1.50, 0.00, 1.20, 1.70, 1.70, 2.20, 2.30, 2.80, 3.20, 3.50, 3.60, 3.70, 4.00, 4.10, 4.20, 4.50, 4.90, 5.00, 5.20, 5.30, 5.60, 5.80, 5.90, 6.00, 6.10},
            {1.80, 1.80, 1.70, 1.20, 0.00, 1.50, 1.70, 2.10, 2.20, 2.70, 3.10, 3.40, 3.50, 3.60, 3.90, 4.00, 4.10, 4.40, 4.80, 4.90, 5.10, 5.20, 5.50, 5.70, 5.80, 5.90, 6.00},
            {2.10, 1.80, 1.70, 1.70, 1.50, 0.00, 1.60, 1.80, 2.20, 2.50, 2.80, 3.10, 3.20, 3.30, 3.60, 3.70, 3.90, 4.10, 4.50, 4.60, 4.80, 5.00, 5.20, 5.40, 5.50, 5.70, 6.00},
            {2.10, 2.10, 2.00, 1.70, 1.70, 1.60, 0.00, 1.80, 1.90, 2.20, 2.50, 2.80, 2.90, 3.00, 3.30, 3.50, 3.60, 3.80, 4.20, 4.30, 4.50, 4.70, 4.90, 5.10, 5.20, 5.40, 5.70},
            {2.50, 2.30, 2.20, 2.20, 2.10, 1.80, 1.80, 0.00, 1.80, 2.00, 2.10, 2.40, 2.50, 2.60, 2.90, 3.10, 3.20, 3.40, 3.80, 3.90, 4.10, 4.30, 4.50, 4.70, 4.80, 5.00, 5.30},
            {2.90, 2.70, 2.60, 2.30, 2.20, 2.20, 1.90, 1.80, 0.00, 2.00, 2.00, 2.00, 2.20, 2.20, 2.60, 2.70, 2.80, 3.00, 3.50, 3.60, 3.70, 3.90, 4.10, 4.40, 4.50, 4.60, 4.90},
            {3.40, 3.20, 3.00, 2.80, 2.70, 2.50, 2.20, 2.00, 2.00, 0.00, 1.70, 1.70, 1.90, 2.00, 2.10, 2.20, 2.30, 2.50, 3.00, 3.10, 3.30, 3.40, 3.70, 3.90, 4.00, 4.10, 4.40},
            {3.70, 3.50, 3.40, 3.20, 3.10, 2.80, 2.50, 2.10, 2.00, 1.70, 0.00, 1.60, 1.90, 1.90, 1.90, 2.10, 2.20, 2.20, 2.60, 2.70, 2.90, 3.00, 3.30, 3.50, 3.60, 3.70, 4.00},
            {4.00, 3.80, 3.70, 3.50, 3.40, 3.10, 2.80, 2.40, 2.00, 1.70, 1.60, 0.00, 1.20, 1.40, 1.70, 1.70, 1.90, 2.20, 2.30, 2.40, 2.60, 2.70, 3.00, 3.20, 3.30, 3.50, 3.80},
            {4.20, 3.90, 3.80, 3.60, 3.50, 3.20, 2.90, 2.50, 2.20, 1.90, 1.90, 1.20, 0.00, 1.10, 1.70, 1.70, 1.90, 2.00, 2.20, 2.30, 2.50, 2.60, 2.90, 3.10, 3.20, 3.30, 3.60},
            {4.20, 4.00, 3.90, 3.70, 3.60, 3.30, 3.00, 2.60, 2.20, 2.00, 1.90, 1.40, 1.10, 0.00, 1.60, 1.60, 1.60, 1.90, 2.10, 2.20, 2.40, 2.50, 2.80, 3.00, 3.10, 3.30, 3.60},
            {4.60, 4.30, 4.20, 4.00, 3.90, 3.60, 3.30, 2.90, 2.60, 2.10, 1.90, 1.70, 1.70, 1.60, 0.00, 1.20, 1.50, 1.90, 2.00, 2.10, 2.10, 2.20, 2.50, 2.70, 2.80, 2.90, 3.20},
            {4.70, 4.40, 4.30, 4.10, 4.00, 3.70, 3.50, 3.10, 2.70, 2.20, 2.10, 1.70, 1.70, 1.60, 1.20, 0.00, 1.30, 1.80, 1.90, 2.00, 2.10, 2.10, 2.40, 2.60, 2.70, 2.80, 3.10},
            {4.80, 4.60, 4.50, 4.20, 4.10, 3.90, 3.60, 3.20, 2.80, 2.30, 2.20, 1.90, 1.90, 1.60, 1.50, 1.30, 0.00, 1.40, 1.70, 1.90, 2.10, 2.10, 2.20, 2.40, 2.50, 2.70, 3.00},
            {5.00, 4.80, 4.70, 4.50, 4.40, 4.10, 3.80, 3.40, 3.00, 2.50, 2.20, 2.20, 2.00, 1.90, 1.90, 1.80, 1.40, 0.00, 1.70, 1.80, 1.80, 2.00, 2.00, 2.20, 2.30, 2.50, 2.80},
            {5.40, 5.20, 5.10, 4.90, 4.80, 4.50, 4.20, 3.80, 3.50, 3.00, 2.60, 2.30, 2.20, 2.10, 2.00, 1.90, 1.70, 1.70, 0.00, 1.20, 1.60, 1.80, 1.80, 2.00, 2.00, 2.00, 2.30},
            {5.60, 5.30, 5.20, 5.00, 4.90, 4.60, 4.30, 3.90, 3.60, 3.10, 2.70, 2.40, 2.30, 2.20, 2.10, 2.00, 1.90, 1.80, 1.20, 0.00, 1.40, 1.60, 1.60, 1.90, 2.00, 2.00, 2.20},
            {5.70, 5.50, 5.40, 5.20, 5.10, 4.80, 4.50, 4.10, 3.70, 3.30, 2.90, 2.60, 2.50, 2.40, 2.10, 2.10, 2.10, 1.80, 1.60, 1.40, 0.00, 1.20, 1.60, 1.70, 1.80, 2.00, 2.00},
            {5.90, 5.70, 5.50, 5.30, 5.20, 5.00, 4.70, 4.30, 3.90, 3.40, 3.00, 2.70, 2.60, 2.50, 2.20, 2.10, 2.10, 2.00, 1.80, 1.60, 1.20, 0.00, 1.50, 1.70, 1.80, 1.80, 2.00},
            {6.10, 5.90, 5.80, 5.60, 5.50, 5.20, 5.90, 4.50, 4.10, 3.70, 3.30, 3.00, 2.90, 2.80, 2.50, 2.40, 2.20, 2.00, 1.80, 1.60, 1.60, 1.50, 0.00, 1.40, 1.70, 1.80, 2.00},
            {6.10, 6.00, 6.00, 5.80, 5.70, 5.40, 5.10, 4.70, 4.40, 3.90, 3.50, 3.20, 3.10, 3.00, 2.70, 2.60, 2.40, 2.20, 2.00, 1.90, 1.70, 1.70, 1.40, 0.00, 1.10, 1.50, 2.00},
            {6.20, 6.00, 6.00, 5.90, 5.80, 5.50, 5.20, 4.80, 4.50, 4.00, 3.60, 3.30, 3.20, 3.10, 2.80, 2.70, 2.50, 2.30, 2.00, 2.00, 1.80, 1.80, 1.70, 1.10, 0.00, 1.20, 2.00},
            {6.30, 6.10, 6.00, 6.00, 5.90, 5.70, 5.40, 5.00, 4.60, 4.10, 3.70, 3.50, 3.30, 3.30, 2.90, 2.80, 2.70, 2.50, 2.00, 2.00, 2.00, 1.80, 1.80, 1.50, 1.20, 0.00, 1.60},
            {6.60, 6.40, 6.30, 6.10, 6.00, 6.00, 5.70, 5.30, 4.90, 4.40, 4.00, 3.80, 3.60, 3.60, 3.20, 3.10, 3.00, 2.80, 2.30, 2.20, 2.00, 2.00, 2.00, 2.00, 2.00, 1.60, 0.00}
            },
                SelectedFareType = fareType
            };

            return View(viewModel);
        }
    }
}
