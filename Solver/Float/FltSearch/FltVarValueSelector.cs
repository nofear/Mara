//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Float/FltSearch/FltVarValueSelector.cs $
 * 
 * 3     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 2     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 1     17-03-06 20:00 Patrick
 * added float version of Generate
 */
//--------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
namespace MaraSolver.Float.Search
{
	/// <summary>
	/// 
	/// </summary>
	public class FltVarValueSelector
	{
		public delegate double Select( FltVar var );

		static public double Min( FltVar var )
		{
			return var.Min;
		}

		static public double Max( FltVar var )
		{
			return var.Max;
		}

		static public double Mid( FltVar var )
		{
			return ( var.Min + var.Max ) / 2;
		}
	}
}

//--------------------------------------------------------------------------------
