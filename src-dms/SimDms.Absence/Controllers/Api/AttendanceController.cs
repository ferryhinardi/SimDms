using Newtonsoft.Json;
using SimDms.Absence.Controllers.Utilities;
using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GeLang;
using System.Configuration;

namespace SimDms.Absence.Controllers.Api
{
    public class AttendanceController : BaseController
    {
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            ResultModel result = new ResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (file != null)
            {
                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);

                string fileHash = ComputeHash(file.InputStream);
                var data = ctxDoc.HrAbsenceFiles.Find(fileHash);
                if (data != null)
                {
                    result.status = true;
                    result.message = "Your file has been uploaded before.";
                    result.data = new
                    {
                        FileID = fileHash,
                        FileName = data.FileName,
                        FileSize = utility.CalculateSize(data.FileSize),
                        FileType = data.FileType,
                        UploadedDate = data.UploadedDate
                    };
                }
                else
                {
                    data = new HrAbsenceFile();
                    data.FileID = fileHash;
                    data.FileName = file.FileName;
                    data.FileType = file.ContentType;
                    data.FileSize = file.ContentLength;
                    data.FileContent = rawData;
                    data.UploadedBy = userId;
                    data.UploadedDate = currentTime;

                    ctxDoc.HrAbsenceFiles.Add(data);
                }

                try
                {
                    ctxDoc.SaveChanges();

                    var extractingStatus = ExtractingData(fileHash);
                    if (extractingStatus)
                    {
                        var hrTrnFileHeader = ctx.HrTrnAttendanceFileHdrs.Where(x => x.CompanyCode==CompanyCode && x.FileID==fileHash).FirstOrDefault();

                        if (hrTrnFileHeader == null)
                        {
                            hrTrnFileHeader = new HrTrnAttendanceFileHdr();
                            hrTrnFileHeader.CompanyCode = CompanyCode;
                            hrTrnFileHeader.FileID = fileHash;
                            hrTrnFileHeader.IsTransfered = 0;
                            hrTrnFileHeader.CreatedBy = userId;
                            hrTrnFileHeader.CreatedDate = currentTime;
                            hrTrnFileHeader.GenerateId = Guid.NewGuid().ToString();

                            ctx.HrTrnAttendanceFileHdrs.Add(hrTrnFileHeader);
                            ctx.SaveChanges();
                        }

                        result.status = true;
                        result.message = "Your file has been uploaded.";
                        result.data = new
                        {
                            FileID = fileHash,
                            FileName = data.FileName,
                            FileSize = utility.CalculateSize(data.FileSize),
                            FileType = data.FileType,
                            UploadedDate = data.UploadedDate,
                        };
                    }
                    else
                    {
                        ctxDoc.HrAbsenceFiles.Remove(data);
                        ctxDoc.SaveChanges();

                        result.status = false;
                        result.message = "Sorry, we cannot process your request.";
                    }
                }
                catch
                {
                    result.message = "Sorry, we can't process your request.";
                }
            }
            else
            {
                result.message = "Sorry, we can't process your request.";
            }

