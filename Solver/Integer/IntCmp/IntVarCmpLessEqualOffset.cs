//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmp/IntVarCmpLessEqualOffset.cs $
 * 
 * 8     6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 7     11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 6     31-07-07 21:45 Patrick
 * fixed copy method
 * 
 * 5     5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 4     28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 3     27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 2     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 1     20-06-07 1:54 Patrick
 * added constraint
 * 
 * 20    11-06-07 23:26 Patrick
 * added copying of goals
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using System.Globalization;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmpLess.
	/// </summary>
	public class IntVarCmpLessEqualOffset : IntVarCmp
	{
		public IntVarCmpLessEqualOffset( IntVar var0, IntVar var1, int offset ) :
			base( var0, var1 )
		{
			m_Offset		= offset;
		}

		public override string ToString( bool wd )
		{
			return ToString( " + " + m_Offset.ToString( CultureInfo.CurrentCulture ) + "<=", wd );
		}

		public int Offset
		{
			get
			{
				return m_Offset;
			}
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value + m_Offset <= Var1.Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Math.Min( Var0.Min, Var1.Min - m_Offset ), Math.Min( Var0.Max, Var1.Max - m_Offset ) );
			Var1.Intersect( Math.Max( Var0.Min + m_Offset, Var1.Min ), Math.Max( Var0.Max + m_Offset, Var1.Max ) );
		}

		private int m_Offset;
	}
}
