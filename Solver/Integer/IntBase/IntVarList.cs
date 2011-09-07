//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntBase/IntVarList.cs $
 * 
 * 73    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 72    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 71    17-11-07 16:58 Patrick
 * added Cardinality property
 * 
 * 70    17-11-07 2:22 Patrick
 * added IsBoundCount(..)
 * 
 * 69    14-11-07 23:57 Patrick
 * added Update(..)
 * 
 * 68    14-11-07 22:44 Patrick
 * added ToDomainArray()
 * 
 * 67    10-11-07 1:44 Patrick
 * rename BaseAt => Get(..), added Set(..)
 * 
 * 66    12-10-07 22:38 Patrick
 * using smarter indexing
 * 
 * 65    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 64    8-08-07 21:27 Patrick
 * 
 * 63    12-07-07 21:31 Patrick
 * removed registration
 * 
 * 62    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 61    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 60    4-07-07 21:08 Patrick
 * added Init(..)
 * 
 * 59    4-07-07 20:23 Patrick
 * added Init() methods
 * 
 * 58    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 57    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 56    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 55    22-03-07 21:09 Patrick
 * refactored Clone()
 * 
 * 54    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 53    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 52    3-03-07 18:53 Patrick
 * separated problem/solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using MaraSolver.Reversible;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements a list of integer variables.
	/// </summary>
	public class IntVarList : VariableList
	{
		public IntVarList( IntVar v0 ) :
			this( v0.Solver, new IntVar[] { v0 } )
		{
		}

		public IntVarList( IntVar v0, IntVar v1 ) :
			this( v0.Solver, new IntVar[] { v0, v1 } )
		{
		}

		public IntVarList( IntVar v0, IntVar v1, IntVar v2 ) :
			this( v0.Solver, new IntVar[] { v0, v1, v2 } )
		{
		}

		public IntVarList( Solver solver, IntVar[] list ) :
			this( solver, list.Length )
		{
			foreach( IntVar var in list )
			{
				Add( var );
			}
		}

		public IntVarList( Solver solver ) :
			this( solver, 0 )
		{
		}

		public IntVarList( Solver solver, int capacity ) :
			base( solver )
		{
			Capacity	= capacity;
		}

		public IntVar[] ToArray()
		{
			IntVar[] list	= new IntVar[ Count ];
		
			for( int idx = 0; idx < Count; ++idx )
			{
				list[ idx ]		= At( idx );
			}

			return list;
		}

		public IntDomain[] ToDomainArray()
		{
			IntDomain[] list	= new IntDomain[ Count ];
		
			for( int idx = 0; idx < Count; ++idx )
			{
				list[ idx ]		= At( idx ).Domain;
			}

			return list;
		}

		public void Update( IntDomain[] list )
		{
			if( list.Length == Count )
			{
				for( int idx = 0; idx < list.Length; ++idx )
				{
					At( idx ).Update( list[ idx ] );
				}
			}
		}

		// Returns number of bound variables.
		public int IsBoundCount
		{
			get
			{
				int count	= 0;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					if( At( idx ).IsBound() )
						++count;
				}

				return count;			
			}
		}

		// Returns sum cardinality.
		public int SumCardinality
		{
			get
			{
				int cardinality	= 0;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					cardinality		+= At( idx ).Domain.Cardinality;
				}

				return cardinality;			
			}
		}

		// Returns domain of cardinality values.
		public IntDomain Cardinality
		{
			get
			{
				IntDomain card	= IntDomain.Empty;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					card	= card.Union( At( idx ).Domain.Cardinality );
				}

				return card;			
			}
		}
		
		// Returns min interval.
		public IntInterval MinInterval
		{
			get
			{
				if( Count == 0 )
					return IntInterval.Empty;

				int tmp		= At( 0 ).Min;
				int min		= tmp;
				int max		= tmp;
				
				for( int idx = 1; idx < Count; ++idx )
				{
					tmp		= At( idx ).Min;
					min		= Math.Min( min, tmp );
					max		= Math.Max( max, tmp );
				}

				return new IntInterval( min, max );
			}
		}
		
		// Returns max interval.
		public IntInterval MaxInterval
		{
			get
			{
				if( Count == 0 )
					return IntInterval.Empty;

				int tmp		= At( 0 ).Max;
				int min		= tmp;
				int max		= tmp;
				
				for( int idx = 1; idx < Count; ++idx )
				{
					tmp		= At( idx ).Max;
					min		= Math.Min( min, tmp );
					max		= Math.Max( max, tmp );
				}

				return new IntInterval( min, max );
			}
		}
				
		public new IntVar this[ int index ]
		{
			get
			{
				return At( index );
			}

			set
			{
				Set( index, value );
			}
		}
		
		public IntVar At( int index )
		{
			return m_Solver.IntVarList[ m_IndexList[ index ] ];
		}	

		protected override Variable Get( int index )
		{
			return At( index );
		}

		public IntVar Front()
		{
			return At( 0 );
		}		

		public IntVar Back()
		{
			return At( Count - 1 );
		}

		public IntDomain Union()
		{
			IntDomain domain	= new IntDomain();

			foreach( IntVar var in this )
			{
				domain	= domain.Union( var.Domain );
			}
			
			return domain;
		}

		public IntVarListMax Max()
		{
			return new IntVarListMax( this );
		}
		
		public IntVarListMin Min()
		{
			return new IntVarListMin( this );
		}

		public IntVarListSum Sum()
		{
			return new IntVarListSum( this );
		}

		public IntVarListSum Sum( IntVar var )
		{
			return new IntVarListSum( var, this );
		}

		public IntVarListSumConstant Sum( int val )
		{
			return new IntVarListSumConstant( this, val );
		}

		public IntVarListMul Mul()
		{
			return new IntVarListMul( this );
		}

		public IntVarListAllDifferent AllDifferent()
		{
			return new IntVarListAllDifferent( this );
		}
		
		public IntVarListIndex At( IntVar index )
		{			
			return new IntVarListIndex( this, index );
		}
		
		public IntVarListDotProduct DotProduct( int[] intArray )
		{
			return new IntVarListDotProduct( m_Solver, this, intArray );
		}
	}
}

//--------------------------------------------------------------------------------
