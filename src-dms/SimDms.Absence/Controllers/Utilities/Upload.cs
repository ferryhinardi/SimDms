using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Utilities
{
    public class Upload
    {
        private Utility utility = new Utility();
        private DataContext ctx = new DataContext();

        public ResultModel UploadFile(HttpPostedFileBase file, string userId)
        {
            ResultModel result = utility.InitializeResult();

            if (file != null)
            {
                try
                {
                    if (file.ContentLength > 0)
                    {
                        BinaryReader br = new BinaryReader(file.InputStream);
                        byte[] rawData = br.ReadBytes(file.ContentLength);
                        string uploadID = utility.GetFileChecksum(rawData);
                        HrUploadedFile entity;

                        if (IsFileHasBeenUploaded(uploadID) == false)
                        {
                            entity = new HrUploadedFile()
                            {
                                Checksum = uploadID,
                                FileType = file.ContentType,
                                FileSize = file.ContentLength,
                                FileName = file.FileName,
                                Contents = rawData,
                                UploadedBy = userId,
                                UploadedDate = DateTime.Now
                            };

                            ctx.HrUploadedFiles.Add(entity);
                            ctx.SaveChanges();

                            result.status = true;
                            
                        }
                        else
                        {
                            entity = (from x in ctx.HrUploadedFiles
                                      where
                                      x.Checksum.Equals(uploadID) == true
                                      select x).FirstOrDefault();
                        }

                        result.data = new UploadResultModel
                        {
                            status = true,
                            fileID = uploadID,
                            fileName = entity.FileName,
                            fileSize = utility.CalculateSize(entity.FileSize),
                            uploadedDate = entity.UploadedDate
                        };
                    }
                }
                catch (Exception)
                {
                    result.status = false;
                    result.message = "File cannot be uploaded to the server.\nPlease, try again later!";
                }
            }

            return result;
        }

        private bool IsFileHasBeenUploaded(string fileChecksum)
        {
            bool isExist = false;

            var data = ctx.HrUploadedFiles.Where(x => x.Checksum.Equals(fileChecksum)).FirstOrDefault();

            if (data != null)
            {
                isExist = true;
            }

            return isExist;
        }
    }
}