namespace FleetManagementSystem_.Net.Models
{
    public class ConfirmPromptModel
    {
        public string Message { get; set; } = "Are you sure?";
        public string Url { get; set; } = "#";
        public string Method { get; set; } = "POST"; // GET or POST
        public string ButtonText { get; set; } = "Delete";
    }
}
