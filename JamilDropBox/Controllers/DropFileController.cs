﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JamilDropBox.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace JamilDropBox.Controllers
{
    public class DropFileController : Controller
    {

        private readonly IConfiguration _configuration;
        public DropFileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile files)
        {
            string blobstorageconnection = _configuration.GetValue<string>("blobstorage");

            byte[] dataFiles;

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("jamilcontainers");

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            string systemFileName = files.FileName;
            await cloudBlobContainer.SetPermissionsAsync(permissions);
            await using (var target = new MemoryStream())
            {
                files.CopyTo(target);
                dataFiles = target.ToArray();
            }

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(systemFileName);
            await cloudBlockBlob.UploadFromByteArrayAsync(dataFiles, 0, dataFiles.Length);

            return View();
        }

        public async Task<IActionResult> Create(IFormFile files)
        {
            string systemFileName = files.FileName;
            string blobstorageconnection = _configuration.GetValue<string>("blobstorage");
            // Retrieve storage account from connection string.
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            // Create the blob client.
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("jamilcontainers");
            // This also does not make a service call; it only creates a local object.
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(systemFileName);
            await using (var data = files.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(data);
            }
            return View("Create");
        }

        public async Task<IActionResult> ShowFileList()
        {
            string blobstorageconnection = _configuration.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            // Create the blob client.
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("jamilcontainers");
            CloudBlobDirectory dirb = container.GetDirectoryReference("jamilcontainers");


            BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                true, BlobListingDetails.Metadata, 100, null, null, null);
            List<DropFile> fileList = new List<DropFile>();

            foreach (var blobItem in resultSegment.Results)
            {
                // A flat listing operation returns only blobs, not virtual directories.
                var blob = (CloudBlob)blobItem;
                fileList.Add(new DropFile()
                {
                    FileName = blob.Name,
                    FileSize = Math.Round((blob.Properties.Length / 1024f) / 1024f, 2).ToString(),
                    ModifiedOn = DateTime.Parse(blob.Properties.LastModified.ToString()).ToLocalTime().ToString(),
                    Descriptions = blob.ToString()
                });
            }

            return View(fileList);
        }

        public async Task<IActionResult> Download(string blobName)
        {
            CloudBlockBlob blockBlob;
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                string blobstorageconnection = _configuration.GetValue<string>("blobstorage");
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("jamilcontainers");
                blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
                await blockBlob.DownloadToStreamAsync(memoryStream);
            }

            Stream blobStream = blockBlob.OpenReadAsync().Result;
            return File(blobStream, blockBlob.Properties.ContentType, blockBlob.Name);
        }

        public async Task<IActionResult> Delete(string blobName)
        {
            string blobstorageconnection = _configuration.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "jamilcontainers";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            var blob = cloudBlobContainer.GetBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
            return RedirectToAction("ShowFileList", "DropFile");
        }
    }
}