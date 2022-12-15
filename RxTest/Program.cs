using System.Reactive.Linq;

var observable = Observable.Create<char>((obs, cancel) =>
{
    return Task.Run(() =>
    {
        while(!cancel.IsCancellationRequested)
        {
            var key = Console.ReadKey();
            obs.OnNext(key.KeyChar);
        }
    });
});

var keys = observable
    .Where(value => char.IsUpper(value))
    .TakeUntil(value => value == 'Q')
    .Publish()
    .RefCount();

keys.Subscribe(new CustomObserver<char>());

var lastItem = keys.LastAsync();
await lastItem;

class CustomObserver<T> : IObserver<T>
{
    public void OnCompleted()
    {
        Console.WriteLine("Completed!");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine("Failed!");
    }

    public void OnNext(T value)
    {
        Console.WriteLine( $"Received {value}" );
    }
}