using Codeworx.Identity;

public class DisableLoginDelayService : ILoginDelayService
{
    public Task DelayAsync()
    {
        return Task.CompletedTask;
    }

    public void Record(TimeSpan duration)
    {
    }
}