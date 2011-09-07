//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Integer/IntSearch/IntVarValueSelector.cs $
 * 
 * 4     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 3     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 2     17-03-06 20:00 Patrick
 * added into Search namespace
 * 
 * 1     17-03-06 19:49 Patrick
 * refactored to generic IntGenerate
 */
//--------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	/// <summary>
	/// 
	/// </summary>
	public class IntVarValueSelector
	{
		public delegate int Select( IntVar var );

		static public int Min( IntVar var )
		{
			return var.Min;
		}

		static public int Max( IntVar var )
		{
			return var.Max;
		}

		static public int Mid( IntVar var )
		{
			return ( var.Min + var.Max ) / 2;
		}
	}
}

//--------------------------------------------------------------------------------
