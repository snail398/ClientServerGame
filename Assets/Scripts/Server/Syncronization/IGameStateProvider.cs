namespace Server {
    public interface IGameStateProvider<T> {
        T State { get; }
    }
}