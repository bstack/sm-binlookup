using System;


namespace binlookup.External
{
    public class LogActivityRequest
    {
        public string Service;
        public string Activity;
        public string ActivityDetail;


        public LogActivityRequest()
        {
        }


        public LogActivityRequest(
            string activity,
            string activityDetail)
        {
            this.Service = "binlookup";
            this.Activity = activity;
            this.ActivityDetail = activityDetail;
        } 
    }
}
