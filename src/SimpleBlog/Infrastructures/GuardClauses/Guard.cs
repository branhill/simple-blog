namespace SimpleBlog.Infrastructures.GuardClauses
{
    public interface IGuardClause
    {
    }

    public class Guard : IGuardClause
    {
        private Guard() { }

        public static IGuardClause Against { get; } = new Guard();
    }
}
