using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Document.Models;
using System.IO;

namespace SimDms.Document.Controllers
{
    public class ManagementController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            if (HelperController.IsAuthorized((Session["Session"] == null ? "" : Session["Session"].ToString())))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult ManageMenu()
        {

            return View();
        }

        public ActionResult ManageIndex()
        {
            var listItem = GetAllItemIndex();
            ViewData["ListIndex"] = listItem;
            return View();
        }

        public List<SelectListItem> GetAllItemIndex()
        {
            var listItem = (from item in ctx.SysHelps
                            orderby item.MenuLevel, item.MenuSeq ascending
                            select new NewSelectedItem
                            {
                                Value = item.MenuID,
                                Text = item.MenuTitle,
                            }
                                    ).ToList();
            var list = (from item in ctx.SysHelps
                        orderby item.MenuLevel, item.MenuSeq ascending
                        select item).ToList();
            List<SelectListItem> selectItem = new List<SelectListItem>();
            selectItem.Add(new SelectListItem {Text="Parent Index Help", Value="0" });
            for (int i = 0; i < listItem.Count; i++)
            {
                selectItem.Add(new SelectListItem { Value= listItem[i].Value.ToString(),
                Text= GetTotalChild(listItem[i].Value, listItem[i].Text) + GetParentNode(list[i].MenuHeader.Value)});
            }
            return selectItem;
        }

        public JsonResult GetAllItem()
        {
            return Json(GetAllItemIndex());
        }

        public JsonResult GetMenuChild(int MenuID)
        {
            var childMenu = ctx.SysHelps.Where(m => m.MenuHeader == MenuID).OrderBy(m => m.MenuSeq).Select(m => new NewSelectedItem{ Text = m.MenuTitle, Value = m.MenuID }).ToList();
            var child = ctx.SysHelps.Where(m => m.MenuHeader == MenuID).OrderBy(m => m.MenuSeq).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            for (int i = 0; i < childMenu.Count; i++)
            {
                listItem.Add(new SelectListItem {  Text = GetTotalChild(childMenu[i].Value, childMenu[i].Text)+GetParentNode(child[i].MenuHeader.Value),
                Value = childMenu[i].Value.ToString()+"|" + child[i].MenuSeq.ToString()});
            }
            return Json(listItem);
        }

        public string GetTotalChild(int MenuID, string MenuTitle)
        {
            int totalChildMenus = ctx.SysHelps.Where(m => m.MenuHeader == MenuID).ToList().Count;
            MenuTitle += string.Format(" ({0} Item)", totalChildMenus);
            return MenuTitle;
        }

        public string GetParentNode(int MenuHeader)
        {
            string returnVal = "";
            var Item = ctx.SysHelps.Where(m => m.MenuID == MenuHeader).FirstOrDefault();
            if (Item != null)
                returnVal+= " > " + Item.MenuTitle;
            return returnVal;
        }

        public JsonResult AddItemIndex(int MenuID, string MenuTitle, string MenuLevel)
        {
            SysHelp itemMenu = new SysHelp();
            var menuID = GetMenuID();
            itemMenu.MenuID = menuID;
            if (MenuLevel == "0")
            {
                itemMenu.MenuTitle = MenuTitle;
                itemMenu.MenuHeader = 0;
                itemMenu.MenuLevel = 0;
                itemMenu.MenuSeq = GetMenuSeq(0);
            }
            else if(MenuLevel =="1")
            {
                itemMenu.MenuTitle = MenuTitle;
                itemMenu.MenuHeader = MenuID;
                itemMenu.MenuLevel = GetMenuLevel(MenuID);
                itemMenu.MenuSeq = GetMenuSeq(itemMenu.MenuLevel, MenuID);
            }

            itemMenu.CreatedBy = User.Identity.Name;
            itemMenu.LastUpdatedBy = User.Identity.Name;
            itemMenu.CreatedDate = DateTime.Now;
            itemMenu.LastUpdatedDate = DateTime.Now;
            ctx.SysHelps.Add(itemMenu);
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json("");
        }

        public JsonResult EditItemIndex(int MenuID, string MenuTitle, bool IsHeader)
        {
            var itemIndex = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            if (itemIndex != null)
            {
                if (itemIndex.MenuHeader == 0)
                    itemIndex.IsContainModule = IsHeader;
                itemIndex.MenuTitle = MenuTitle;
                itemIndex.LastUpdatedBy = User.Identity.Name;
            }
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json("");
        }

        public JsonResult DeleteItem(int MenuID)
        {
            var item = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            ctx.SysHelps.Remove(item);
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return Json("");
        }

        public int GetMenuID()
        {
            var menuID = ctx.Database.SqlQuery<int>("select TOP 1 menuID from SysHelp order BY CreatedDate DESC").FirstOrDefault();
            if (menuID != null)
            {
                menuID = menuID + 1;
            }
            else
            {
                menuID = 1;
            }
            return menuID;
        }

        public int GetMenuLevel(int MenuID)
        {
            var currentLevel = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault().MenuLevel;
            var menuLevel = currentLevel + 1;
            return menuLevel;
        }

        public int GetMenuSeq(int MenuLevel,int MenuHeader=0)
        {
            int seq =0;
            if (MenuHeader == 0)
            {
                var item = ctx.SysHelps.Where(m => m.MenuHeader == MenuHeader).OrderByDescending(m => m.MenuSeq).Take(1).FirstOrDefault();
                if (item == null)
                {
                    seq = -1;
                }
                else
                {
                    seq = item.MenuSeq;
                }
            }
            else
            {
                var entity= ctx.SysHelps.Where(m => m.MenuLevel == MenuLevel).OrderByDescending(m => m.MenuSeq).Take(1).FirstOrDefault();
                if (entity == null)
                {
                    seq = 0;
                }
            }
            return seq = seq+1;
        }

