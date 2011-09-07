//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltBase/FltVarList.cs $
 * 
 * 61    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 60    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 59    14-11-07 23:57 Patrick
 * added ToDomainArray(), Update(..)
 * 
 * 58    10-11-07 1:44 Patrick
 * rename BaseAt => Get(..), added Set(..)
 * 
 * 57    12-10-07 22:38 Patrick
 * using smarter indexing
 * 
 * 56    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 55    12-07-07 21:31 Patrick
 * removed registration at solver
 * 
 * 54    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 53    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 52    4-07-07 21:08 Patrick
 * added Init(..)
 * 
 * 51    4-07-07 20:23 Patrick
 * added Init() methods
 * 
 * 50    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 49    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 48    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 47    22-03-07 22:42 Patrick
 * removed flt alldiff
 * 
 * 46    22-03-07 21:09 Patrick
 * refactored Clone()
 * 
 * 45    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 44    3-03-07 18:53 Patrick
 * separated problem/solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;

using MaraSolver;
using MaraSolver.Reversible;
using MaraInterval.Interval;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarList.
	/// </summary>
	public class FltVarList : VariableList
	{
		public FltVarList( FltVar v0, FltVar v1 ) :
			this( v0.Solver, new FltVar[] { v0, v1 } )
		{
		}

		public FltVarList( FltVar v0, FltVar v1, FltVar v2 ) :
			this( v0.Solver, new FltVar[] { v0, v1, v2 } )
		{
		}

		public FltVarList( Solver solver, FltVar[] list ) :
			this( solver )
		{
			foreach( FltVar var in list )
			{
				Add( var );
			}
		}

		public FltVarList( Solver solver ) :
			this( solver, 0 )
		{
		}

		public FltVarList( Solver solver, int capacity ) :
			base( solver )
		{
			Capacity	= capacity;
		}

		public FltVar[] ToArray()
		{
			FltVar[] list	= new FltVar[ Count ];
		
			for( int idx = 0; idx < Count; ++idx )
			{
				list[ idx ]		= At( idx );
			}

			return list;
		}

		public FltDomain[] ToDomainArray()
		{
			FltDomain[] list	= new FltDomain[ Count ];
		
			for( int idx = 0; idx < Count; ++idx )
			{
				list[ idx ]		= At( idx ).Domain;
			}

			return list;
		}

		public void Update( FltDomain[] list )
		{
			if( list.Length == Count )
			{
				for( int idx = 0; idx < list.Length; ++idx )
				{
					At( idx ).Update( list[ idx ] );
				}
			}
		}

		// Returns sum Cardinality.
		public double SumCardinality
		{
			get
			{
				double cardinality	= 0;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					cardinality		+= At( idx ).Domain.Cardinality;
				}

				return cardinality;			
			}
		}

		// Returns min interval.
		public FltInterval MinInterval
		{
			get
			{
				if( Count == 0 )
					return FltInterval.Empty;

				double tmp	= At( 0 ).Min;
				double min	= tmp;
				double max	= tmp;
				
				for( int idx = 1; idx < Count; ++idx )
				{
					tmp		= At( idx ).Min;
					min		= Math.Min( min, tmp );
					max		= Math.Max( max, tmp );
				}

				return new FltInterval( min, max );
			}
		}
		
		// Returns max interval.
		public FltInterval MaxInterval
		{
			get
			{
				if( Count == 0 )
					return FltInterval.Empty;

				double tmp	= At( 0 ).Max;
				double min	= tmp;
				double max	= tmp;
				
				for( int idx = 1; idx < Count; ++idx )
				{
					tmp		= At( idx ).Max;
					min		= Math.Min( min, tmp );
					max		= Math.Max( max, tmp );
				}

				return new FltInterval( min, max );
			}
		}
						
		public new FltVar this[ int index ]
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
		
		public FltVar At( int index )
		{
			return m_Solver.FltVarList[ m_IndexList[ index ] ];
		}	

		protected override Variable Get( int index )
		{
			return At( index );
		}

		public FltVar Front()
		{
			return At( 0 );
		}		

		public FltVar Back()
		{
			return At( Count - 1 );
		}		

		public FltDomain Union()
		{
			FltDomain domain	= new FltDomain();

			foreach( FltVar var in this )
			{
				domain	= domain.Union( var.Domain );
			}
			
			return domain;
		}

		public FltVarListMax Max()
		{
			return new FltVarListMax( this );
		}
		
		public FltVarListMin Min()
		{
			return new FltVarListMin( this );
		}

		public FltVarListSum Sum()
		{
			return new FltVarListSum( this );
		}

		public FltVarListSum Sum( IntVar var )
		{
			return new FltVarListSum( var, this );
		}

		public FltVarListMul Mul()
		{
			return new FltVarListMul( this );
		}
	
		public FltVarListIndex At( IntVar index )
		{			
			return new FltVarListIndex( this, index );
		}
	}
}
