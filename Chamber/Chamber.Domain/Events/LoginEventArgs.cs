using Chamber.Domain.Interfaces.UnitOfWork;

namespace Chamber.Domain.Events
{
    public class LoginEventArgs : ChamberEventArgs
    {
        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
    }
}
