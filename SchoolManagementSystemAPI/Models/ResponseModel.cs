using Newtonsoft.Json;

namespace SchoolManagementSystemAPI.Models
{
    public class ResponseModel
    {

        public object data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
