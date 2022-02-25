using SmartRP.Domain.Service;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace SmartRP.Controllers
{
	public class UploadController : Controller
	{
		#region Fields

		private readonly IUploadService _uploadServices;

		#endregion

		#region Ctor

		public UploadController(IUploadService uploadServices)
		{
			_uploadServices = uploadServices;
		}

		#endregion

		#region Actions

		//upload file to Server  
		[HttpPost]
        public virtual ActionResult UploadFileToServer(string fileString, string subjectAndUserID)
        {
            string path = ConfigurationManager.AppSettings[fileString];
            HttpPostedFileBase file = Request.Files[0];

            string url = _uploadServices.UploadToServer(file, path, subjectAndUserID);

            return attachmentProcess(url);
        }

        [HttpDelete]
		public virtual ActionResult DeleteFileFromServer(string fileName)
		{
			if (_uploadServices.DeleteFromServer(fileName))
				return Json(new { message = "The file has delete !" }, "text/html");
			else
				return Json(new { message = "Error" }, "text/html");
		}

		#endregion

		#region Helper

		private JsonResult attachmentProcess(string url)
		{
			bool isUploaded = false;
			if (url != null)
			{
				isUploaded = true;
				string message = "100% complete";

				return Json(new
				{
					statusCode = 200,
					status = "File uploaded.",
					file = url,
					isUploaded = isUploaded,
					message = message
				}, "text/html");

			}
			else
			{
				string message = "Error";
				return Json(new
				{
					statusCode = 500,
					status = "Error uploading image.",
					file = string.Empty,
					isUploaded = isUploaded,
					message = message
				}, "text/html");
			}
		}

		#endregion
	}
}