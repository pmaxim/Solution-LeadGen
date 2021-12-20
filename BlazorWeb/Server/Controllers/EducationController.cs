using BlazorWeb.Server.Hubs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = $"{Constants.RoleDeveloper}")]
    [ApiController]
    [Route("[controller]")]
    public class EducationController : ControllerBase
    {
        readonly IHubContext<TalkActive> _hubContext;

        public EducationController(IHubContext<TalkActive> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("[action]")]
        public async Task GetTask()
        {
            async void Action(object obj)
            {
                await _hubContext.Clients.Group(User.Identity!.Name!)
                    .SendAsync("EducationTask",$"Task={Task.CurrentId}, obj={obj}, Thread={Thread.CurrentThread.ManagedThreadId}");
            }

            var t1 = new Task(Action!, "alpha");
            var t2 = new Task(Action!, "beta");
            t2.Start();
            t1.Start();

            await _hubContext.Clients.Group(User.Identity!.Name!).SendAsync("EducationTask", "End");
        }

        [HttpGet("[action]")]
        public async Task GetThread()
        {
            var t = new Thread(new ThreadStart(ThreadProc))
            {
                Priority = ThreadPriority.Lowest
            };
            t.Start();

            for (var i = 0; i < 5; i++)
            {
                await _hubContext.Clients.Group(User.Identity!.Name!).SendAsync("EducationThread", $"Main: {i}");

                Thread.Sleep(1000);
            }
            
            t.Interrupt();
            // Wait for ThreadProc to end.
            t.Join();
            await _hubContext.Clients.Group(User.Identity!.Name!).SendAsync("EducationThread", "Interrupt");
        }

        public async void ThreadProc()
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return;
            var count = 0;
            var flag = true;
            try
            {
                while (flag)
                {
                    await _hubContext.Clients.Group(User.Identity!.Name!)
                        .SendAsync("EducationThread", $"Thread Proc: {++count}");
                    Thread.Sleep(500);
                    if (count > 100) break;
                }
            }
            catch (ThreadInterruptedException e)
            { 
                await _hubContext.Clients.Group(User.Identity!.Name!).SendAsync("EducationThread", e.Message);
            }
            finally
            {
                flag = false;
            }
        }
    }
}
