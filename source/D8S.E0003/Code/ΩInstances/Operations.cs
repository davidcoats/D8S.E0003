using System;


namespace D8S.E0003
{
	public class Operations : IOperations
	{
		#region Infrastructure

	    public static IOperations Instance { get; } = new Operations();

	    private Operations()
	    {
        }

	    #endregion
	}
}