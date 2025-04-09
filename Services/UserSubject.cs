using MvcMovie.Models;

namespace MvcMovie.Observer
{
    public class UserSubject
    {
        private readonly List<IUserObserver> _observers = new();

        public void Attach(IUserObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IUserObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string action, User user)
        {
            foreach (var observer in _observers)
            {
                observer.OnUserChanged(action, user);
            }
        }
    }
}
