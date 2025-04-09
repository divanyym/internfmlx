using MvcMovie.Models;

namespace MvcMovie.Observer
{
    public class UserChangeNotifier
    {
        private readonly List<IUserObserver> _observers = new();

        public void RegisterObserver(IUserObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void UnregisterObserver(IUserObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(string action, User user)
        {
            foreach (var observer in _observers)
            {
                observer.OnUserChanged(action, user);
            }
        }
    }
}
