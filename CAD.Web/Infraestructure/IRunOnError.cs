namespace CAD.Web.Infraestructure
{
    public interface IRunOnError
    {
        void Execute();
    }

    public class Farofa: IRunOnError
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}