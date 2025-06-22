using Microsoft.AspNetCore.Mvc;

namespace ktm_project.Controllers
{
    public class ChatBotController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetResponse(string userInput)
        {
            string response = GetBotResponse(userInput);
            return Json(new { response });
        }
            
        private string GetBotResponse(string userInput)
        {
            // Simple hardcoded responses
            if (userInput.Contains("How can I buy a ticket?", StringComparison.OrdinalIgnoreCase))
            {
                return "You can purchase tickets online through our website, via our mobile app, or at ticket counters at the stations.!";
            }
            else if (userInput.Contains("What are the ticket prices?", StringComparison.OrdinalIgnoreCase))
            {
                return "Ticket prices vary based on the distance traveled and the type of train service.For detailed pricing information, please visit our 'Fares' section on the website.";
            }
            else if (userInput.Contains("Is there a discount for students?", StringComparison.OrdinalIgnoreCase))
            {
                return "Yes, students are eligible for a 20% discount on all train tickets. Please present a valid student ID when purchasing your ticket.";
            }
            else if (userInput.Contains("Can I bring my bicycle on the train?", StringComparison.OrdinalIgnoreCase))
            {
                return "Yes, bicycles are allowed on our trains. Please make sure to use the designated bike areas and secure your bike properly during the journey.";
            }
            else if (userInput.Contains("Are pets allowed on the train?", StringComparison.OrdinalIgnoreCase))
            {
                return "Small pets are allowed on our trains as long as they are kept in a carrier. Service animals are permitted to accompany passengers with disabilities.";
            }
            else if (userInput.Contains("What safety measures are in place due to COVID-19?", StringComparison.OrdinalIgnoreCase))
            {
                return "We have implemented enhanced cleaning protocols, mandatory mask policies, and social distancing measures to ensure the safety of our passengers and staff. Hand sanitizers are available at all stations and on board the trains.";
            }
            else if (userInput.Contains("What should I do if I lose something on the train?", StringComparison.OrdinalIgnoreCase))
            {
                return "If you lose an item on the train, please contact our lost and found department at (555) 123-4567 or visit the lost and found office at the nearest station.";
            }
            else
            {
                return "I'm sorry, I don't understand that. Can you please rephrase?";
            }
        }
    }
}
