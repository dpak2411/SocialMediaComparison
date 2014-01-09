using System;
using System.Collections.Generic;
using System.Linq;
using AlteryxGalleryAPIWrapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Fizzler.Systems.HtmlAgilityPack;

namespace SocialMediaComparison
{
    [Binding]
    public class SocialMediaComparisionSteps
    {
        private string alteryxurl;
        private string _sessionid;
        private string _appid;
        private string _userid;
        private string _appName;
        private string jobid;
        private string outputid;
        private string validationId;
        private string _appActualName;
        private dynamic statusresp;
        private int messagecount;
        private string twitterhandle;
        

        private Client Obj = new Client("https://gallery.alteryx.com/api/");

        private RootObject jsString = new RootObject();

        [Given(@"alteryx running at""(.*)""")]
        public void GivenAlteryxRunningAt(string SUT_url)
        {
            alteryxurl = Environment.GetEnvironmentVariable(SUT_url);
        }
        
        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {
            _sessionid = Obj.Authenticate(user, password).sessionId;
        }

        [When(@"I run the app ""(.*)"" that searches the twitter handle ""(.*)""")]
        public void WhenIRunTheAppThatSearchesTheTwitterHandle(string app, string handle)
        {
            twitterhandle = handle;
            //url + "/apps/gallery/?search=" + appName + "&limit=20&offset=0"
            //Search for App & Get AppId & userId 
            string response = Obj.SearchAppsGallery(app);
            var appresponse =
                new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                    response);
            int count = appresponse["recordCount"];
            if (count == 1)
            {
                _appid = appresponse["records"][0]["id"];
                _userid = appresponse["records"][0]["owner"]["id"];
                _appName = appresponse["records"][0]["primaryApplication"]["fileName"];
            }
            else
            {
                for (int i = 0; i <= count - 1; i++)
                {

                    _appActualName = appresponse["records"][i]["primaryApplication"]["metaInfo"]["name"];
                    if (_appActualName == app)
                    {
                        _appid = appresponse["records"][i]["id"];
                        _userid = appresponse["records"][i]["owner"]["id"];
                        _appName = appresponse["records"][i]["primaryApplication"]["fileName"];
                        break;
                    }
                }

            }
            jsString.appPackage.id = _appid;
            jsString.userId = _userid;
            jsString.appName = _appName;

