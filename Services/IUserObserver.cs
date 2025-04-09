namespace MvcMovie.Observer
{
    public interface IUserObserver
    {
        void OnUserChanged(string action, MvcMovie.Models.User user);
    }
}
