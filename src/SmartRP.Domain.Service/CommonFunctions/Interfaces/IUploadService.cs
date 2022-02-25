using System.Web;

namespace SmartRP.Domain.Service
{
	public interface IUploadService
	{
        //string UploadToAzureStorage(HttpPostedFileBase file, string containerName);
        //bool DeleteFromAzureStorage(string fileURL, string containerName);
        //void ListBlobItemFromAzure(string containerName);
        string UploadToServer(HttpPostedFileBase file, string uploadPath, string subjectAndUserID);

        bool DeleteFromServer(string filePath);
	}
}
