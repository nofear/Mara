//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarListSum.cs $
 * 
 * 48    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 47    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 46    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 45    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 44    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 43    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 42    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 41    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 40    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 39    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 38    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;

using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarListSum.
	/// </summary>
	public class FltVarListSum : ConstraintVarList1<FltVar,FltVarList>
	{
		public FltVarListSum( FltVarList list ) :
			this( new FltVar( list.Solver ), list )
		{
		}
			
		public FltVarListSum( FltVar var0, FltVarList list ) :
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
			bool isViolated = false;

			if( IsBound() )
			{
				int sum = 0;

				foreach( IntVar var in VarList )
				{
					sum += var.Value;
				}

				isViolated = Var0.Value != sum;
			}
			
			return isViolated;
		}
		
		public override void Update()
		{
			double sum_min		= 0;
			double sum_max		= 0;

			for( int i = 0; i < VarList.Count; ++i )
			{
				FltInterval intv	= VarList[ i ].Domain.Interval;

				sum_min		+= intv.Min;
				sum_max		+= intv.Max;
			}

			Var0.Intersect( sum_min, sum_max );

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				FltVar var	= VarList[ idx ];

				double min		= Var0.Min - ( sum_max - var.Max );
				double max		= Var0.Max - ( sum_min - var.Min );
				
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
