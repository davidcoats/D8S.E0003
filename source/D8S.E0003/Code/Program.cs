using System;


namespace D8S.E0003
{
    class Program
    {
        static void Main()
        {
            //Instances.Operations.TryGetNonExistentService();
            //Instances.Operations.TryGetEnumerableOfNonExistentService();
            //Instances.Operations.TryGetRequiredNonExistentService();
            //Instances.Operations.TryGetServiceProvider();
            //Instances.Operations.TryGetIServiceProvider();
            //Instances.Operations.TryGetServiceCollection();
            //Instances.Operations.TryGetIServiceCollection();

            Instances.Operations.TryDisposeOfServiceProvider();
        }
    }
}