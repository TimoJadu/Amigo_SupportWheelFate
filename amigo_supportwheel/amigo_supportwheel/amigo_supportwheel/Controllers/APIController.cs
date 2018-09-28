using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Cors;
using amigo_supportwheel.BusinessRules;
using Newtonsoft.Json;


namespace amigo_supportwheel.Controllers
{
    [EnableCors("*", "*", "*")]
    [System.Web.Http.AllowAnonymous]
    public class APIController : ApiController
    {
        // GET: API
        //public ActionResult Index()
        //{
        //    return View();
        //}

        string[] engineersArr = new string[] { "Tom", "Mera", "Merry", "Susain", "Jerry", "Daniel", "Daren", "Danise", "Donald", "Peter" };
        Wrapper inst = new Wrapper();
        BuildDataSet v;

        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("Validate")]
        [EnableCors("*", "*", "*")]
        public Wrapper Validate(string date, string employeeName, string shiftType)
        {
            System.Diagnostics.Debugger.Break();
            v = BuildDataSet.GetInstance;
            //v.formDataSet(Convert.ToDateTime(date), engineersArr[0], "Half");
            //v.formDataSet(Convert.ToDateTime(date), engineersArr[0], "Half");
            v.formDataSet(DateTime.ParseExact(date, "MM/dd/yyyy", null), employeeName, shiftType);

            inst.message = v.errorMessage;
            if (inst.message == string.Empty)
            {
                inst.status = true;
                inst.message = "Duty confirmed";
            }

            return inst;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("DeleteInstance")]
        public void DeleteInstance()
        {
            BuildDataSet.GetInstance.dutyList = new Dictionary<DateTime, List<string[]>>();
        }

        [Serializable]
        [JsonObject]
        public class Wrapper
        {
            public bool status { get; set; }
            public string message { get; set; }
        }
    }
}