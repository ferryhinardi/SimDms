namespace SimDms.Service.Controllers
{
    public class DashboardController : BaseController
    {
        public string RoomQueue()
        {
            return HtmlRender("dashboard/roomqueue.js");
        }

        public string RoomSA()
        {
            return HtmlRender("dashboard/roomsa.js");
        }
    }
}