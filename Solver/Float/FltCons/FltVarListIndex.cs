//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarListIndex.cs $
 * 
 * 56    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 55    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 54    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 53    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 52    31-07-07 22:03 Patrick
 * 
 * 51    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 50    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 49    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 48    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 47    11-06-07 23:26 Patrick
 * added copying of goals
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarListIndex.
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class FltVarListIndex : ConstraintVarList2<FltVar,IntVar,FltVarList>
	{
		public FltVarListIndex( Solver solver ) :
			this( new FltVar( solver ), new IntVar( solver ), new FltVarList( solver ) )
		{
		}

		public FltVarListIndex( FltVarList list, IntVar index ) :
			this( new FltVar( list.Solver ), index, list )
		{
		}

		public FltVarListIndex( FltVar var0, IntVar index, FltVarList list ) :
			base( var0, index, list )
		{
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=" + Index.ToString( wd ) + "{" + VarList.ToString( wd ) + "}";
		}

		public new IntVar Index
		{
			get
			{
				return Var1;
			}
		}

		public override bool IsViolated()
		{
			return false;
		}

		public override void Update()
		{
			UpdateIndex();
			UpdateVar0();
			UpdateVar1();
			UpdateVarList();
		}

		private void UpdateVar0()
		{
			FltDomain domain	= new FltDomain();

			foreach( int idx in Var1 )
			{
				domain	= domain.Union( VarList[ idx ].Domain );
			}
		
			Var0.Intersect( domain );
		}
		
		private void UpdateVar1()
		{
			IntDomain diff	= new IntDomain();

			foreach( int idx in Index )
			{
				FltVar var		= VarList[ idx ];

				if( !Var0.Domain.IntersectsWith( var.Domain ) )
				{
					diff	= diff.Union( idx );
				}
			}
			
			Index.Difference( diff );
		}

		private void UpdateVarList()
		{
			foreach( int idx in Index )
			{
				VarList[ idx ].Intersect( Var0.Domain );
			}
		}

		private void UpdateIndex()
		{
			Var1.Intersect( 0, VarList.Count - 1 );
		}
	}
}

//--------------------------------------------------------------------------------
