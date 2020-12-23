namespace Imgeneus.Login
{
    public interface ILoginManagerFactory
    {
        /// <summary>
        /// Creates login manager.
        /// </summary>
        public LoginManager CreateLoginManager(LoginClient client, ILoginServer server);
    }
}
