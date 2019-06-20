using System.ComponentModel;

namespace AsyncDemo.AspNetCore.ViewModels
{
    public class ThreadsViewModel
    {
        [DisplayName("Available Threads")]
        public int AvailableThreads { get; set; }
        [DisplayName("Available Threads")]
        public int CompletionPortThreads { get; set; }
        [DisplayName("Min Threads")]
        public int MinThreads { get; set; }
        [DisplayName("Min Threads")]
        public int MinCompletionPortThreads { get; set; }
        [DisplayName("Max Threads")]
        public int MaxCompletionPortThreads { get; set; }
        [DisplayName("Max Threads")]
        public int MaxWorkerThreads { get; set; }
    }
}