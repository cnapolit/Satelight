using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Comms.Host.Interface;
using HostPlugin.Services.RequestHandlers;

namespace HostPlugin.Services;

public class Server(IHostListener listener, IRequestHandlerDirectory requestHandlerFactory) : IServer
{
    private readonly object _gate = new();
    private readonly HashSet<Task> _inflight = [];
    private Task _runTask = Task.CompletedTask;

    public void Dispose()
    {
        Task[] tasks;
        lock (_gate)
        {
            tasks = _inflight.ToArray();
        }

        try
        {
            Task.WhenAll(tasks).Wait(1000);
        }
        catch
        {
            // ignored: Dispose is best-effort
        }
    }

    public void Run(CancellationToken token)
    {
        _runTask = RunAsync(token);
    }

    public async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.WaitForConnectionAsync(token);
                Track(ProcessConnectionAsync(connection, token));
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                LogManager.GetLogger().Error(e, "Failed to handle server request.");
            }
        }

        await DrainAsync();
    }

    private async Task ProcessConnectionAsync(IHostConnection connection, CancellationToken token)
    {
        try
        {
            await requestHandlerFactory.HandleAsync(connection, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // normal shutdown
        }
        catch (Exception e)
        {
            LogManager.GetLogger().Error(e, "Failed to resolve server request.");
        }
        finally
        {
            try
            {
                await connection.DisposeAsync();
            }
            catch (Exception e)
            {
                LogManager.GetLogger().Error(e, "Failed to dispose server connection.");
            }
        }
    }

    private void Track(Task task)
    {
        lock (_gate)
        {
            _inflight.Add(task);
        }

        _ = task.ContinueWith(
            t =>
            {
                lock (_gate)
                {
                    _inflight.Remove(t);
                }
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    private async Task DrainAsync()
    {
        while (true)
        {
            Task[] tasks;
            lock (_gate)
            {
                tasks = _inflight.ToArray();
            }

            if (tasks.Length == 0)
            {
                return;
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch
            {
                // exceptions are already logged per-connection
            }
        }
    }
}