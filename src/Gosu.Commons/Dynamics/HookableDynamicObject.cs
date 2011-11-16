using System;
using System.Dynamic;

namespace Gosu.Commons.Dynamics
{
    public class HookableDynamicObject : DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var invocationResult = MethodMissing(binder.Name, args);

            result = invocationResult.ReturnValue;

            return invocationResult.WasInvocationSuccessful;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var invocationResult = PropertyMissing(binder.Name);

            result = invocationResult.ReturnValue;

            return invocationResult.WasInvocationSuccessful;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var conversionMode = binder.Explicit ? ConversionMode.Explicit : ConversionMode.Implicit;

            var invocationResult = ConversionMissing(binder.Type, conversionMode);

            result = invocationResult.ReturnValue;

            return invocationResult.WasInvocationSuccessful;
        }

        protected virtual InvocationResult ConversionMissing(Type type, ConversionMode conversionMode)
        {
            return new FailedInvocationResult();
        }

        protected virtual InvocationResult MethodMissing(string methodName, object[] arguments)
        {
            return new FailedInvocationResult();
        }

        protected virtual InvocationResult PropertyMissing(string name)
        {
            return new FailedInvocationResult();
        }
    }
}