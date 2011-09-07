//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListIndex.cs $
 * 
 * 68    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 67    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 66    17-11-07 18:00 Patrick
 * added IntDomainList
 * 
 * 65    17-11-07 2:22 Patrick
 * propagate varlist, added IntDomainListIndex class
 * 
 * 64    15-11-07 0:51 Patrick
 * do not update varlist!
 * 
 * 63    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 62    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 61    31-07-07 20:18 Patrick
 * 
 * 60    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 59    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 58    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 57    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 56    11-06-07 23:26 Patrick
 * added copying of goals
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint R = List[ Index ].
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVarListIndex : ConstraintVarList2<IntVar,IntVar,IntVarList>
	{
		public IntVarListIndex( Solver solver ) :
			this( new IntVar( solver ), new IntVar( solver ), new IntVarList( solver ) )
		{
		}

		public IntVarListIndex( IntVarList list, IntVar index ) :
			this( new IntVar( list.Solver ), index, list )
		{
		}

		public IntVarListIndex( IntVar var0, IntVar index, IntVarList list ) :
			base( var0, index, list )
		{
			m_IntVarListUpdate	= true;
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=" + Index.ToString( wd ) + "{" + VarList.ToString( wd ) + "}";
		}

		public bool IntVarListUpdate
		{
			get
			{
				return m_IntVarListUpdate;
			}
		
			set
			{
			
			}
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
			
			if( m_IntVarListUpdate )
			{
				UpdateVarList();
			}
		}

		private void UpdateVar0()
		{
			IntDomain domain	= new IntDomain();

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
				IntVar var		= VarList[ idx ];

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
			Index.Intersect( 0, VarList.Count - 1 );
		}
		
		bool m_IntVarListUpdate;
	}
}

//--------------------------------------------------------------------------------
