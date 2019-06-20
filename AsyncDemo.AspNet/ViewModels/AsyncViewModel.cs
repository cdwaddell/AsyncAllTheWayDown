using System.Collections.Concurrent;

namespace AsyncDemo.AspNet.ViewModels
{
    public class AsyncViewModel
    {
        public ConcurrentBag<EventTagViewModel> Bag { get; set; } = new ConcurrentBag<EventTagViewModel>();
    }
}