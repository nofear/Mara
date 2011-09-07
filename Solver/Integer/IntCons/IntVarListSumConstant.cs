//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListSumConstant.cs $
 * 
 * 20    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 19    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 18    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 17    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 16    25-07-07 3:59 Patrick
 * renamed Fail() -> Violate()
 * 
 * 15    12-07-07 21:29 Patrick
 * fixed copier
 * 
 * 14    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 13    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 12    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 11    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 10    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 9     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint: C = V1 + V2 + .. + Vn
	/// </summary>
	public class IntVarListSumConstant : ConstraintVarList0<IntVarList>
	{
		public IntVarListSumConstant( IntVarList list, int value ) :
			base( list )
		{
			m_DomArray	= new IntDomain[ list.Count ];
			m_Value		= value;
		}

		public override string ToString( bool wd )
		{
			return m_Value.ToString() + "=Sum(" + VarList.ToString( wd ) + ")";
		}

		public override void Post()
		{
			Post( Variable.How.OnInterval );
		}

		public override bool IsViolated()
		{
			int idx;
			for( idx = 0;
					idx < VarList.Count
						&& VarList[ idx ].Domain.Contains( m_Value );
					++idx ) {};

			return idx == VarList.Count;
		}

		public override void Update()
		{
			Update2();
		}
		
		private void Update1()
		{
			int sum_min		= 0;
			int sum_max		= 0;

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntDomain dom	= VarList[ idx ].Domain;

				m_DomArray[ idx ]	= dom;

				sum_min		+= dom.Min;
				sum_max		+= dom.Max;
			}

			if( m_Value < sum_min || m_Value > sum_max )
			{
				Violate();
				return;
			}
			
			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntDomain dom	= m_DomArray[ idx ];

				int min		= m_Value - ( sum_max - dom.Max );
				int max		= m_Value - ( sum_min - dom.Min );
				
				if( min > dom.Min || max < dom.Max )
				{
					sum_min		-= dom.Min;
					sum_max		-= dom.Max;
				
					dom		= dom.Intersect( min, max );
				
					if( dom.IsEmpty() )
					{
						Violate();
						return;
					}

					sum_min		+= dom.Min;
					sum_max		+= dom.Max;
					
					if( m_Value < sum_min || m_Value > sum_max )
					{
						Violate();
						return;
					}
					
					m_DomArray[ idx ]	= dom;

					idx		= -1;
				}
			}
			
			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				VarList[ idx ].Update( m_DomArray[ idx ] );
			}			
		}

		private void Update2()
		{
			int sum_min		= 0;
			int sum_max		= 0;

			for( int i = 0; i < VarList.Count; ++i )
			{
				IntDomain dom	= VarList[ i ].Domain;

				sum_min		+= dom.Min;
				sum_max		+= dom.Max;
			}

			if( m_Value < sum_min || m_Value > sum_max )
			{
				Violate();
				return;
			}

			for( int idx = 0; idx < VarList.Count; ++idx )
			{
				IntVar var	= VarList[ idx ];

				int min		= m_Value - ( sum_max - var.Max );
				int max		= m_Value - ( sum_min - var.Min );
				
				if( min > var.Min || max < var.Max )
				{
					sum_min		-= var.Min;
					sum_max		-= var.Max;
				
					var.Intersect( min, max );
				
					if( var.Domain.IsEmpty() )
						return;

					sum_min		+= var.Min;
					sum_max		+= var.Max;
					
					if( m_Value < sum_min || m_Value > sum_max )
					{
						Violate();
						return;
					}

					idx		= -1;
				}
			}
		}

		private int			m_Value;
		private IntDomain[]	m_DomArray;
	}
}