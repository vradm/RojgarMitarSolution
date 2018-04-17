using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RojgarmitraSolution.Models;
using Facebook;
using Newtonsoft.Json;
using System.IO;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using System.Net;
using BusinessModel;
using RojgarmitraSolution.Security;
using System.Configuration;
using System.Net.Http;
using System.Text;
using SimpleJson;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace RojgarmitraSolution.Controllers
{
    public class UserRegistrationController : Controller
    {
        string BaseUrl = ConfigurationManager.AppSettings["baseUrl"].ToString();
        //static readonly i  _repository = new CompaniesOtherMasterRepository(new DayBrokersEntities());
        // GET: UserRegistrations
        public async Task<ActionResult> PersonalDetails()
        {
            List<OtherMasterModel> OtherMasterModel = await GetOthermaster();
            Session["OtherMasterModel"] = OtherMasterModel;
            UserRegistartionPersonalDetailsModel model = new UserRegistartionPersonalDetailsModel();
            model.ListOfExper = GetOthermaster("EXPERINCE", OtherMasterModel);
            return View(model);
        }
        [HttpPost]
        public async Task<JsonResult> SavePersonalDetails()
        {
            ResponseModel responseModel = new ResponseModel();
            UserRegistartionPersonalDetailsModel Model = new UserRegistartionPersonalDetailsModel();
            if (ModelState.IsValid)
            {

                Model.FullName = Request.Form["FullName"];
                Model.EmailID = Request.Form["EmailID"];
                Model.Password = Request.Form["Password"];
                Model.MobileNumber = Request.Form["MobileNumber"];
                Model.TotalExYear = Convert.ToInt32(Request.Form["TotalExYear"]);
                Model.Gender = Request.Form["radioValue"];
                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    string Key = files.AllKeys[i].ToString();
                    HttpPostedFileBase file = files[i];
                    string fname;
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    Model.Resume = fname;
                    string Extension = System.IO.Path.GetExtension(file.FileName);
                    Model.Extension = Extension;
                }
                string url = BaseUrl + "Account/SavePersonalDetails";
                using (var httpclient = new HttpClient())
                {
                    HttpResponseMessage ResponseMessage = await httpclient.PostAsJsonAsync(url, Model);
                    if (ResponseMessage.IsSuccessStatusCode)
                    {
                        var response = ResponseMessage.Content.ReadAsStringAsync().Result;
                        responseModel = JsonConvert.DeserializeObject<ResponseModel>(response);
                        if (responseModel.status == true)
                        {
                            TempData["Message"] = "You have successfully created your profile with us";
                            Session["userId"] = (responseModel.id);
                            object saveFile = new object();
                            saveFile = responseModel.data;

                            try
                            {
                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                    string Extension = System.IO.Path.GetExtension(file.FileName);

                                    if (Model.Resume != null && Model.Resume != "")
                                    {
                                                                               
                                        var filePath = Server.MapPath("~/Content/Resume/" + saveFile.ToString());
                                        file.SaveAs(filePath);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                TempData["Message"] = ex.Message;
                                //return RedirectToAction("PersonalDetails");
                            }
                        }
                    }
                    return Json(responseModel);
                }

            }
            return Json(responseModel);

        }

        public async Task<List<OtherMasterModel>> GetOthermaster()
        {
            string url = BaseUrl + "Master/OthermasterList";
            List<OtherMasterModel> OtherMasterModel = new List<OtherMasterModel>();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    OtherMasterModel = JsonConvert.DeserializeObject<List<OtherMasterModel>>(result);
                }
                return OtherMasterModel;


            }
        }
        public async Task<List<SelectListItem>> DesignationList()
        {
            string url = BaseUrl + "Master/DesignationList";
            List<SelectListItem> DesignationList = new List<SelectListItem>();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    var Result = JsonConvert.DeserializeObject<List<DesignationModel>>(result);
                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            DesignationList.Add(new SelectListItem { Text = item.DesignationName, Value = item.DesignationID.ToString() });
                        }

                    }
                    DesignationList.Insert(0, new SelectListItem { Text = "-Select Designation-", Value = "0" });
                }
                return DesignationList;
            }
        }
        public async Task<List<SelectListItem>> CompanyList()
        {
            string url = BaseUrl + "Master/CompanyList";
            List<SelectListItem> listofCompany = new List<SelectListItem>();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    var Result = JsonConvert.DeserializeObject<List<CompanyModel>>(result);
                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            listofCompany.Add(new SelectListItem { Text = item.CompanyName, Value = item.CompanyID.ToString() });
                        }

                    }
                    listofCompany.Insert(0, new SelectListItem { Text = "-Select company-", Value = "0" });
                }
                return listofCompany;
            }
        }
        public async Task<List<SelectListItem>> StateList()
        {
            string url = BaseUrl + "Master/StateList";
            List<SelectListItem> StateList = new List<SelectListItem>();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    var Result = JsonConvert.DeserializeObject<List<StateModel>>(result);
                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            StateList.Add(new SelectListItem { Text = item.Statename, Value = item.StateID.ToString() });
                        }

                    }
                    StateList.Insert(0, new SelectListItem { Text = "-Select City-", Value = "0" });
                }
                return StateList;
            }
        }
        public async Task<List<SelectListItem>> University_CollegeList()
        {
            string url = BaseUrl + "Master/University_CollegeList";
            List<SelectListItem> University_CollegeList = new List<SelectListItem>();
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    var Result = JsonConvert.DeserializeObject<List<University_CollegeModel>>(result);
                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            University_CollegeList.Add(new SelectListItem { Text = item.University_CollegeName, Value = item.ID.ToString() });
                        }

                    }
                    University_CollegeList.Insert(0, new SelectListItem { Text = "-Select-", Value = "0" });
                }
                return University_CollegeList;
            }
        }
        private SelectList GetOthermaster(string MasterType, List<OtherMasterModel> ListOtherMaster)
        {

            var olist = ListOtherMaster.Where(x => x.MasterType.ToUpper() == MasterType.ToUpper()).ToList();
            return new SelectList(olist, "Code", "Name");
        }

        [HttpGet]
        public async Task<ActionResult> EmploymentDetails()
        {
            UserRegistrationEmployeMentModel model = new UserRegistrationEmployeMentModel();
            model.UserID = Convert.ToInt32(Session["userId"]);


            List<OtherMasterModel> otherMasterModels = new List<OtherMasterModel>();
            if (Session["OtherMasterModel"] != null)
            {
                otherMasterModels = (List<OtherMasterModel>)Session["OtherMasterModel"];
            }
            else
            {
                otherMasterModels = await GetOthermaster();
            }

            model.ListAnnualSalaryInLakh = GetOthermaster("ANNUAL SALARY IN LAKH", otherMasterModels);
            model.ListAnnualSalaryInThousand = GetOthermaster("ANNUAL SALARY IN THOUSAND", otherMasterModels);
            model.ListWorkingSince = GetOthermaster("WORKING", otherMasterModels);
            model.ListWorkingMonth = GetOthermaster("MONTH", otherMasterModels);
            model.CompanyList = await CompanyList();
            model.DesignationList = await DesignationList();
            model.CityList = await StateList();
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> EmploymentDetails(UserRegistrationEmployeMentModel model)
        {
            //if (ModelState.IsValid)
            //{
            ResponseModel responseModel = new ResponseModel();
            string url = BaseUrl + "Account/SaveEmploymentDetails";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, model);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = responseMessage.Content.ReadAsStringAsync().Result;
                    responseModel = JsonConvert.DeserializeObject<ResponseModel>(result);
                    if (responseModel.status == true)
                    {
                        Session["userId"] = (responseModel.id);
                        TempData["Message"] = "You have successfully created your profile with us";
                        return RedirectToAction("EducationDetails");
                    }
                    else
                    {
                        TempData["Message"] = "Error  in saving Record";
                    }
                }
            }
            
            return RedirectToAction("EmploymentDetails");
        }

        [HttpGet]
        public async Task<ActionResult> EducationDetails()
        {
            UserRegistrationEducationModel model = new UserRegistrationEducationModel();
            model.UserId = Convert.ToInt32(Session["userId"]);
            List<OtherMasterModel> otherMasterModels = new List<OtherMasterModel>();
            if (Session["OtherMasterModel"] != null)
            {
                otherMasterModels = (List<OtherMasterModel>)Session["OtherMasterModel"];
            }
            else
            {
                otherMasterModels = await GetOthermaster();
            }
            model.ListSpecialization = GetOthermaster("SPECIALIZATION", otherMasterModels);
            model.ListCourse = GetOthermaster("COURSE", otherMasterModels);
            model.ListHighestQulaification = GetOthermaster("QUALIFICATION", otherMasterModels);
            model.UniversityList = await University_CollegeList();
            return View(model);
        }

        public async Task<JsonResult> SaveEducationDetails()
        {
            UserRegistrationEducationModel Model = new UserRegistrationEducationModel();
            ResponseModel responseModel = new ResponseModel();
            Model.UserId = Convert.ToInt32(Session["userId"]);
            Model.ProfessionalExperience = Request.Form["ProfessionalExperience"];
            Model.HighestQualification = Convert.ToInt32(Request.Form["HighestQualification"]);
            Model.Course = Convert.ToInt32(Request.Form["Course"]);
            Model.Specialization = Convert.ToInt32(Request.Form["Specialization"]);
            Model.University_College = Convert.ToInt32(Request.Form["University_College"]);
            Model.CourseType = Request.Form["CourseType"];
            Model.PassingYear = Convert.ToInt32(Request.Form["PassingYear"]);
            HttpFileCollectionBase files = Request.Files;
            string fname = "";
            for (int i = 0; i < files.Count; i++)
            {
                string Key = files.AllKeys[i].ToString();
                HttpPostedFileBase file = files[i];
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }
            }
            Model.ProfileImage = fname;
            string url = BaseUrl + "Account/SaveEducationDetails";
            using (var httpclient = new HttpClient())
            {
                HttpResponseMessage ResponseMessage = await httpclient.PostAsJsonAsync(url, Model);
                if (ResponseMessage.IsSuccessStatusCode)
                {
                    var response = ResponseMessage.Content.ReadAsStringAsync().Result;
                    responseModel = JsonConvert.DeserializeObject<ResponseModel>(response);
                    TempData["Message"] = responseModel.message;
                    Session["userId"] = (responseModel.id);
                    object saveFile = new object();
                    saveFile = responseModel.data;
                    try
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string Extension = System.IO.Path.GetExtension(file.FileName);

                            if (Model.ProfileImage != null && Model.ProfileImage != "")
                            {

                                //var fileName = String.Format("{0}_{1}{2}", response, "Policy", Extension);
                                var filePath = Server.MapPath("~/Content/ProfileImage/" + saveFile.ToString());
                                file.SaveAs(filePath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Json(true);
            }
        }
        public ActionResult UserProfile()
        {
            AccountModel model = new AccountModel();
            return View(model);
        }
        // GET: UserRegistration/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: UserRegistration/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: UserRegistration/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRegistration/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserRegistration/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRegistration/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRegistration/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
