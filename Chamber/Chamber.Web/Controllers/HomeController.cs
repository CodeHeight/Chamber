using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.IO;
using System.Web.Hosting;

namespace Chamber.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Join()
        {
            return View();
        }

        public ActionResult UnderDevelopment()
        {
            return View();
        }

        public ActionResult Chamber()
        {
            return View();
        }

        public ActionResult Structure()
        {
            return View();
        }

        public ActionResult Mission()
        {
            return View();
        }

        public ActionResult Coosa()
        {
            return View();
        }

        public ActionResult ThingsToDo()
        {
            return View();
        }

        public FileResult DownloadForm()
        {
            string file = HostingEnvironment.MapPath("~/App_Data/Files/ForestryTaxSeminarFlyer.pdf");
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(file));
        }


        public class EmailDataViewModel
        {
            public string Name { get; set; }
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            public string Message { get; set; }
        }


        public string EmailData(EmailDataViewModel viewModel)
        {
            string api_user = "sendgrid-api-user";
            string api_key = "sendgrid-api-key";
            string toAddress = "chamber@gmail.com";
            string toName = "Chamber Board Members";
            string subject = "Message from Chamber web app";
            string text = viewModel.Name + " " + viewModel.Message;
            string fromAddress = viewModel.Email;
            string url = "https://sendgrid.com/api/mail.send.json";
            // Create a form encoded string for the request body
            string parameters = "api_user=" + api_user + "&api_key=" + api_key + "&to=" + toAddress +
                                "&toname=" + toName + "&subject=" + subject + "&text=" + text +
                                "&from=" + fromAddress;

            try
            {
                //Create Request
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.Method = "POST";
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

                // Create a new write stream for the POST body
                StreamWriter streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream());

                // Write the parameters to the stream
                streamWriter.Write(parameters);
                streamWriter.Flush();
                streamWriter.Close();

                // Get the response
                HttpWebResponse httpResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                // Create a new read stream for the response body and read it
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                string result = streamReader.ReadToEnd();
                return "Email sent!  We will be in touch with you soon.";
                // Write the results to the console
                //Console.WriteLine(result);
            }
            catch (WebException ex)
            {
                // Catch any execptions and gather the response
                HttpWebResponse response = (HttpWebResponse)ex.Response;

                // Create a new read stream for the exception body and read it
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string result = streamReader.ReadToEnd();
                return ex.ToString();
                // Write the results to the console
                //Console.WriteLine(result);
            }
        }
    }
}