        public ActionResult ManageContent()
        {
            var listItem = (from item in ctx.SysHelps
                            orderby item.MenuLevel, item.MenuSeq ascending
                            select new NewSelectedItem
                            {
                                Value = item.MenuID,
                                Text = item.MenuTitle,
                            }
                                     ).ToList();
            var list = (from item in ctx.SysHelps
                        orderby item.MenuLevel, item.MenuSeq ascending
                        select item).ToList();
            List<SelectListItem> selectItem = new List<SelectListItem>();
            for (int i = 0; i < listItem.Count; i++)
            {
                selectItem.Add(new SelectListItem
                {
                    Value = listItem[i].Value.ToString(),
                    Text = GetTotalChild(listItem[i].Value, listItem[i].Text) + GetParentNode(list[i].MenuHeader.Value)
                });
            }
            ViewData["ListIndex"] = selectItem;
            return View();
        }

        public ActionResult GetContent(int MenuID, string GetFrom)
        {
            var item = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(item.Content) && !string.IsNullOrEmpty(GetFrom))
            {
                item.Content = "<h1>No Content</h1>";
            }
            return Json(new {content = item.Content });
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult SaveContent(int MenuID, string Content)
        {
            Object returnObj = null;
            var entity = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(Content) || Content == "<br>")
            {
                entity.Content = null;
            }
            else
            {
                entity.Content = Content;
            }
            try
            {
                ctx.SaveChanges();
                returnObj = new {succes=true};
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.ToString() };
                throw new Exception("Error in function SaveContent, Message="+ ex.ToString());
            }
            return Json(returnObj);
        }

        public JsonResult UploadImage(string MenuID, string ImageData)
        {
            //validate 
            var ImgData = ImageData.Replace("data:image/png;base64,", "");
            ImgData = ImgData.Replace("data:image/jpeg;base64,", "");
            byte[] byteImgData = Convert.FromBase64String(ImgData);
            DocImage docImg = new DocImage();
            docImg.MenuID = MenuID;
            docImg.ImageId = Guid.NewGuid().ToString().Replace("-","");
            docImg.ImageData = byteImgData;
            docImg.Caption = "";
            docImg.UploadDate = DateTime.Now;
            docImg.UploadBy = "";
            ctx.DocImage.Add(docImg);

            try 
	        {	        
		        ctx.SaveChanges();
	        }
	        catch (Exception)
	        {
		        throw;
	        }
            return Json("");
        }

        public JsonResult DeleteImage(string ImageID)
        {
            var imgEnt = ctx.DocImage.Where(m=>m.ImageId == ImageID).FirstOrDefault();
            if (imgEnt != null)
            {
                ctx.DocImage.Remove(imgEnt);
            }

            try
            {
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }
            return Json("");
        }

        public ActionResult GetImage(string ImageID)
        {
            var image = ctx.DocImage.Where(m => m.ImageId == ImageID).FirstOrDefault();
            return new FileContentResult(image.ImageData, "image/jpeg");
        }

        public ActionResult ListImageDoc(string MenuID)
        {
            var listImage = ctx.DocImage.Where(m => m.MenuID == MenuID).ToList();
            ViewData["listImage"] = listImage;
            return View();
        }

        public JsonResult EnableUpDownButton(int MenuID)
        {
            var idxItem = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            string EnableBtn = "";
            //get list of sequence
            var listSeq = ctx.Database.SqlQuery<int>("SELECT MenuSeq FROM SysHelp where MenuLevel = {0} AND MenuHeader = {1}", idxItem.MenuLevel, idxItem.MenuHeader).ToList();
            var currentSeq = ctx.Database.SqlQuery<int>("SELECT MenuSeq from SysHelp where MenuID ={0}", MenuID).FirstOrDefault();
            if (listSeq.Max() == currentSeq)
            {
                EnableBtn = "up";
            }
            else if (listSeq.Min() == currentSeq)
            {
                EnableBtn = "down";
            }
            else
            {
                EnableBtn = "enableall";
            }

            return Json(EnableBtn);
        }

        public JsonResult MoveUpDownSequence(int MenuID, string Operation)
        {
            int seqOld=0, seqNew=0;
            bool getSeq = false;
            SysHelp movedIdx = null;
            var idxItem = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            seqOld = idxItem.MenuSeq;
            string EnableBtn = "";
            var listSeq = ctx.SysHelps.Where(m=>m.MenuHeader == idxItem.MenuHeader&& m.MenuLevel == idxItem.MenuLevel).OrderBy(m=>m.MenuSeq).ToList();

            if(Operation.ToLower().Equals("up"))
                listSeq.Reverse();

            int idx = 0;
            foreach (var itemIndex in listSeq)
            {
                if (idxItem.MenuSeq == itemIndex.MenuSeq)
                {
                    getSeq = true;
                }
                idx++;

                if (getSeq)
                {
                    movedIdx = listSeq[idx];
                    seqNew = listSeq[idx].MenuSeq;
                    break;
                }
            }
            idxItem.MenuSeq = seqNew;
            var movedIndex = ctx.SysHelps.Where(m => m.MenuID == movedIdx.MenuID).FirstOrDefault();
            movedIndex.MenuSeq = seqOld;
            ctx.SaveChanges();
            return Json("");
        }

        public JsonResult GetStatusHeader(int MenuID)
        {
            var item = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault().IsContainModule;
            return Json(item);
        }
    }
}
