using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncDemo.AspNetCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace AsyncDemo.AspNetCore.Controllers
{
    public class HomeController : Controller
    {        
        public IActionResult Index()
        {
            ThreadPool.GetAvailableThreads(
                out var workerThreads,
                out var completionPortThreads
            );

            ThreadPool.GetMinThreads(
                out var minWorkerThreads,
                out var minCompletionPortThreads
            );

            ThreadPool.GetMaxThreads(
                out var maxWorkerThreads,
                out var maxCompletionPortThreads
            );

            return View(new ThreadsViewModel
            {
                AvailableThreads = workerThreads,
                CompletionPortThreads = completionPortThreads,
                MinThreads = minWorkerThreads,
                MinCompletionPortThreads = minCompletionPortThreads,
                MaxWorkerThreads = maxWorkerThreads,
                MaxCompletionPortThreads = maxCompletionPortThreads
            });
        }
        public async Task<IActionResult> TestSingleThreaded()
        {
            var events = new AsyncViewModel();

            var event1 = DoEventNotAsync(events, "Event 1", 3);
            var event2 = DoEventNotAsync(events, "Event 2", 1);
            var event3 = DoEventNotAsync(events, "Event 3", 2);

            await Task.WhenAll(event1, event2, event3);

            return View("TestMultiThreaded", events);
        }

        public async Task<ActionResult> TestMultiThreadedOrder()
        {
            var events = new AsyncViewModel();

            await DoEventAsync(events, "Event 1");
            await DoEventAsync(events, "Event 2");
            await DoEventAsync(events, "Event 3");

            return View("TestMultiThreaded", events);
        }

        public async Task<IActionResult> TestMultiThreaded()
        {
            var events = new AsyncViewModel();

            var event1 = DoEventAsync(events, "Event 1");
            var event2 = DoEventAsync(events, "Event 2");
            var event3 = DoEventAsync(events, "Event 3");

            await Task.WhenAll(event1, event2, event3);

            return View("TestMultiThreaded", events);
        }
        public IActionResult DeadlockDemo()
        {
            var result = GetTimeZonesBadAsync(new Uri("http://worldtimeapi.org/api/timezone"));
            var strings = result.Result;
            
            strings.Insert(0, HttpContext?.Request.GetDisplayUrl());
            return View("DeadlockDemo", strings);
        }

        public IActionResult NoDeadlockDemo()
        {
            var strings = GetTimeZonesGoodAsync(new Uri("http://worldtimeapi.org/api/timezone"))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            strings.Insert(0, HttpContext?.Request.GetDisplayUrl());
            return View("DeadlockDemo", strings);
        }

        public async Task<IActionResult> LostContextDemo()
        {
            var strings = await GetTimeZonesBadAsync(new Uri("http://worldtimeapi.org/api/timezone"))
                .ConfigureAwait(false);

            strings.Insert(0, HttpContext?.Request.GetDisplayUrl());
            return View("DeadlockDemo", strings);
        }

        private static async Task<List<string>> GetTimeZonesBadAsync(Uri uri)
        {
            // (real-world code shouldn't use HttpClient in a using block; this is just example code)
            using (var client = new HttpClient())
            {
                var jsonString = await client.GetStringAsync(uri);
                var strings = JsonConvert.DeserializeObject<List<string>>(jsonString);
                return strings;
            }
        }

        private static async Task<List<string>> GetTimeZonesGoodAsync(Uri uri)
        {
            // (real-world code shouldn't use HttpClient in a using block; this is just example code)
            using (var client = new HttpClient())
            {
                var jsonString = 
                    await client.GetStringAsync(uri)
                        .ConfigureAwait(false)
                    ;
                var strings = JsonConvert.DeserializeObject<List<string>>(jsonString);
                return strings;
            }
        }

        private async Task DoEventAsync(AsyncViewModel events, string eventName)
        {
            await Task.Run(() =>
            {
                DoEvent(events, eventName);
            });
        }

        private async Task DoEventNotAsync(AsyncViewModel events, string eventName, int delay)
        {
            await Task.Delay(delay);
            DoEvent(events, eventName);
        }

        private void DoEvent(AsyncViewModel events, string eventName)
        {
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                events.Bag.Add(new EventTagViewModel
                {
                    Event = $"({HttpContext?.Request.GetDisplayUrl() }) {eventName} - {i}",
                    Date = DateTime.Now
                });
            }
        }
    }
}
