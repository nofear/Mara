//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListDotProduct.cs $
 * 
 * 36    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 35    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 34    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 33    9-08-07 2:34 Patrick
 * made init(..) private
 * 
 * 32    7-07-07 15:36 Patrick
 * fixed another copy issue
 * 
 * 31    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 30    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 29    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 28    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 27    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 26    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint R = Var[1] * Int[1] + Var[2] * Int[2] + ... + Var[n] * Int[n]
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVarListDotProduct : ConstraintVarList1<IntVar,IntVarList>
	{
		public IntVarListDotProduct( Solver solver, IntVar[] varArray, int[] intArray ) :
			this( new IntVar( solver ), new IntVarList( solver, varArray ), intArray )
		{
		}

		public IntVarListDotProduct( Solver solver, IntVarList varList, int[] intArray ) :
			this( new IntVar( solver ), varList, intArray )
		{
		}

		public IntVarListDotProduct( IntVar var, IntVar[] varArray, int[] intArray ) :
			this( var, new IntVarList( var.Solver, varArray ), intArray )
		{
		}

		public IntVarListDotProduct( IntVar var, IntVarList varList, int[] intArray ) :
			base( var, varList )
		{
			m_IntArray	= intArray;
		}

		public override string ToString( bool wd )
		{
			StringBuilder str	= new StringBuilder();
			
			str.Append( Var0.ToString( wd ) );
			str.Append( "=(" );
	
			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				if( idx > 0 )
				{
					str.Append( "+" );
				}

				str.Append( VarList[ idx ].ToString( wd ) );
				str.Append( "*" );
				str.Append( m_IntArray[ idx ].ToString( CultureInfo.CurrentCulture ) );
			}			

			str.Append( ")" );

			return str.ToString();
		}

		public override bool IsViolated()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Update()
		{
			UpdateVar0();
			UpdateVarList();
		}

		private void UpdateVar0()
		{
			if( VarList.Count > 0 )
			{
				IntDomain domain	= VarList.Front().Domain * m_IntArray[ 0 ];
				
				for( int idx = 1; idx < VarList.Count; ++idx )
				{
					domain	+= VarList[ idx ].Domain * m_IntArray[ idx ];
				}
				
				Var0.Intersect( domain );
			}
		}

		private void UpdateVarList()
		{
			int sum_min		= 0;
			int sum_max		= 0;

			for( int i = 0; i < VarList.Count; ++i )
			{
				IntVar var	= VarList[ i ];

				sum_min		+= var.Min * m_IntArray[ i ];
				sum_max		+= var.Max * m_IntArray[ i ];
			}

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntVar var	= VarList[ idx ];

				int min		= ( Var0.Min - ( sum_max - var.Max * m_IntArray[ idx ] ) ) / m_IntArray[ idx ];
				int max		= ( Var0.Max - ( sum_min - var.Min * m_IntArray[ idx ] ) ) / m_IntArray[ idx ];
				
				var.Intersect( min, max );
			}
		}
		
		int[]	m_IntArray;
	}
}

//--------------------------------------------------------------------------------
