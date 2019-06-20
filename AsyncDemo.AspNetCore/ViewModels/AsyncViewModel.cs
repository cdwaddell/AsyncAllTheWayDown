using System.Collections.Concurrent;

namespace AsyncDemo.AspNetCore.ViewModels
{
    public class AsyncViewModel
    {
        public ConcurrentBag<EventTagViewModel> Bag { get; set; } = new ConcurrentBag<EventTagViewModel>();
    }
}