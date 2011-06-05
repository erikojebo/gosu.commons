namespace Gosu.Commons.Dynamics
{
    public class SuccessfulInvocationResult : InvocationResult
    {
        public SuccessfulInvocationResult(object returnValue) : base(true, returnValue) {}
    }
}