            return Json(result);
        }

        private string ComputeHash(Stream inputStream)
        {
            HashAlgorithm ha = System.Security.Cryptography.SHA256.Create();
            return BitConverter.ToString(ha.ComputeHash(inputStream)).Replace("-", "");
        }

        private bool ExtractingData(string fileHash)
        {
            bool result = false;
            var attendanceFile = ctxDoc.HrAbsenceFiles.Find(fileHash);

            if (attendanceFile != null)
            {
                string attendanceType = ConfigurationManager.AppSettings["AttendanceFlatFileType"].ToString();

                string rawContent = new StreamReader(new MemoryStream(attendanceFile.FileContent)).ReadToEnd().Trim().TrimStart('\n', '\r').TrimEnd('\n', '\r');
                string[] splittedContent = rawContent.Split('\n');
                
                switch (attendanceType)
                {
                    case "Attendance_II":
                        try
                        {
                            result = ctx.Database.SqlQuery<bool>("exec uspfn_AbExtractData_II @FileID=@p0, @FileContent=@p1, @UserID=@p2", fileHash, rawContent, CurrentUser.UserId).FirstOrDefault();
                            //bool results = ctx.Database.SqlQuery<bool>("select convert(bit, 0); ", fileHash, rawContent, CurrentUser.UserId).FirstOrDefault();
                            ctx.Database.Connection.Open();
                            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                            //SqlCommand cmd = new SqlCommand();
                            cmd.Connection = ctx.Database.Connection as SqlConnection;
                            cmd.CommandText = "uspfn_AbExtractData_II";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 360;
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@FileID", fileHash);
                            cmd.Parameters.AddWithValue("@FileContent", rawContent);
                            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

                            //ctx.Database.ExecuteSqlCommand
                            ctx.Database.Connection.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                result = Convert.ToBoolean(reader[0] ?? "false");
                            }
                            //ctx.Database.Connection.Close();
                        }
                        catch (Exception) { }
        
                        break;

                    default:
                        try
                        {
                            result = ctx.Database.SqlQuery<bool>("exec uspfn_AbExtractData @FileID=@p0, @FileContent=@p1, @UserID=@p2", fileHash, rawContent, CurrentUser.UserId).FirstOrDefault();
                            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                            //cmd.CommandText = "uspfn_AbExtractData";
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.CommandTimeout = 360;
                            //cmd.Parameters.Clear();
                            //cmd.Parameters.AddWithValue("@FileID", fileHash);
                            //cmd.Parameters.AddWithValue("@FileContent", rawContent);
                            //cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

                            //SqlDataReader reader = cmd.ExecuteReader();
                            //while (reader.Read())
                            //{
                            //    result = Convert.ToBoolean(reader[0] ?? "false");
                            //}

                        }
                        catch (Exception) { }

                        break;
                }

                
            }

            return result;
        }

        [HttpPost]
        public JsonResult LoadUploadedFileData(string AbsenceFile)
        {
            ResultModel result = InitializeResult();

            var attendanceRecords = (from x in ctx.HrTrnAttendanceFileDtlViews
                                     where
                                     x.FileID == AbsenceFile
                                     select x);

            //return Json(GeLang.DataTables<SimDms.Absence.Models.HrTrnAttendanceFileDtlView>.Parse(attendanceRecords, Request));
            return Json(attendanceRecords.KGrid());
        }

        [HttpPost]
        public JsonResult Process()
        {
            ResultModel result = InitializeResult();


            string fileID = Request["FileID"];
            try
            {
                ctx.Database.ExecuteSqlCommand("exec uspfn_AbAssignAttendance @CompanyCode=@p0, @FileID=@p1", CompanyCode, fileID);
                UpdateFileHdrStatus(fileID);

                result.status = true;
                result.message = "Data has been processed.";
            }
            catch (Exception)
            {
                result.message = "Sorry, your request cannot be processed.\nPlease, try again later!";
            }

            return Json(result);
        }

        public JsonResult Progress()
        {
            var progress = ctx.Database.SqlQuery<decimal?>("exec uspfn_CalculateAttendanceUploadProcess").FirstOrDefault();

            return Json(new
            {
                progress = progress
            });
        }

        private void UpdateFileHdrStatus(string fileID)
        {
            int totalRecords = 0;
            int processedRecords = 0;
            int unprocessedRecords = 0;
            int fileStatus = 0;

            totalRecords = (from x in ctx.HrTrnAttendanceFileDtls
                            where
                            x.CompanyCode.Equals(CompanyCode) == true
                            &&
                            x.FileID.Equals(fileID) == true
                            select x).ToList().Count();
            processedRecords = (from x in ctx.HrTrnAttendanceFileDtls
                                where
                                x.CompanyCode.Equals(CompanyCode) == true
                                &&
                                x.IsTransfered == true
                                &&
                                x.FileID.Equals(fileID) == true
                                select x).ToList().Count();
            unprocessedRecords = (from x in ctx.HrTrnAttendanceFileDtls
                                  where
                                  x.CompanyCode.Equals(CompanyCode) == true
                                  &&
                                  x.FileID.Equals(fileID) == true
                                  &&
                                  x.IsTransfered == false
                                  select x).ToList().Count();

            if (unprocessedRecords == totalRecords)
            {
                fileStatus = 0;
            }

            if (processedRecords < totalRecords)
            {
                fileStatus = 1;
            }

            if (processedRecords == totalRecords)
            {
                fileStatus = 2;
            }

            HrTrnAttendanceFileHdr entity = (from x in ctx.HrTrnAttendanceFileHdrs
                                             where
                                             x.FileID.Equals(fileID) == true
                                             select x).FirstOrDefault();
            if (entity != null)
            {
                entity.IsTransfered = fileStatus;
            }

            ctx.SaveChanges();
        }

        //private ResultModel Save(string AbsenceFile)
        //{
        //    ResultModel result = InitializeResult();

        //    HrTrnAttendanceFileHdr entity = (from c in ctx.HrTrnAttendanceFileHdrs
        //                                     where
        //                                     c.FileID.Equals(AbsenceFile) == true
        //                                     select c).FirstOrDefault();

        //    if (entity != null)
        //    {
        //        string fileContent = GetAttendanceFileContent(entity.FileID);
        //        if (string.IsNullOrEmpty(fileContent) == false)
        //        {
        //            string[] records = fileContent.Split('\n');
        //            int sequence = 1;
        //            foreach (string record in records)
        //            {
        //                HrTrnAttendanceFileDtl recordEntity;
        //                if (string.IsNullOrEmpty(record) == false)
        //                {
        //                    string[] items = record.Split('\t');

        //                    //string strDate = items[2].ToString();
        //                    string strDate = items[1].ToString();
        //                    string dateFormat = "yyyy-MM-dd HH:mm:ss";

        //                    DateTime attendanceTime = DateTime.ParseExact(strDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);

        //                    recordEntity = new HrTrnAttendanceFileDtl()
        //                    {
        //                        CompanyCode = CurrentUser.CompanyCode,
        //                        GenerateId = Guid.NewGuid().ToString(),
        //                        FileID = AbsenceFile,
        //                        SequenceNo = sequence,
        //                        EmployeeID = items[0],
        //                        //EmployeeName = items[1],
        //                        AttendanceTime = attendanceTime,
        //                        MachineCode = items[3],
        //                        IdentityCode = items[4],
        //                        CreatedBy = CurrentUser.UserId,
        //                        CreatedDate = DateTime.Now
        //                    };

        //                    if (IsAttendanceRecordExist(items[0], attendanceTime) == false)
        //                    {
        //                        ctx.HrTrnAttendanceFileDtls.Add(recordEntity);
        //                    }

        //                }

        //                sequence++;
        //            }

        //            try
        //            {
        //                ctx.SaveChanges();
        //                result.status = true;
        //                result.message = "Data has been processed.";
        //            }
        //            catch (Exception)
        //            {
        //                result.message = "Sorry, your request cannot be processed.\nPlease, try again later!";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        result.message = "Sorry, your uploaded file cannot be found for processing request.";
        //    }

        //    return result;
        //}

        //[HttpPost]
        //public JsonResult UploadFile(HttpPostedFileBase file)
        //{
        //    ResultModel result = InitializeResult();
        //    try
        //    {
        //        result = upload.UploadFile(file, CurrentUser.UserId);
        //        HrTrnAttendanceFileHdr entity;
        //        UploadResultModel uploadResult = (UploadResultModel)result.data;
        //        if (IsFileSavedInAttendanceHdr(uploadResult.fileID) == false)
        //        {
        //            entity = new HrTrnAttendanceFileHdr()
        //            {
        //                CompanyCode = CurrentUser.CompanyCode,
        //                GenerateId = Guid.NewGuid().ToString(),
        //                FileID = uploadResult.fileID,
        //                IsTransfered = 0,
        //                CreatedBy = CurrentUser.UserId,
        //                CreatedDate = DateTime.Now
        //            };

        //            ctx.HrTrnAttendanceFileHdrs.Add(entity);
        //            ctx.SaveChanges();

        //            result.data = new { fileID = entity.FileID };
        //        }

        //        result = Save(uploadResult.fileID);
        //        UploadResultModel data = (from x in ctx.HrUploadedFiles
        //                                  where
        //                                  x.Checksum.Equals(uploadResult.fileID) == true
        //                                  select new UploadResultModel
        //                                  {
        //                                      fileID = x.Checksum,
        //                                      fileName = x.FileName,
        //                                      fileSize = "",
        //                                      size = x.FileSize,
        //                                      uploadedDate = x.UploadedDate
        //                                  }).FirstOrDefault();
        //        data.fileSize = utility.CalculateSize(data.size);
        //        result.data = data;
        //        result.status = true;
        //    }
        //    catch (Exception)
        //    {
        //        result.message = "Sorry, your request cannot be processed.\nPlease, try again later!";
        //    }

        //    return Json(result);
        //}

        //[HttpPost]
        //public JsonResult LoadUploadedFileData(string AbsenceFile)
        //{
        //    ResultModel result = InitializeResult();

        //    var attendanceRecords = (from x in ctx.HrTrnAttendanceFileDtlViews
        //                             where
        //                             x.FileID == AbsenceFile
        //                             select x);

        //    foreach (HrTrnAttendanceFileDtlView record in attendanceRecords)
        //    {
        //        record.IdentityCode = record.IdentityCode.Equals("I") ? "IN" : "OUT";
        //    }

        //    result.message = "Sorry, attendance data cannot be downloaded.\nPlease, try again later.";

        //    return Json(GeLang.DataTables<SimDms.Absence.Models.HrTrnAttendanceFileDtlView>.Parse(attendanceRecords, Request));
        //}

        //private ResultModel Save(string AbsenceFile)
        //{
        //    ResultModel result = InitializeResult();

        //    HrTrnAttendanceFileHdr entity = (from c in ctx.HrTrnAttendanceFileHdrs
        //                                     where
        //                                     c.FileID.Equals(AbsenceFile) == true
        //                                     select c).FirstOrDefault();

        //    if (entity != null)
        //    {
        //        string fileContent = GetAttendanceFileContent(entity.FileID);
        //        if (string.IsNullOrEmpty(fileContent) == false)
        //        {
        //            string[] records = fileContent.Split('\n');
        //            int sequence = 1;
        //            foreach (string record in records)
        //            {
        //                HrTrnAttendanceFileDtl recordEntity;
        //                if (string.IsNullOrEmpty(record) == false)
        //                {
        //                    string[] items = record.Split('\t');

        //                    //string strDate = items[2].ToString();
        //                    string strDate = items[1].ToString();
        //                    string dateFormat = "yyyy-MM-dd HH:mm:ss";

        //                    DateTime attendanceTime = DateTime.ParseExact(strDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);

        //                    recordEntity = new HrTrnAttendanceFileDtl()
        //                    {
        //                        CompanyCode = CurrentUser.CompanyCode,
        //                        GenerateId = Guid.NewGuid().ToString(),
        //                        FileID = AbsenceFile,
        //                        SequenceNo = sequence,
        //                        EmployeeID = items[0],
        //                        //EmployeeName = items[1],
        //                        AttendanceTime = attendanceTime,
        //                        MachineCode = items[3],
        //                        IdentityCode = items[4],
        //                        CreatedBy = CurrentUser.UserId,
        //                        CreatedDate = DateTime.Now
        //                    };

        //                    if (IsAttendanceRecordExist(items[0], attendanceTime) == false)
        //                    {
        //                        ctx.HrTrnAttendanceFileDtls.Add(recordEntity);
        //                    }

        //                }

        //                sequence++;
        //            }

        //            try
        //            {
        //                ctx.SaveChanges();
        //                result.status = true;
        //                result.message = "Data has been processed.";
        //            }
        //            catch (Exception)
        //            {
        //                result.message = "Sorry, your request cannot be processed.\nPlease, try again later!";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        result.message = "Sorry, your uploaded file cannot be found for processing request.";
        //    }

        //    return result;
        //}

        //private string GetAttendanceFileContent(string checksum)
        //{
        //    HrUploadedFile entity = (from x in ctx.HrUploadedFiles
        //                             where x.Checksum.Equals(checksum) == true
        //                             select x).FirstOrDefault();
        //    string fileContent = "";

        //    if (entity != null)
        //    {
        //        fileContent = Encoding.UTF8.GetString(entity.Contents).Trim();
        //    }

        //    return fileContent;
        //}

        //private bool IsAttendanceRecordExist(string employeeID, DateTime attendanceTime)
        //{
        //    bool result = false;

        //    HrTrnAttendanceFileDtl entity = (from x in ctx.HrTrnAttendanceFileDtls
        //                                     where
        //                                     x.EmployeeID.Equals(employeeID) == true
        //                                     &&
        //                                     x.AttendanceTime == attendanceTime
        //                                     select x).FirstOrDefault();

        //    if (entity != null)
        //    {
        //        result = true;
        //    }

        //    return result;
        //}

        //private bool IsFileSavedInAttendanceHdr(string checksum)
        //{
        //    bool result = false;

        //    var data = (from x in ctx.HrTrnAttendanceFileHdrs
        //                where
        //                x.FileID.Equals(checksum) == true
        //                select x).FirstOrDefault();

        //    if (data != null)
        //    {
        //        result = true;
        //    }

        //    return result;
        //}

        //[HttpPost]
        //public JsonResult Process()
        //{
        //    ResultModel result = InitializeResult();


        //    string fileID = Request["AbsenceFile"];
        //    try
        //    {
        //        ctx.Database.ExecuteSqlCommand("exec uspfn_AbAssignAttendance @CompanyCode=@p0, @FileID=@p1", CompanyCode, fileID);
        //        UpdateFileHdrStatus(fileID);

        //        result.status = true;
        //        result.message = "Data has been processed.";
        //    }
        //    catch (Exception)
        //    {
        //        result.message = "Sorry, your request cannot be processed.\nPlease, try again later!";
        //    }

        //    return Json(result);
        //}

        //private void UpdateFileHdrStatus(string fileID)
        //{
        //    int totalRecords = 0;
        //    int processedRecords = 0;
        //    int unprocessedRecords = 0;
        //    int fileStatus = 0;

        //    totalRecords = (from x in ctx.HrTrnAttendanceFileDtls
        //                    where
        //                    x.CompanyCode.Equals(CompanyCode) == true
        //                    &&
        //                    x.FileID.Equals(fileID) == true
        //                    select x).ToList().Count();
        //    processedRecords = (from x in ctx.HrTrnAttendanceFileDtls
        //                        where
        //                        x.CompanyCode.Equals(CompanyCode) == true
        //                        &&
        //                        x.IsTransfered == true
        //                        &&
        //                        x.FileID.Equals(fileID) == true
        //                        select x).ToList().Count();
        //    unprocessedRecords = (from x in ctx.HrTrnAttendanceFileDtls
        //                          where
        //                          x.CompanyCode.Equals(CompanyCode) == true
        //                          &&
        //                          x.FileID.Equals(fileID) == true
        //                          &&
        //                          x.IsTransfered == false
        //                          select x).ToList().Count();

        //    if (unprocessedRecords == totalRecords)
        //    {
        //        fileStatus = 0;
        //    }

        //    if (processedRecords < totalRecords)
        //    {
        //        fileStatus = 1;
        //    }

        //    if (processedRecords == totalRecords)
        //    {
        //        fileStatus = 2;
        //    }

        //    HrTrnAttendanceFileHdr entity = (from x in ctx.HrTrnAttendanceFileHdrs
        //                                     where
        //                                     x.FileID.Equals(fileID) == true
        //                                     select x).FirstOrDefault();
        //    if (entity != null)
        //    {
        //        entity.IsTransfered = fileStatus;
        //    }

        //    ctx.SaveChanges();
        //}


        //public JsonResult TestUpload(HttpPostedFileBase file)
        //{
        //    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
        //    cmd.CommandText = "uspfn_";
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);

        //    return Json(ds);
        //}
    }
}
