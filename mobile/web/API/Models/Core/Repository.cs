using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace eXpressAPI.Models
{

    public class SaveResult
    {
        public SaveResult(int a,bool b)
        {
            Count = a;
            success = b;
        }

        public int Count { get; set; }
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


        public DataSet Tables()
        {
            if (data != null)
            {
                return ((DataSet)data);
            }
            return null;
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

    public class RepositoryExp<T> : eXpressRepository<T> where T : class
    {
        private readonly DataAccessContext _dbContext;

        public RepositoryExp(DataAccessContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SaveResult Add(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Set<T>().Add(t);

            try
            {
                ret.Count =  _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> AddAsync(T t)
        {
            SaveResult ret = new SaveResult(0,false);

            _dbContext.Set<T>().Add(t);

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message  = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> RemoveAsync(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Deleted;

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbUpdateException ex)
            {
                ret.message = ex.InnerException.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public SaveResult Remove(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Deleted;

            try
            {
                ret.Count = _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbUpdateException ex)
            {
                ret.message = ex.InnerException.Message;
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public  SaveResult Update(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Modified;

            try
            {
                ret.Count =  _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> UpdateAsync(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Modified;

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(match);
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _dbContext.Set<T>().Where(match).ToListAsync();
        }
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
 
        //MongoServer _server;
        MongoClient _client;
        MongoDatabase _database;
        MongoCollection<CodeEditor> _CodeEditor;

        public CodeEditorRepository()
            : this("")
        {
        }

        public CodeEditorRepository(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                connection = ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString;
            }
            var _databaseName = MongoUrl.Create(connection).DatabaseName;
            var client = new MongoClient(connection);
            var server = client.GetServer();
            var _database = server.GetDatabase(_databaseName);
            _CodeEditor = _database.GetCollection<CodeEditor>("CodeEditor");
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


        public CodeEditor AddOrUpdateCodeEditor(CodeEditor item)
        {
            IMongoQuery query = Query.EQ("filename", item.filename);
            var item1 =  _CodeEditor.Find(query).FirstOrDefault();
            if (item1 == null)
            {
                item._id = ObjectId.GenerateNewId();
                _CodeEditor.Insert(item);
            }
            else
            {
                string cfg = item.cfg ?? "";
                string css = item.css  ?? "";
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

        public bool RemoveCodeEditor(string id)
        {
            IMongoQuery query = Query.EQ("_id", id);
            var result = _CodeEditor.Remove(query);
            return result.DocumentsAffected == 1;
        }

        public bool UpdateCodeEditor(string id, CodeEditor item)
        {
            IMongoQuery query = Query.EQ("_id", id);
            IMongoUpdate update = Update
                .Set("html", item.html)
                .Set("css", item.css)
                .Set("js", item.js)
                .Set("cfg", item.cfg);
            var result = _CodeEditor.Update(query, update);
            return result.UpdatedExisting;
        }
    }

}