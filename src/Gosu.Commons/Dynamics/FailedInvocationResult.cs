namespace Gosu.Commons.Dynamics
{
    public class FailedInvocationResult : InvocationResult
    {
        public FailedInvocationResult() 
            : base(wasInvocationSuccessful: false, returnValue: null) {}
    }
}