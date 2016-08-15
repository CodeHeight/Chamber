namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class AdminGenericMessageViewModel
    {
        public AdminGenericMessageViewModel()
        {
            MessageType = GenericMessages.info;
        }
        public string Message { get; set; }
        public GenericMessages MessageType { get; set; }
        public bool ConstantMessage { get; set; }
    }

    public enum GenericMessages
    {
        warning,
        danger,
        success,
        info
    }
}