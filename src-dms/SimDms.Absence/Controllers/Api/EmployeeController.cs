using Newtonsoft.Json;
using SimDms.Absence.Controllers.Utilities;
using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Windows;
using System.Drawing.Imaging;

namespace SimDms.Absence.Controllers.Api
{
    public class EmployeeController : BaseController
    {
        [HttpPost]
        public JsonResult Save(HrEmployee models)
        {
            string AdditionalJob1 = Request["AdditionalJob1"] ?? "";
            string AdditionalJob2 = Request["AdditionalJob2"] ?? "";
            string JoinBranch = Request["JoinBranch"];

            ResultModel result = InitializeResult();
            bool isNew = false;
            var currentDate = DateTime.Now;
            var userID = CurrentUser.UserId;
            HrEmployee entity = ctx.HrEmployees.Find(CompanyCode, models.EmployeeID);

            if (models.JoinDate == null)
            {
                result.message = "Join Date cannot be blank.";

                return Json(result);
            }

            if (models.ResignDate <= models.JoinDate)
            {
                result.message = "Resign date cannot equal or less than join date.";

                return Json(result);
            }

            if (entity == null)
            {
                entity = new HrEmployee();
                entity.CompanyCode = CompanyCode;
                entity.CreatedBy = userID;
                entity.CreatedDate = currentDate;

                HrEmployeeAchievement position = new HrEmployeeAchievement();
                position.CompanyCode = CompanyCode;
                position.CreatedBy = userID;
                position.CreatedDate = currentDate;
                position.EmployeeID = models.EmployeeID;
                position.AssignDate = models.JoinDate;
                position.IsJoinDate = true;
                position.Department = models.Department;
                position.Position = models.Position;
                position.Grade = models.Grade;
                position.IsDeleted = false;

                ctx.HrEmployeeAchievements.Add(position);
                ctx.HrEmployees.Add(entity);

                isNew = true;
            }
            else
            {
                if (entity.JoinDate != null &&  entity.JoinDate.Value.Date != models.JoinDate.Value.Date)
                {
                    var lpos = ctx.HrEmployeeAchievements.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == models.EmployeeID);
                    foreach (var pos in lpos)
                    {
                        if (pos.IsJoinDate == true && pos.AssignDate.Value.Date != models.JoinDate.Value.Date)
                        {
                            pos.IsJoinDate = false;
                            pos.UpdatedBy = userID;
                            pos.UpdatedDate = currentDate;
                        }

                        if (pos.IsJoinDate == false && pos.AssignDate.Value.Date == models.JoinDate.Value.Date)
                        {
                            pos.IsJoinDate = true;
                            pos.UpdatedBy = userID;
                            pos.UpdatedDate = currentDate;
                        }
                    }

                    var lmut = ctx.HrEmployeeMutations.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == models.EmployeeID);
                    foreach (var mut in lmut)
                    {
                        if (mut.IsJoinDate == true && mut.MutationDate.Date != models.JoinDate.Value.Date)
                        {
                            mut.IsJoinDate = false;
                            mut.UpdatedBy = userID;
                            mut.UpdatedDate = currentDate;
                        }

                        if (mut.IsJoinDate == false && mut.MutationDate.Date == models.JoinDate.Value.Date)
                        {
                            mut.IsJoinDate = true;
                            mut.UpdatedBy = userID;
                            mut.UpdatedDate = currentDate;
                        }
                    }
                }
            }

