using System;

using R5T.F0028;


namespace D8S.E0003
{
    public static class Instances
    {
        public static IOperations Operations { get; } = E0003.Operations.Instance;
        public static IServicesOperator ServicesOperator { get; } = R5T.F0028.ServicesOperator.Instance;
    }
}