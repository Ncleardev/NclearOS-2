
namespace NclearOS2
{
    public class ServiceExample : Service
    {
        public ServiceExample() : base("Service Example", Priority.High) { }
        internal override bool Start()
        {
            return true;
        }
        internal override bool Update()
        {
            return true;
        }
        internal override int Stop() { return 0; }
    }
}