            entity.EmployeeID = models.EmployeeID;
            entity.EmployeeName = models.EmployeeName;
            entity.Email = models.Email;
            entity.FaxNo = models.FaxNo;
            entity.Handphone1 = models.Handphone1;
            entity.Handphone2 = models.Handphone2;
            entity.Handphone3 = models.Handphone3;
            entity.Handphone4 = models.Handphone4;
            entity.Telephone1 = models.Telephone1;
            entity.Telephone2 = models.Telephone2;
            entity.OfficeLocation = models.OfficeLocation;
            entity.IsLinkedUser = string.IsNullOrEmpty(models.RelatedUser) == true ? false : true;
            entity.RelatedUser = models.RelatedUser;
            entity.JoinDate = models.JoinDate;
            entity.Department = models.Department;
            entity.Position = models.Position;
            entity.Grade = models.Grade;
            entity.Rank = models.Rank;
            entity.Gender = models.Gender;
            entity.TeamLeader = models.TeamLeader;
            entity.IsAssigned = !string.IsNullOrEmpty(models.TeamLeader);
            entity.PersonnelStatus = models.PersonnelStatus;
            entity.ResignDate = models.ResignDate;
            entity.ResignDescription = models.ResignDescription;
            entity.ResignCategory = models.ResignCategory;
            entity.IdentityNo = models.IdentityNo;
            entity.NPWPNo = models.NPWPNo;
            entity.NPWPDate = models.NPWPDate;
            entity.BirthDate = models.BirthDate;
            entity.BirthPlace = models.BirthPlace;
            entity.Address1 = models.Address1;
            entity.Address2 = models.Address2;
            entity.Address3 = models.Address3;
            entity.Address4 = models.Address4;
            entity.Province = models.Province;
            entity.District = models.District;
            entity.SubDistrict = models.SubDistrict;
            entity.Village = models.Village;
            entity.ZipCode = models.ZipCode;
            entity.DrivingLicense1 = models.DrivingLicense1;
            entity.DrivingLicense2 = models.DrivingLicense2;
            entity.MaritalStatus = models.MaritalStatus;
            entity.MaritalStatusCode = models.MaritalStatusCode;
            entity.Height = models.Height;
            entity.Weight = models.Weight;
            entity.UniformSize = models.UniformSize;
            entity.UniformSizeAlt = models.UniformSizeAlt;
            entity.ShoesSize = models.ShoesSize;
            entity.FormalEducation = models.FormalEducation;
            entity.BloodCode = models.BloodCode;
            entity.OtherInformation = models.OtherInformation;
            entity.Religion = models.Religion;
            entity.UpdatedBy = userID;
            entity.UpdatedDate = currentDate;


            HrEmployeeAdditionalJob entityAdditionalJob1 = (from x in ctx.HrEmployeeAdditionalJobs
                                                            where
                                                            x.CompanyCode.Equals(CompanyCode) == true
                                                            &&
                                                            x.EmployeeID.Equals(models.EmployeeID) == true
                                                            &&
                                                            x.SeqNo == 1
                                                            select x).FirstOrDefault();

            if (string.IsNullOrEmpty(AdditionalJob1) == false)
            {
                if (entityAdditionalJob1 == null)
                {
                    entityAdditionalJob1 = new HrEmployeeAdditionalJob();

                    entityAdditionalJob1.CompanyCode = CompanyCode;
                    entityAdditionalJob1.EmployeeID = models.EmployeeID;
                    entityAdditionalJob1.SeqNo = 1;

                    ctx.HrEmployeeAdditionalJobs.Add(entityAdditionalJob1);
                }

                entityAdditionalJob1.Department = models.Department;
                entityAdditionalJob1.Position = AdditionalJob1;
                entityAdditionalJob1.AssignDate = currentDate;
            }
            else
            {
                if (entityAdditionalJob1 != null)
                {
                    ctx.HrEmployeeAdditionalJobs.Remove(entityAdditionalJob1);
                }
            }

            HrEmployeeAdditionalJob entityAdditionalJob2 = (from x in ctx.HrEmployeeAdditionalJobs
                                                            where
                                                            x.CompanyCode == (CompanyCode)
                                                            &&
                                                            x.EmployeeID == (models.EmployeeID)
                                                            &&
                                                            x.SeqNo == 2
                                                            select x).FirstOrDefault();

