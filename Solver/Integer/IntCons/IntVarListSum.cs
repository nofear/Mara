//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListSum.cs $
 * 
 * 60    2/22/09 2:42p Patrick
 * 
 * 59    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 58    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 57    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 56    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 55    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 54    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 53    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 52    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 51    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 50    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 49    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint: R = V1 + V2 + .. + Vn
	/// </summary>
	public sealed class IntVarListSum : ConstraintVarList1<IntVar,IntVarList>
	{
		public IntVarListSum( IntVarList list ) :
			this( new IntVar( list.Solver ), list )
		{
		}
			
		public IntVarListSum( IntVar var0, IntVarList list ) :
			base( var0, list )
		{
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=Sum(" + VarList.ToString( wd ) + ")";
		}

		public override void Post()
		{
			Post( Variable.How.OnInterval );
		}

		public override bool IsViolated()
		{
			if( !IsBound() )
				return false;

			int sum = 0;

			foreach( IntVar var in VarList )
			{
				sum += var.Value;
			}

			return Var0.Value != sum;
		}
		
		public override void Update()
		{
			int sum_min		= 0;
			int sum_max		= 0;

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntDomain dom	= VarList[ idx ].Domain;

				sum_min		+= dom.Min;
				sum_max		+= dom.Max;
			}

			Var0.Intersect( sum_min, sum_max );
			if( Var0.Domain.IsEmpty() )
				return;

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntVar var	= VarList[ idx ];

				int min		= Var0.Min - ( sum_max - var.Max );
				int max		= Var0.Max - ( sum_min - var.Min );
				
				if( min > var.Min || max < var.Max )
				{
					sum_min		-= var.Min;
					sum_max		-= var.Max;
				
					var.Intersect( min, max );
				
					if( var.Domain.IsEmpty() )
						break;

					sum_min		+= var.Min;
					sum_max		+= var.Max;
					
					Var0.Intersect( sum_min, sum_max );
					
					if( Var0.Domain.IsEmpty() )
						break;

					idx		= -1;
				}
			}
		}
	}
}