            //url +"/apps/" + appPackageId + "/interface/
            //Get the app interface - not required
            string appinterface = Obj.GetAppInterface(_appid);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);
        }
        
        [When(@"I pass the first location details ""(.*)"",""""(.*)"""",""(.*)"",""(.*)"",""(.*)""")]
        public void WhenIPassTheFirstLocationDetails(string locationname, string address, string city, string state, string zipcode)
        {
            
            List<JsonPayload.Question> questionAnsls1 = new List<JsonPayload.Question>();
            questionAnsls1.Add(new JsonPayload.Question("Site 1 name", "\""+locationname+"\""));
            questionAnsls1.Add(new JsonPayload.Question("Site 1 Address", "\"" + address + "\""));
            questionAnsls1.Add(new JsonPayload.Question("Site 1 city", "\"" + city + "\""));
            questionAnsls1.Add(new JsonPayload.Question("Site 1 State", "\"" + state + "\""));
            questionAnsls1.Add(new JsonPayload.Question("Site 1 ZIP",  zipcode ));
            questionAnsls1.Add(new JsonPayload.Question("Site 1 Twitter Key Words", "\"" + twitterhandle + "\""));

            jsString.questions.AddRange(questionAnsls1);

        }
        
        [When(@"I also pass the second location details ""(.*)"",""""(.*)"""",""(.*)"",""(.*)"",""(.*)""")]
        public void WhenIAlsoPassTheSecondLocationDetails(string locationname, string address, string city, string state, string zipcode)
        {
            List<JsonPayload.Question> questionAnsls2 = new List<JsonPayload.Question>();
            questionAnsls2.Add(new JsonPayload.Question("Site 2 name","\""+ locationname+"\""));
            questionAnsls2.Add(new JsonPayload.Question("Site 2 Address", "\""+address+"\""));
            questionAnsls2.Add(new JsonPayload.Question("Site 2 city", "\""+city+"\""));
            questionAnsls2.Add(new JsonPayload.Question("Site 2 State", "\""+state+"\""));
            questionAnsls2.Add(new JsonPayload.Question("Site 2 ZIP", zipcode));
            questionAnsls2.Add(new JsonPayload.Question("Site 2 Twitter Key Words", "\""+twitterhandle+"\""));
            
            jsString.questions.AddRange(questionAnsls2);
        }
        
        [When(@"I choose the radius and the size of area to study ""(.*)"" in miles")]
        public void WhenIChooseTheRadiusAndTheSizeOfAreaToStudyInMiles(int SA)
        {

            //Construct the payload to be posted.
            List<JsonPayload.Question> questionAnsls3 = new List<JsonPayload.Question>();
            questionAnsls3.Add(new JsonPayload.Question("Radius Selected", "true"));
            questionAnsls3.Add(new JsonPayload.Question("Drive Time Selection", "false"));
            questionAnsls3.Add(new JsonPayload.Question("Trade Area Size", "\""+SA.ToString()+"\""));

            jsString.questions.AddRange(questionAnsls3);
            jsString.jobName = "Job Name";

            // Make Call to run app

            var postData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsString);
            string postdata = postData.ToString();
            string resjobqueue = Obj.QueueJob(postdata);

            var jobqueue =
                new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                    resjobqueue);
            jobid = jobqueue["id"];

             string status = "";
            while (status != "Completed")
            {
                string jobstatusresp = Obj.GetJobStatus(jobid);
                statusresp =
                    new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                        jobstatusresp);
                status = statusresp["status"];
                messagecount = statusresp["messages"].Count;
            }
        }

        [Then(@"I see the output has the report ""(.*)""")]
        public void ThenISeeTheOutputHasTheReport(string output)
        {

            //url + "/apps/jobs/" + jobId + "/output/"
            string getmetadata = Obj.GetOutputMetadata(jobid);
            dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);

            // outputid = metadataresp[0]["id"];
            int count = metadataresp.Count;
            for (int j = 0; j <= count - 1; j++)
            {
                outputid = metadataresp[j]["id"];
            }

            string getjoboutput = Obj.GetJobOutput(jobid, outputid, "html");
            string htmlresponse = getjoboutput;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlresponse);
            string response = doc.DocumentNode.SelectSingleNode("//div[@class='DefaultText']").InnerText;  
            StringAssert.Contains(output,response);
        }
        #region
        //[Then(@"I see the SentimentScore more than (.*) for screenname ""(.*)""")]
        //public void ThenISeeTheSentimentScoreMoreThanForScreenname(int sentiscore, string screenname)
        //{
        //    //url + "/apps/jobs/" + jobId + "/output/"
        //    string getmetadata = Obj.GetOutputMetadata(jobid);
        //    dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);

        //    // outputid = metadataresp[0]["id"];
        //    int count = metadataresp.Count;
        //    for (int j = 0; j <= count - 1; j++)
        //    {
        //        outputid = metadataresp[j]["id"];
        //    }

        //    string getjoboutput = Obj.GetJobOutput(jobid, outputid, "html");
        //    string htmlresponse = getjoboutput;
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(htmlresponse);
        //    doc.DocumentNode.QuerySelector("\td class=\"column0\"");
            
        //  //string output = doc.DocumentNode.SelectSingleNode("//datatable/dbody/dr/cell[@class='column0']").InnerHtml;  
        //   // var output = doc.DocumentNode.ChildNodes.Descendants("srcreport");
        //   // var count1 = doc.DocumentNode.ChildNodes.GetNodeIndex(HtmlNode node);
        //    //for (int i = 0; i < count1; i++)
        //    //{
                
        //    //}

        //    var table = doc.DocumentNode.Descendants("table");

        //}
        #endregion
    }
}
