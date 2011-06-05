namespace Gosu.Commons.Dynamics
{
    public abstract class InvocationResult
    {
        protected InvocationResult(bool wasInvocationSuccessful, object returnValue)
        {
            WasInvocationSuccessful = wasInvocationSuccessful;
            ReturnValue = returnValue;
        }

        public bool WasInvocationSuccessful { get; private set; }

        public object ReturnValue { get; private set; }
    }
}