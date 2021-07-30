using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Controllers
{
    public class BlobtheController : Controller
    {
        private CloudBlobContainer getBlobStorageInformation()
        {
            //step 1: read json
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            //to get key access
            //once link, time to read the content to get the connectionstring
            CloudStorageAccount objectaccount = CloudStorageAccount.Parse(configure["ConnectionStrings:BlobConnection"]);
            CloudBlobClient blobclient = objectaccount.CreateCloudBlobClient();
            //step 2: how to create a new container in the blob storage account.
            CloudBlobContainer container = blobclient.GetContainerReference("imageblob");
            return container;
        }

        //step 3: create a page to show the successful step - whether you success to build the container or not
        public bool CreateBlobContainer()
        {
            //refer to the blob storage connection string
            CloudBlobContainer container = getBlobStorageInformation();
            //Returns true if container does not exists and is created / false if existing
            container.CreateIfNotExistsAsync();
            return true;
        }

        public String UploadBlob(IFormFile images)
        {
            //Finds blob container name
            CloudBlobContainer container = getBlobStorageInformation();

            //Block blob is a type of blob
            CloudBlockBlob blob = container.GetBlockBlobReference(images.FileName);

            //Upload data stream to blob container
            //UploadFromStream creates a new blob if it doesnt exist or overwrites it if it does exist
            //using (var fileStream = System.IO.File.OpenRead(@"C:\Users\user\Desktop\SuperFolder\Wallpaper\Olivia.jpg"))
            using (var fileStream = images.OpenReadStream())
            {
                blob.UploadFromStreamAsync(fileStream).Wait();
            }

            return "Success!";
        }

        public List<String> DisplayBlob(IFormFile images)
        {
            CloudBlobContainer container = getBlobStorageInformation();
            //Step 2: create the empty list to store for the blobs list information
            List<string> blobs = new List<string>();
            //step 3: get the listing record from the blob storage
            BlobResultSegment result = container.ListBlobsSegmentedAsync(null).Result;
            //step 4: to read blob listing from the storage
            foreach (IListBlobItem item in result.Results)
            {
                //step 4.1. check the type of the blob : block blob or directory or page block
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    //Query Images
                    if (blob.Name == images.FileName)
                    {
                        blobs.Add(blob.Uri.ToString());
                    }
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory blob = (CloudBlobDirectory)item;
                    blobs.Add(blob.Uri.ToString());
                }
            }
            return blobs;
        }
    }
}