            if (string.IsNullOrEmpty(AdditionalJob2) == false)
            {
                
                if (entityAdditionalJob2 == null)
                {
                    entityAdditionalJob2 = new HrEmployeeAdditionalJob();

                    entityAdditionalJob2.CompanyCode = CompanyCode;
                    entityAdditionalJob2.EmployeeID = models.EmployeeID;
                    entityAdditionalJob2.SeqNo = 2;

                    ctx.HrEmployeeAdditionalJobs.Add(entityAdditionalJob2);
                }

                entityAdditionalJob2.Department = models.Department;
                entityAdditionalJob2.Position = AdditionalJob2;
                entityAdditionalJob2.AssignDate = currentDate;

                //entityAdditionalJob2[0].Department = models.Department;
                //entityAdditionalJob2[0].Position = AdditionalJob2;
                //entityAdditionalJob2[0].AssignDate = currentDate;
            }
            else
            {
                if (entityAdditionalJob2 != null)
                {
                    ctx.HrEmployeeAdditionalJobs.Remove(entityAdditionalJob2);
                }
            }


            //clear spesific user to access
            if (!string.IsNullOrEmpty(models.RelatedUser))
            {
                ctx.Database.SqlQuery<object>("update hremployee set RelatedUser='' where CompanyCode='" + CompanyCode + "' and employeeid !='" + models.EmployeeID + "' and RelatedUser='" + models.RelatedUser + "'").FirstOrDefault();
            }

            //var oldAssignedUser = ctx.HrEmployees.Where(x => x.RelatedUser == models.RelatedUser && x.EmployeeID != models.EmployeeID).ToList();
            //if (oldAssignedUser.Count > 0)
            //{
            //    oldAssignedUser[0].RelatedUser = "";
            //    oldAssignedUser[0].IsLinkedUser = false;
            //}

            if (models.PersonnelStatus == "1")
            {
                models.ResignDate = null;
                models.ResignDescription = "";
            }

            if (models.PersonnelStatus == "3" || models.PersonnelStatus == "4")
            {
                if (models.ResignDate == null)
                {
                    result.message = "Resign date cannot be empty if EMployee has been resign.";
                    return Json(result);
                }
            }

