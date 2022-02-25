using System.Web.Mvc;
using SmartRP.Domain;
using System.Configuration;
using System.Web;
using AutoMapper;
using SmartRP.Domain.Service;

namespace SmartRP.Controllers
{
	[Authorize]
	public class ReportsController : Controller
	{
		#region Fields

		private readonly IProjectService _projectServices;
		private readonly IUploadService _uploadServices;

		#endregion

		#region Ctor

		public ReportsController(IProjectService projectServices, IUploadService uploadServices)
		{
			_projectServices = projectServices;
			_uploadServices = uploadServices;
		}

		#endregion

		#region Actions
		 

		//upload file to Server storage blob
		[HttpPost]
		//public virtual ActionResult UploadFileToServer()
		//{
		//	string path = ConfigurationManager.AppSettings["uploadPath_Report"];
		//	HttpPostedFileBase file = Request.Files[0];

		//	string url = _uploadServices.UploadToServer(file, path);

		//	return attachmentProcess(url);
		//}

		//[HttpDelete]
		//public virtual ActionResult DeleteFileFromServer(string fileName)
		//{
		//	if (_uploadServices.DeleteFromServer(fileName))
		//		return Json(new { message = "The file has delete !" }, "text/html");
		//	else
		//		return Json(new { message = "Error" }, "text/html");
		//}

		#endregion


		#region Helper

		private ReportViewModel mapReportToVM(Report report)
		{
			ReportViewModel model = new ReportViewModel();
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<Report, ReportViewModel>();
			});
			model = Mapper.Map<Report, ReportViewModel>(report);
			return model;
		}

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
