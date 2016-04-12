using eXpressAPI.Models;
using MongoDB.Driver.Builders;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eXpressAPP.Controllers.Sys
{
    [Authorize]
    public class AppEngineController : Controller
    {

        CodeEditorRepository _codeRepo = new CodeEditorRepository();


        [HttpGet]
        public async Task<string> getfile(string name, string type)
        {
            CodeEditor ce = _codeRepo.GetByFileName(name);
            string content = "";

            try
            {
                if (type == "html")
                {
                    if (ce != null)
                    {
                        content = ce.html;
                    }
                } else if (type=="htmlx")
                {
                    if ( ce != null)
                    {
                        content = ce.html;
                    }
                    ce = _codeRepo.GetByFileName("ribbon");
                    content = ce.html + content;                     
                } else if (type=="css")
                {
                    if (ce != null)
                    {
                        content = ce.css;
                    }
                } else
                {
                    if (ce != null)
                    {
                        content = ce.cfg + ce.js;
                    }
                }
            }
            catch (Exception ex)
            {
                content = ex.Message;
            }

            return content;
        }
                
        [HttpGet]
        public  JsonResult  LoadContent(string filename)
        {
            CodeEditor ce = null;
            string result = "success";
            bool success = false;
            try
            {
                ce = _codeRepo.GetByFileName(filename);
                success = true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Json(new { success = success, data = ce, message = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> create(string filename)
        {
            CodeEditor ce = null;
            string result = "success";
            bool success = false;

            CreateDll(filename);

            return Json(new { success = success,  message = result }, JsonRequestBehavior.AllowGet);
        }
        
        public Assembly CreateDll(string _controllerName)
        {
            IDictionary<string, string> compParams =
                 new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp", compParams);

            string path = Settings.ReffDir;
            string outputDll = path + _controllerName + ".dll";

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = outputDll;
            
            parameters.ReferencedAssemblies.Add(path + @"System.Net.Http.dll");
            parameters.ReferencedAssemblies.Add(path + @"System.Net.Http.WebRequest.dll");
            parameters.ReferencedAssemblies.Add(path + @"System.Net.Http.Formatting.dll");
            parameters.ReferencedAssemblies.Add(path + @"System.Web.Http.dll");
            string code = new StringBuilder()
                .AppendLine("using System.Web.Http;")
                .AppendLine("namespace ControllerLibrary")
                .AppendLine("{")
                .AppendLine(string.Format("public class {0} : ApiController", _controllerName))
                .AppendLine(" {")
                .AppendLine("  public string Get()")
                .AppendLine("  {")
                .AppendLine(string.Format("return \"Hi from a Dynamic controller library- {0} !\";", _controllerName))
                .AppendLine("  }")
                .AppendLine(" }")
                .AppendLine("}")
                .ToString();
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.Count > 0)
            {
                Console.WriteLine("Build Failed");
                foreach (CompilerError CompErr in results.Errors)
                {
                    Console.WriteLine(
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine);
                }
            }
            else
            {
                //Console.WriteLine("Build Succeeded");
                return Assembly.LoadFrom(outputDll);
            }
            return null;
        }



    }
    
}