            try
            {

                ctx.SaveChanges();

                ctx.Database.SqlQuery<object>("exec uspfn_HrUpdateJoinDateMutation @CompanyCode=@p0, @EmployeeID=@p1, @JoinDate=@p2, @ResignDate=@p3", CompanyCode, models.EmployeeID, models.JoinDate, models.ResignDate).FirstOrDefault();
                ctx.Database.SqlQuery<object>("exec uspfn_HrUpdateJoinDateAchievement @CompanyCode=@p0, @EmployeeID=@p1, @JoinDate=@p2, @ResignDate=@p3", CompanyCode, models.EmployeeID, models.JoinDate, models.ResignDate).FirstOrDefault();
                ctx.Database.SqlQuery<object>("exec uspfn_HrUpdateJoinDateTraining @CompanyCode=@p0, @EmployeeID=@p1, @JoinDate=@p2, @ResignDate=@p3", CompanyCode, models.EmployeeID, models.JoinDate, models.ResignDate).FirstOrDefault();

                result.status = true;
                result.message = "Employee data has been saved.";
                if (isNew == true)
                {
                    try
                    {
                        ctx.Database.SqlQuery<object>("exec uspfn_HrInsertNewEmployeeMutation @CompanyCode=@p0, @EmployeeID=@p1, @MutationDate=@p2, @BranchCode=@p3, @IsJoinDate=@p4, @Createdby=@p5, @CreatedDate=@p6, @UpdatedBy=@p7, @UpdatedDate=@p8", CompanyCode, models.EmployeeID, models.JoinDate, JoinBranch, true, userID, currentDate, userID, currentDate).FirstOrDefault();
                    }
                    catch (Exception ex) { Elmah.ErrorSignal.FromCurrentContext().Raise(ex);  }
                }
            }
            catch (Exception innerException)
            {
                result.message = "Employee data cannot be saved.\nPlease, try again later!";
                Elmah.ErrorSignal.FromCurrentContext().Raise(innerException);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Find(string employeeID)
        {
            ResultModel result = InitializeResult();

            HrEmployeeView entity = (from x in ctx.HrEmployeeViews
                                     where
                                     x.CompanyCode.Equals(CompanyCode) == true
                                     &&
                                     x.EmployeeID.Equals(employeeID) == true
                                     select x).FirstOrDefault();

            if (entity != null)
            {
                result.data = entity;
            }
            else
            {
                result.message = "Sorry, no data found.";
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult RelatedUser(string employeeID)
        {
            ResultModel result = InitializeResult();


            return Json(result);
        }

        [HttpPost, Authorize]
        public JsonResult GetSalesID()
        {
            string employeeID = Request["EmployeeID"];
            ATPMResultModel result = new ATPMResultModel();
            result.status = false;

            try
            {
                string atpmIdUrl = @"http://dms.suzuki.co.id:9091/api/getatpm";

                string isAllowToGetAtpmID = ConfigurationManager.AppSettings["ATPM_IS_ALLOWED"];

                if (string.IsNullOrEmpty(isAllowToGetAtpmID) == false && isAllowToGetAtpmID.Equals("false") == true)
                {
                    result.message = "Get ATPM ID is not allowed by Administrator.";
                    return Json(result);
                }

                if (string.IsNullOrEmpty(atpmIdUrl) == false)
                {
                    var user = CurrentUser.UserId;
                    string companyCode = CompanyCode;
                    string atpmCode;
                    try
                    {
                        atpmCode = ConfigurationManager.AppSettings["ATPMCode"].ToString();
                    }
                    catch (Exception)
                    {
                        atpmCode = "ATPM";
                    }

                    WebClient client = new WebClient();
                    NameValueCollection param = new NameValueCollection();
                    param.Add("code", atpmCode);
                    param.Add("usr", CurrentUser.UserId);
                    param.Add("pwd", CurrentUser.Password);
                    param.Add("ccode", CompanyCode);

                    byte[] rawResult = client.UploadValues(atpmIdUrl, "POST", param);
                    var rawJson = Content(Encoding.UTF8.GetString(rawResult), "application/json");
                    string cleanJson = rawJson.Content.Substring(6, rawJson.Content.Length - 12);
                    var formattedJson = JsonConvert.DeserializeObject<SalesIDModel2>(cleanJson);

                    if (formattedJson.Nomor > 0)
                    {
                        var hrEmployeeSales = ctx.HrEmployeeSales.Find(CompanyCode, employeeID);
                        if (hrEmployeeSales == null)
                        {
                            hrEmployeeSales = new HrEmployeeSales()
                            {
                                CompanyCode = CompanyCode,
                                EmployeeID = employeeID,
                                CreatedBy = CurrentUser.UserId,
                                CreatedDate = DateTime.Now
                            };

                            ctx.HrEmployeeSales.Add(hrEmployeeSales);
                        }
                        hrEmployeeSales.SalesID = formattedJson.Nomor.ToString();
                        hrEmployeeSales.UpdatedBy = CurrentUser.UserId;
                        hrEmployeeSales.UpdatedDate = DateTime.Now;

                        ctx.SaveChanges();
                        result.status = true;
                        result.message = "ATPM ID generated";
                        result.atpmID = formattedJson.Nomor.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.status = false;
                result.message = "Sorry, ATPM ID cannot be generated because dealer server has a problem while connecting to central server.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveServiceID()
        {
            ResultModel result = InitializeResult();
            string employeeID = Request["EmployeeID"] ?? "";
            string serviceID = Request["ServiceID"] ?? "";
            var currentDate = DateTime.Now;
            var userID = CurrentUser.UserId;

            var data = ctx.HrEmployeeService.Find(CompanyCode, employeeID);
            if (data == null)
            {
                data = new HrEmployeeService()
                {
                    CompanyCode = CompanyCode,
                    EmployeeID = employeeID,
                    CreatedBy = userID,
                    CreatedDate = currentDate
                };
                ctx.HrEmployeeService.Add(data);
            }
            data.ServiceID = serviceID;
            data.UpdatedBy = userID;
            data.UpdatedDate = currentDate;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Your data has been saved.";
            }
            catch (Exception)
            {
                result.message = "Sorry, Service ID cannot be saved.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CheckJoinDate()
        {

            string employeeID = Request["EmployeeID"];
            ResultModel result = InitializeResult();

            if (string.IsNullOrEmpty(employeeID))
            {
                result.message = "";
                return Json(result);
            }

            HrEmployee dataEmployee = ctx.HrEmployees.Where(x => x.CompanyCode.Equals(CompanyCode) == true && x.EmployeeID.Equals(employeeID) == true).FirstOrDefault();
            HrEmployeeAchievement dataAchiemevent = ctx.HrEmployeeAchievements.Where(x => x.CompanyCode.Equals(CompanyCode) == true && x.EmployeeID.Equals(employeeID) == true && x.IsJoinDate == true && x.IsDeleted != true).FirstOrDefault();

            if (dataEmployee != null)
            {
                result.status = true;
                result.data = new
                {
                    JoinDate = dataEmployee.JoinDate,
                    HasJoinDateInAchievement = (dataAchiemevent == null ? false : true)
                };
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetDetailsEmployeePosition()
        {
            string employeeID = Request["EmployeeID"];
            string strTrainingDate = Request["AssignDate"];
            DateTime? trainingDate = DateTime.ParseExact(strTrainingDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            HrEmployeePositionDetails employeePositionDetails = ctx.Database.SqlQuery<HrEmployeePositionDetails>("exec uspfn_HrGetDetailsEmployeePosition @CompanyCode=@p0, @EmployeeID=@p1, @ValidDate=@p2", CompanyCode, employeeID, trainingDate.Value.ToString("yyyy-MM-dd")).FirstOrDefault();

            ResultModel result = this.InitializeResult();

            if (employeePositionDetails != null)
            {
                result.status = true;
                result.data = employeePositionDetails;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetJoinAndResignDate()
        {
            string employeeID = Request["EmployeeID"];
            ResultModel result = InitializeResult();

            HrEmployee data = ctx.HrEmployees.Where(x => x.CompanyCode.Equals(CompanyCode) == true && x.EmployeeID.Equals(employeeID) == true).FirstOrDefault();

            if (data != null)
            {
                result.status = true;
                result.data = new
                {
                    JoinDate = data.JoinDate,
                    ResignDate = data.ResignDate
                };
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult UploadPhoto(HttpPostedFileBase berkas)
        {
            ResultModel result = InitializeResult();

            try
            {
                string employeeID = Request["EmployeeID"];
                string photoType = Request["PhotoType"];
                var userID = CurrentUser.UserId;
                var currenDate = DateTime.Now;

                if (string.IsNullOrEmpty(employeeID))
                {
                    result.message = "Employee ID cannot be null.";
                    return Json(result);
                }

                string fileChecksum = "";
                byte[] rawData;
                
                if (berkas != null)
                {
                    if (berkas.ContentLength > 3000000)
                    {
                        result.message = "Maximum Size Photo 3MB.";
                        return Json(result);
                    }
                    BinaryReader br = new BinaryReader(berkas.InputStream);
                    rawData = br.ReadBytes(berkas.ContentLength);
                    fileChecksum = utility.GetFileChecksum(rawData);
                }
                else
                {
                    result.message = "Your file cannot be uploaded.";
                    return Json(result);
                }

                Session["Photo_" + userID] = rawData;
                Session["PhotoType_" + userID] = berkas.ContentType;
                Session["PhotoFileName_" + userID] = berkas.FileName;
                result.status = true;
                result.message = "Your photo has been uploaded.";
                result.data = new
                {
                    fileID = fileChecksum
                };
            }
            catch (Exception)
            {
                result.message = "Your photo cannot be uploaded.";
            }

            return Json(result);
        }

        public FileContentResult Photo()
        {
            ResultModel result = InitializeResult();

            string fileID = Request["fileID"];
            string fileName = Guid.NewGuid().ToString();

            GnMstEmployeeDocument docs = ctxDoc.GnMstEmployeeDocuments.Where(x => x.DocumentID.Equals(fileID) == true).FirstOrDefault();

            if (docs != null)
            {
                return File(docs.DocumentContent, docs.DocumentType, fileName);
            }

            return null;
        }

        [HttpPost]
        public JsonResult SaveImage(CropImage model)
        {
            ResultModel result = InitializeResult();
            try
            {
                byte[] rawCroppedImage = CropImage(model);
                string userID = CurrentUser.UserId;
                string companyCode = CompanyCode;
                DateTime currentTime = DateTime.Now;
                string documentID = Guid.NewGuid().ToString();
                string fileName = Session["PhotoFileName_" + userID].ToString();
                string fileType = Session["PhotoType_" + userID].ToString();

                var entity = ctxDoc.GnMstEmployeeDocuments.Where(x => x.DocumentID == documentID).FirstOrDefault();
                if (entity != null)
                {
                    entity.DocumentContent = rawCroppedImage;
                }
                else
                {
                    GnMstEmployeeDocument photo = new GnMstEmployeeDocument()
                    {
                        DocumentID = documentID,
                        CreatedBy = userID,
                        CreatedDate = currentTime,
                        DocumentName = fileName,
                        DocumentType = fileType,
                        UpdatedBy = userID,
                        UpdatedDate = currentTime,
                        DocumentContent = rawCroppedImage
                    };
                    ctxDoc.GnMstEmployeeDocuments.Add(photo);
                }

                var employee = ctx.HrEmployees.Where(x =>
                        x.CompanyCode == companyCode
                        &&
                        x.EmployeeID == model.EmployeeID
                    ).FirstOrDefault();

                if (employee != null)
                {
                    if (model.KindOfPhoto == "SelfPhoto")
                    {
                        employee.SelfPhoto = documentID;
                    }
                    else if (model.KindOfPhoto == "IdentityCardPhoto")
                    {
                        employee.IdentityCardPhoto = documentID;
                    }
                    else if (model.KindOfPhoto == "FamilyCardPhoto") 
                    {
                        employee.FamilyCardPhoto = documentID;
                    }
                }


                ctxDoc.SaveChanges();
                ctx.SaveChanges();

                result.status = true;
                result.message = "Your image has been saved.";
                result.data = new
                {
                    ImageID = documentID
                };
            }
            catch (Exception)
            {
                result.message = "Sorry, your request cannot be processed.\nPlease, try again later.";
            }

            return Json(result);
        }


        [HttpGet]
        private Image GetImage(string imageID)
        {
            Image image = null;
            string userID = CurrentUser.UserId;

            byte[] rawData = (byte[])Session["Photo_" + userID];
            MemoryStream ms = new MemoryStream(rawData);
            image = Image.FromStream(ms);

            return image;
        }

        public FileContentResult TempPhoto()
        {
            string userID = CurrentUser.UserId;
            string fileType = Session["PhotoType_" + userID].ToString();
            byte[] rawData = (byte[])Session["Photo_" + userID];
            string fileName = Guid.NewGuid().ToString();


            if (rawData != null)
            {
                return File(rawData, fileType, fileName);
            }

            return null;
        }

        private byte[] CropImage(CropImage model)
        {
            byte[] rawCroppedImage = null;
            var format = (dynamic)null;
            Image originalImage = GetImage(model.ImageID);

            if (originalImage != null)
            {
                int originalImageHeight = Convert.ToInt32(model.OriginalImageHeight);
                int originalImageWidth = Convert.ToInt32(model.OriginalImageWidth);
                int imageHeight = Convert.ToInt32(model.ImageHeight);
                int imageWidth = Convert.ToInt32(model.ImageWidth);
                
                Bitmap croppedPic = new Bitmap(originalImageWidth, originalImageHeight);
                croppedPic.SetResolution(originalImageWidth, originalImageHeight);

                int x1 = 0,
                    x2 = 0,
                    y1 = 0,
                    y2 = 0;

                int originalCroppedHeight = Convert.ToInt32(model.y2 - model.y1);
                int originalCroppedWidth = Convert.ToInt32(model.x2 - model.x1);
                int scaledCroppedHeight = 0;
                int scaledCroppedWidth = 0;
                int scaledMarginTop = 0;
                int scaledMarginLeft = 0;

                scaledCroppedHeight = originalCroppedHeight * originalImageHeight / imageHeight;
                scaledCroppedWidth = originalCroppedWidth * originalImageWidth / imageWidth;
                //scaledMarginLeft = (originalImageWidth - scaledCroppedWidth) / 2;
                //scaledMarginTop = (originalImageHeight - scaledCroppedHeight) / 2;

                scaledMarginLeft = Convert.ToInt32(model.x1) * scaledCroppedWidth / originalCroppedWidth;
                scaledMarginTop = Convert.ToInt32(model.y1) * scaledCroppedHeight / originalCroppedHeight;

                x1 = scaledMarginLeft;
                x2 = x1 + scaledCroppedWidth;
                y1 = scaledMarginTop;
                y2 = y1 + scaledCroppedHeight;

                Graphics graphic = Graphics.FromImage(croppedPic);
                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.DrawImage(originalImage, new Rectangle(0, 0, originalImageWidth, originalImageHeight), scaledMarginLeft, scaledMarginTop, scaledCroppedWidth, scaledCroppedHeight, GraphicsUnit.Pixel);
                
                #region Test
                Bitmap scaledOriginalImage = new Bitmap(originalImage);

                graphic = Graphics.FromImage(croppedPic);
                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.DrawImage(originalImage, new Rectangle(0, 0, originalImageWidth, originalImageHeight), scaledMarginLeft, scaledMarginTop, scaledCroppedWidth, scaledCroppedHeight, GraphicsUnit.Pixel);
                #endregion

                if (!originalImage.RawFormat.Equals(ImageFormat.Jpeg) && !originalImage.RawFormat.Equals(ImageFormat.Png))
                    format = ImageFormat.Jpeg;
                else
                    format = originalImage.RawFormat;

                MemoryStream ms = new MemoryStream();
                croppedPic.Save(ms, format);
                rawCroppedImage = ms.GetBuffer();
            }

            return rawCroppedImage;
        }

        [HttpPost]
        public JsonResult ChangeEmployeeID()
        {
            string currentEmployeeID = Request["CurrentEmployeeID"] ?? "";
            string newEmployeeID = Request["NewEmployeeID"] ?? "";

            ResultModel result = ctx.Database.SqlQuery<ResultModel>("exec uspfn_ChangeEmployeeID @CompanyCode=@p0, @UserID=@p1, @CurrentEmployeeID=@p2, @NewEmployeeID=@p3", CompanyCode, CurrentUser.UserId, currentEmployeeID, newEmployeeID).FirstOrDefault();

            return Json(result);
        }

        [HttpPost]
        public string ProductType()
        {
            string productType = "4W";

            productType = ctx.GnMstCoProfiles.Select(x => x.ProductType).FirstOrDefault();

            return productType;
        }

        [HttpPost]
        public JsonResult CheckKdp(string employeeID)
        {
            try
            {
                var recCount = ctx.PmKdps.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.EmployeeID == employeeID && x.LastProgress != "LOST").Count();
                if (recCount == 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, msg = "Karyawan tersebut masih mempunyai Jumlah KDP = \"" + recCount + "\". Silahkan melakukan transfer KDP terlebih dahulu di Modul ITS." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Terjadi kesalahan pada Server", error_log = ex.Message });

            }
        }
    }
}
