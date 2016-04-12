using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver.GridFS;
using eXpressAPP;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace eXpressAPI.Models
{
    public class SaveResult
    {
        public SaveResult(int a, bool b)
        {
            Count = a;
            success = b;
            ErrorNo = 0;
        }

        public int Count { get; set; }
        public int ErrorNo { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public DataTable Table()
        {
            if (data != null)
            {
                return ((DataSet)data).Tables[0];
            }
            return null;
        }

        public string Value()
        {
            string data_row = null;
            if (data != null)
            {
                var dt = ((DataSet)data).Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        data_row = dt.Rows[0][0].ToString();
                    }
                }
            }
            return data_row;
        }

        public DataSet Tables()
        {
            if (data != null)
            {
                return ((DataSet)data);
            }
            return null;
        }

        public Grid Result(DataSourceFilter filters)
        {
            if (data == null)
            {
                return null;
            }

            DataTable dataTableSrc = null;

            if (((DataSet)data).Tables.Count > 0)
            {
                dataTableSrc = ((DataSet)data).Tables[0];
            }

            return MyGlobalVar.GetGrid(((DataSet)data).Tables[0], filters);
        }
    }

    public class SearchParam
    {
        public string filename { get; set; }
    }

    public class CodeEditor
    {
        public ObjectId _id { get; set; }
        public string html { get; set; }
        public string css { get; set; }
        public string js { get; set; }
        public string cfg { get; set; }
        public string filename { get; set; }
    }

    public class KeyValues
    {
        public ObjectId _id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }

    public class FileSystems
    {
        public ObjectId _id { get; set; }
        public ObjectId Fileid { get; set; }
        public string AutoNo { get; set; }
        public string FileType { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string UploadBy { get; set; }
        public string UploadDate { get; set; }
        public bool Compress { get; set; }
    }
    
    public class MapDirectory
    {
        public ObjectId _id { get; set; }
        public string FullPath { get; set; }
        public string Caption { get; set; }
        public string Parent { get; set; }
        public string Owner { get; set; }
    }

    public class SearchResult
    {
        public string AutoNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string UploadBy { get; set; }
        public string UploadDate { get; set; }
        public string FileId { get; set; }
        public string Tag { get; set; }
    }

    public class FileSystemsParam
    {
        public string FileType { get; set; }
        public string Folder { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }

    public interface eXpressRepository<T> where T : class
    {
        Task<SaveResult> AddAsync(T t);
        Task<SaveResult> RemoveAsync(T t);
        Task<List<T>> GetAllAsync();
        Task<SaveResult> UpdateAsync(T t);
        Task<int> CountAsync();
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
    }

    public class MongoHelper<T> where T : class
    {
        public MongoCollection<T> Collection { get; private set; }

        public MongoHelper()
        {
            string Url = ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString;
            var con = new MongoUrlBuilder(Url);
            var client = new MongoClient(Url);
            var server = client.GetServer();
            var db = server.GetDatabase(con.DatabaseName);
            Collection = db.GetCollection<T>(typeof(T).Name);
        }
    }

    public class CodeEditorRepository
    {
        public MongoDatabase _database;
        public MongoDatabase _database2;

        private readonly MongoGridFS _gridFs;

        public MongoCollection<CodeEditor> _CodeEditor;
        public MongoCollection<FileSystems> _Files;
        public MongoCollection<KeyValues> _keyValues;
        public MongoCollection<Document> _Document;

        private string FileTypeNotRequired2Compress = null;

        public MongoDatabase _database3;
        private readonly MongoGridFS _gridFs3;
        public MongoCollection<FileSystems> _Files3;
        public MongoCollection<MapDirectory> _Folders;
        
        public CodeEditorRepository()
            : this("")
        {
        }

        public CodeEditorRepository(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                connection = Settings.MongoDbData;
            }

            var _databaseName = MongoUrl.Create(connection).DatabaseName;
            var client = new MongoClient(connection);
            var server = client.GetServer();

            _database2 = server.GetDatabase(_databaseName);
            _gridFs = _database2.GridFS;

            _Files = _database2.GetCollection<FileSystems>("FileInfo");
            _keyValues = _database2.GetCollection<KeyValues>("Configuration");
            _Document = _database2.GetCollection<Document>("Documents");
            
            _databaseName = GetSetting("ui-database");
            _database = server.GetDatabase(_databaseName);
            _CodeEditor = _database.GetCollection<CodeEditor>("CodeEditor");

            _databaseName = GetSetting("cdn-database");
            _database3 = server.GetDatabase(_databaseName);
            _gridFs3 = _database3.GridFS;

            _Files3 = _database3.GetCollection<FileSystems>("files");
            _Folders = _database3.GetCollection<MapDirectory>("folders");

            FileTypeNotRequired2Compress = GetSetting("LIST_FILE_TYPES_DO_NOT_REQUIRED_TO_COMPRESS").ToLower();
        }
        
        public FileSystems GetFileInfo3(string Id)
        {
            IMongoQuery query = Query.EQ("AutoNo", Id);
            return _Files3.FindOne(query);
        }

        public Stream GetFile3(ObjectId id)
        {
            var file = _gridFs3.FindOneById(id);
            return file.OpenRead();
        }

        public void DeleteFile3(string Id)
        {
            IMongoQuery query = Query.EQ("Fileid", ObjectId.Parse(Id));
            _Files3.Remove(query);
            _gridFs3.DeleteById(ObjectId.Parse(Id));
        }

        private void GenerateFolderStructure(string fullPath, string owner)
        {
            var Items = fullPath.Split('/');
            string parent = null;
            string name = owner;

            foreach (var item in Items)
            {
                parent = name;
                name += "/" + item;
                IMongoQuery query1 = Query.EQ("FullPath", name);
                IMongoQuery query2 = Query.EQ("Parent", parent);
                IMongoQuery query3 = Query.EQ("Owner", owner);
                IMongoQuery[] andQuery = { query1, query2, query3 };
                IMongoQuery query = Query.And(andQuery);
                var folder = _Folders.Find(query).FirstOrDefault();
                if (folder == null)
                {
                    var data = new MapDirectory();
                    data._id = ObjectId.GenerateNewId();
                    data.Caption = item;
                    data.Parent = parent;
                    data.Owner = owner;
                    data.FullPath = name;
                    _Folders.Insert(data);
                }
            }
        }

        public FileSystems AddOrUpdateFiles3(FileSystems data, Stream filedata, string filetype)
        {
            //IMongoQuery query1 = Query.EQ("FileName", data.FileName);
            //IMongoQuery query2 = Query.EQ("Folder", data.Folder);
            //IMongoQuery query3 = Query.EQ("FileType", data.FileType);
            
            //IMongoQuery[] andQuery = { query1, query2, query3 };
            //IMongoQuery query = Query.And(andQuery);

            IMongoQuery query = Query.EQ("AutoNo", data.AutoNo);

            var item1 = _Files3.Find(query).FirstOrDefault();

            if (item1 == null)
            {
                data._id = ObjectId.GenerateNewId();

                _Files3.Insert(data);
            }
            else
            {
                try
                {
                    _gridFs3.DeleteById(item1.Fileid);
                }
                catch (Exception) { }
            }

            var fileInfo = _gridFs3.Upload(filedata, data.AutoNo);
            GenerateFolderStructure(data.Folder, data.FileType);
           
            if (fileInfo != null)
            {
                IMongoUpdate update = Update.Set("Fileid", (ObjectId)fileInfo.Id)
                    .Set("FileType", filetype)
                    .Set("UploadBy", data.UploadBy)
                    .Set("UploadDate", data.UploadDate);

                _Files3.Update(query, update);
            }

            return data;
        }

        public IEnumerable<KeyValues> GetAllConfig()
        {
            return _keyValues.FindAll();
        }

        private bool NeedCompress(string fname)
        {
            var i = fname.LastIndexOf('.');
            var ext = fname.Substring(i + 1).ToString().ToLower();
            if (FileTypeNotRequired2Compress.Contains(ext))
            {
                return false;
            }
            return true;
        }

        public KeyValues GetByKey(string key)
        {
            IMongoQuery query = Query.EQ("key", key);
             
            return _keyValues.Find(query).FirstOrDefault();
        }
        
        public string GetSetting(string key)
        {
            string ret = "";
            IMongoQuery query = Query.EQ("key", key);
            var item = _keyValues.Find(query).FirstOrDefault();
            if (item != null)
            {
                ret = item.value;
            }
            return ret;
        }

        public KeyValues AddOrUpdateConfig(KeyValues item, string Id = null)
        {
            IMongoQuery query = Query.EQ("key", item.key);
            if (Id != null)
            {
                query = Query.EQ("_id", ObjectId.Parse(Id));
            }
            var item1 = _keyValues.Find(query).FirstOrDefault();
            if (item1 == null)
            {
                item._id = ObjectId.GenerateNewId();
                _keyValues.Insert(item);
            }
            else
            {
                IMongoUpdate update = Update.Set("value", item.value).Set("key", item.key);
                var result = _keyValues.Update(query, update);
            }

            return item;
        }

        public bool RemoveConfig(string id)
        {
            IMongoQuery query = Query.EQ("key", id);
            var result = _keyValues.Remove(query);
            return result.DocumentsAffected == 1;
        }

        public IEnumerable<CodeEditor> GetAllCode()
        {
            return _CodeEditor.FindAll();
        }

        public CodeEditor GetCodeEditor(string id)
        {
            IMongoQuery query = Query.EQ("_id", id);
            return _CodeEditor.Find(query).FirstOrDefault();
        }

        public CodeEditor GetByFileName(string filename)
        {
            IMongoQuery query = Query.EQ("filename", filename);
            return _CodeEditor.Find(query).FirstOrDefault();
        }

        public string GetCodeByFileName(string fn)
        {
            string ret = "";
            IMongoQuery query = Query.EQ("filename", fn);
            var ce = _CodeEditor.Find(query).FirstOrDefault();
            if (ce != null)
            {
                var minifier = new Microsoft.Ajax.Utilities.Minifier();
                if (fn.Right(3) == ".js")
                {
                    if (fn.Right(6) == "min.js")
                        ret = minifier.MinifyJavaScript(ce.js);
                    else
                        ret = ce.js;
                }
                else if (fn.Right(3) == "css")
                {
                    if (fn.Right(7) == "min.css")
                        ret = minifier.MinifyStyleSheet(ce.css);
                    else
                        ret = ce.css;
                }
                else
                {
                    ret = ce.html;
                }
            }
            return ret;
        }

        public string GetFileTypeDesc(string fn)
        {
            string ret = "application/octet-stream";

            if (fn.Right(3) == ".js")
            {
                ret = "application/javascript";
            }
            else if (fn.Right(3) == "css")
            {
                ret = "text/css";
            }
            else if (fn.Right(4) == "html")
            {
                ret = "text/html";
            }

            return ret;
        }

        
        public void UpdateDocument(Document data)
        {
            IMongoQuery query = Query.EQ("AutoNo", data.AutoNo);
            var item1 = _Document.Find(query).FirstOrDefault();

            if (item1 != null)
            {
                var searchData = Settings.Concat(data.AutoNo, data.DocNo, data.ReffNo, data.Title, data.Description, data.Description2, data.Tag, data.DocType, data.Department);
                var hashKey = Settings.CalculateMD5(searchData);

                IMongoUpdate update = Update.Set("DocType", data.DocType ?? "")
                                        .Set("Department", data.Department ?? "")
                                        .Set("ReffNo", hashKey)
                                        .Set("DocNo", data.DocNo ?? "")
                                        .Set("Title", data.Title ?? "")
                                        .Set("Description", data.Description ?? "")
                                        .Set("Description2", searchData)
                                        .Set("UploadDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                        .Set("Tag", data.Tag ?? "");

                _Document.Update(query, update);
             }
        }
        
        public CodeEditor AddOrUpdateCodeEditor(CodeEditor item)
        {
            IMongoQuery query = Query.EQ("filename", item.filename);
            var item1 = _CodeEditor.Find(query).FirstOrDefault();
            if (item1 == null)
            {
                item._id = ObjectId.GenerateNewId();
                _CodeEditor.Insert(item);
            }
            else
            {
                string cfg = item.cfg ?? "";
                string css = item.css ?? "";
                string html = item.html ?? "";
                string js = item.js ?? "";

                IMongoUpdate update = Update.Set("cfg", cfg)
                .Set("html", html)
                .Set("css", css)
                .Set("js", js);

                var result = _CodeEditor.Update(query, update);
            }

            return item;
        }

        public Stream Compression(Archieve data)
        {
            if (NeedCompress(data.Attachment.FileName))
            {
                MemoryStream outputMemStream = new MemoryStream();
                ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                zipStream.SetLevel(5); //0-9, 9 being the highest level of compression

                {
                    var newEntry = new ZipEntry(data.Attachment.FileName);
                    newEntry.DateTime = DateTime.Now;
                    zipStream.PutNextEntry(newEntry);
                    var fs = data.Attachment.InputStream;
                    byte[] buffer = new byte[4096];
                    int size;
                    do
                    {
                        size = fs.Read(buffer, 0, buffer.Length);
                        zipStream.Write(buffer, 0, size);
                    } while (size > 0);
                }

                zipStream.IsStreamOwner = false; // False stops the Close also Closing the underlying stream.
                zipStream.Close();  // Must finish the ZipOutputStream before using outputMemStream.

                outputMemStream.Position = 0;

                return outputMemStream;
            }
            else
            {
                return data.Attachment.InputStream;
            }
        }

        public Stream Decompression(ObjectId Id, bool IsCompressed = true)
        {
            if (IsCompressed == true)
            {
                using (var fs = GetFile(Id))
                {
                    ZipInputStream zipInStream = new ZipInputStream(fs);
                    ZipEntry entry = zipInStream.GetNextEntry();
                    var fileStreamOut = new MemoryStream();
                    int size;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        size = zipInStream.Read(buffer, 0, buffer.Length);
                        fileStreamOut.Write(buffer, 0, size);
                    } while (size > 0);
                    zipInStream.Close();

                    fileStreamOut.Position = 0;

                    return fileStreamOut;           
                }
            }
            else
            {
                return GetFile(Id);
            }
        }

        public object AddFile(Archieve data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Attachment == null)
            {
                return null;
            }
            
            var fileInfo = _gridFs.Upload(Compression(data), data.Attachment.FileName);

            IMongoQuery query = Query.EQ("AutoNo", data.AutoNo);
            if (data.DocType.Length > 0)
            {
                data.DocType = data.DocType.Replace(",", " ");
            }
            if (data.Department.Length > 0)
            {
                data.Department = data.Department.Replace(",", " ");
            }

            var searchData = Settings.Concat(data.AutoNo, data.DocNo, data.ReffNo, data.Title, data.Description, data.Description2, data.Tag, data.DocType, data.Department);
            var hashKey = Settings.CalculateMD5(searchData);

            var item = _Document.Find(query).FirstOrDefault();

            if (item == null)
            {
                var doc = new Document();
                doc.AutoNo = data.AutoNo;
                doc.DocNo = data.DocNo;
                doc.DocType = data.DocType;
                doc.Department = data.Department;
                doc.ReffNo = hashKey;
                doc.Title = data.Title;
                doc.Description = data.Description;
                doc.Description2 = searchData;
                doc.Tag = data.Tag;
                doc.UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                doc.Private = data.Private;
                doc._id = ObjectId.GenerateNewId();
                _Document.Insert(doc);
            }
            else
            {
                if (item.ReffNo != hashKey)
                {
                    IMongoUpdate update = Update.Set("DocType", data.DocType ?? "")
                                            .Set("Department", data.Department ?? "")
                                            .Set("ReffNo", hashKey)
                                            .Set("Private", data.Private)
                                            .Set("DocNo", data.DocNo ?? "")
                                            .Set("Title", data.Title ?? "")
                                            .Set("Description", data.Description ?? "")
                                            .Set("Description2", searchData)
                                            .Set("UploadDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                            .Set("Tag", data.Tag);
                    _Document.Update(query, update);
                }
            }

            var fs = new FileSystems();
            fs.AutoNo = data.AutoNo;
            fs.FileName = data.Attachment.FileName;
            fs.Compress = NeedCompress(fs.FileName);
            fs.UploadBy = Settings.CurrentUser();
            fs.FileType = data.Attachment.ContentType;
            fs.UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            fs.Fileid = (ObjectId)fileInfo.Id;
            _Files.Insert(fs);

            var dataList = _Files.Find(query).Select(x => new { filename = x.FileName, fileid = x.Fileid }).ToList();

            return dataList;
        }

        public object GetList(string Id)
        {
            IMongoQuery query = Query.EQ("AutoNo", Id);
            var dataList = _Files.Find(query).Select(x => new { filename = x.FileName, fileid = x.Fileid }).ToList();

            return dataList;
        }

        public object GetGeneralList()
        {
            IMongoQuery query = Query.EQ("DocType", Settings.GetSetting("GENERAL_DOCID"));
            var docList =
                _Document.Find(query)
                    .SetSortOrder(SortBy.Descending("UploadDate"))
                    .SetLimit(8)
                    .Select(x => x.AutoNo)
                    .ToList();

            var fileList =
                _Files.Find(Query<FileSystems>.In(x => x.AutoNo, docList))
                    .SetSortOrder(SortBy.Descending("UploadDate"))
                    .SetLimit(8)
                    .Select(x => new {x.FileName, x.Fileid, x.AutoNo, x.UploadBy, x.UploadDate})
                    .ToList();

            return fileList;
        }

        public FileSystems GetFileInfo(string Id)
        {
            IMongoQuery query = Query.EQ("Fileid", ObjectId.Parse(Id));
            return _Files.FindOne(query);
        }

        public Stream GetFile(ObjectId id)
        {
            var file = _gridFs.FindOneById(id);
            return file.OpenRead();
        }

        public Stream GetFileStream(ObjectId id, bool IsCompressed = true)
        {
            return Decompression(id, IsCompressed);
        }

        public MemoryStream CreateThumbnail(Stream fromStream)
        {
            var image = Image.FromStream(fromStream);
            MemoryStream toStream = new MemoryStream();

            //double scaleFactor = 0.5;
            var newWidth = 240; //(int)(image.Width * scaleFactor);
            var newHeight = 200; //(int)(image.Height * scaleFactor);
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(toStream, image.RawFormat);

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();
            toStream.Position = 0;

            return toStream;
        }

        public MemoryStream CreateThumbnail2(Stream fromStream)
        {
            var image = Image.FromStream(fromStream);
            MemoryStream toStream = new MemoryStream();

            //double scaleFactor = 0.5;
            var newWidth = 600; //(int)(image.Width * scaleFactor);
            var newHeight = 400; //(int)(image.Height * scaleFactor);
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(toStream, image.RawFormat);

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();
            toStream.Position = 0;

            return toStream;
        }

        public Stream GetAllFilesByNo(string id)
        {
            IMongoQuery query = Query.EQ("AutoNo", id);
            var dataList = _Files.Find(query).Select(x => new { filename = x.FileName, fileid = x.Fileid , x.Compress}).ToList();

            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(5); //0-9, 9 being the highest level of compression
            foreach(var data in dataList)
            {
                var newEntry = new ZipEntry(data.filename);
                newEntry.DateTime = DateTime.Now;

                zipStream.PutNextEntry(newEntry);
                var fs = Decompression(data.fileid, data.Compress);
                byte[] buffer = new byte[4096];
                int size;
                do
                {
                    size = fs.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, size);
                } while (size > 0);
                zipStream.CloseEntry();
            }

            zipStream.IsStreamOwner = false; // False stops the Close also Closing the underlying stream.
            zipStream.Close();  // Must finish the ZipOutputStream before using outputMemStream.

            outputMemStream.Position = 0;

            return outputMemStream;
        }

        public void DeleteFile(string Id)
        {
            IMongoQuery query = Query.EQ("Fileid", ObjectId.Parse(Id));
            _Files.Remove(query);
            _gridFs.DeleteById(ObjectId.Parse(Id));
        }

        public FileSystems AddOrUpdateFiles(FileSystems data, Stream filedata)
        {
            IMongoQuery query1 = Query.EQ("FileName", data.FileName);
            IMongoQuery query2 = Query.EQ("Folder", data.Folder);
            IMongoQuery query3 = Query.EQ("FileType", data.FileType);

            IMongoQuery[] andQuery = { query1, query2, query3 };
            IMongoQuery query = Query.And(andQuery);

            var item1 = _Files.Find(query).FirstOrDefault();

            if (item1 == null)
            {
                data._id = ObjectId.GenerateNewId();
                _Files.Insert(data);
            }
            else
            {
                try
                {
                    _gridFs.DeleteById(item1.Fileid);
                } catch(Exception){}               
            }

            var fileInfo = _gridFs.Upload(filedata, data.Folder + "/" + data.FileName);

            if (fileInfo != null)
            {
                IMongoUpdate update = Update.Set("Fileid", (ObjectId)fileInfo.Id)
                    .Set("UploadBy", data.UploadBy)
                    .Set("UploadDate", data.UploadDate);

                _Files.Update(query, update);
            }

            return data;
        }

        public bool RemoveCodeEditor(string id)
        {
            IMongoQuery query = Query.EQ("_id", ObjectId.Parse(id));
            var result = _CodeEditor.Remove(query);
            return result.DocumentsAffected == 1;
        }

        public bool UpdateCodeEditor(string id, CodeEditor item)
        {
            IMongoQuery query = Query.EQ("_id", ObjectId.Parse(id));
            IMongoUpdate update = Update
                .Set("html", item.html)
                .Set("css", item.css)
                .Set("js", item.js)
                .Set("cfg", item.cfg);
            var result = _CodeEditor.Update(query, update);
            return result.UpdatedExisting;
        }
    }

    public class DocFile
    {
        public string Folder { get; set; }
        public string FileType { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }

    public class Archieve
    {
        public string AutoNo { get; set; }
        public string DocNo { get; set; }
        public string ReffNo { get; set; }
        public string Title { get; set; }
        public string DocType { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Tag { get; set; }
        public bool Private { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }

    public class Document
    {
        public ObjectId _id { get; set; }
        public string AutoNo { get; set; }
        public string DocNo { get; set; }
        public string DocType { get; set; }
        public string Department { get; set; }
        public string ReffNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Tag { get; set; }
        public string UploadDate { get; set; }
        public bool Private { get; set; }
    }

    //public class FSRepository
    //{
    //    public MongoDatabase _database;
    //    private readonly MongoGridFS _gridFs;
    //    public MongoCollection<FileSystems> _Files;
    //    public MongoCollection<MapDirectory> _Folders;

    //    public FSRepository()
    //        : this("")
    //    {
    //    }

    //    public FSRepository(string connection)
    //    {
    //        if (string.IsNullOrWhiteSpace(connection))
    //        {
    //            connection = Settings.MongoDbCDN;
    //        }

    //        var _databaseName = MongoUrl.Create(connection).DatabaseName;
    //        var client = new MongoClient(connection);
    //        var server = client.GetServer();

    //        _database = server.GetDatabase(_databaseName);
    //        _gridFs = _database.GridFS;
    //        _Files = _database.GetCollection<FileSystems>("files");
    //        _Folders = _database.GetCollection<MapDirectory>("folders");
    //    }
        
    //    public FileSystems GetFileInfo(string Id)
    //    {
    //        IMongoQuery query = Query.EQ("AutoNo", Id);
    //        return _Files.FindOne(query);
    //    }

    //    public Stream GetFile(ObjectId id)
    //    {
    //        var file = _gridFs.FindOneById(id);
    //        return file.OpenRead();
    //    }
        
    //    public void DeleteFile(string Id)
    //    {
    //        IMongoQuery query = Query.EQ("Fileid", ObjectId.Parse(Id));
    //        _Files.Remove(query);
    //        _gridFs.DeleteById(ObjectId.Parse(Id));
    //    }

    //    private void GenerateFolderStructure(string fullPath, string owner)
    //    {
    //        var Items = fullPath.Split('/');
    //        string parent = null;
    //        string name = owner;

    //        foreach (var item in Items)
    //        {
    //            parent = name;
    //            name += "/" + item;
    //            IMongoQuery query1 = Query.EQ("FullPath", name);
    //            IMongoQuery query2 = Query.EQ("Parent", parent);
    //            IMongoQuery query3 = Query.EQ("Owner", owner);
    //            IMongoQuery[] andQuery = { query1, query2, query3 };
    //            IMongoQuery query = Query.And(andQuery);
    //            var folder = _Folders.Find(query).FirstOrDefault();
    //            if (folder == null)
    //            {
    //                var data = new MapDirectory();
    //                data._id = ObjectId.GenerateNewId();
    //                data.Caption = item;
    //                data.Parent = parent;
    //                data.Owner = owner;
    //                data.FullPath = name;
    //                _Folders.Insert(data);
    //            }
    //        }
    //    }

    //    public FileSystems AddOrUpdateFiles(FileSystems data, Stream filedata, string filetype)
    //    {
    //        IMongoQuery query1 = Query.EQ("FileName", data.FileName);
    //        IMongoQuery query2 = Query.EQ("Folder", data.Folder);
    //        IMongoQuery query3 = Query.EQ("FileType", data.FileType);

    //        IMongoQuery[] andQuery = { query1, query2, query3 };
    //        IMongoQuery query = Query.And(andQuery);

    //        var item1 = _Files.Find(query).FirstOrDefault();

    //        if (item1 == null)
    //        {
    //            data._id = ObjectId.GenerateNewId();

    //            _Files.Insert(data);
    //        }
    //        else
    //        {
    //            try
    //            {
    //                _gridFs.DeleteById(item1.Fileid);
    //            }
    //            catch (Exception) { }
    //        }

    //        var fileInfo = _gridFs.Upload(filedata, data.AutoNo);
    //        GenerateFolderStructure(data.Folder, data.FileType);

    //        if (fileInfo != null)
    //        {
    //            IMongoUpdate update = Update.Set("Fileid", (ObjectId)fileInfo.Id)
    //                .Set("FileType", filetype)
    //                .Set("UploadBy", data.UploadBy)
    //                .Set("UploadDate", data.UploadDate);

    //            _Files.Update(query, update);
    //        }

    //        return data;
    //    }
    //}
}