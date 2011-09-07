//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListAllDifferent.cs $
 * 
 * 81    2/10/09 10:00p Patrick
 * simplified code
 * 
 * 80    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 79    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 78    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 77    21-11-07 1:53 Patrick
 * made propagation class public
 * 
 * 76    14-11-07 23:58 Patrick
 * changed filtering methods to static
 * 
 * 75    20-10-07 0:56 Patrick
 * added 2nd version of threaded propagation queue
 * 
 * 74    19-10-07 0:35 Patrick
 * added demonlist
 * 
 * 73    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 72    9-08-07 2:34 Patrick
 * made init(..) private
 * 
 * 71    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 70    8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 69    8-08-07 21:55 Patrick
 * moved different filtering algorithms into own class
 * 
 * 68    25-07-07 3:59 Patrick
 * renamed Fail() -> Violate()
 * 
 * 67    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 66    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 65    27-06-07 22:17 Patrick
 * added SolverCopier class
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint AllDifferent( IntVarList )
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVarListAllDifferent : ConstraintVarList0<IntVarList>
	{
		public IntVarListAllDifferent( Solver solver ) :
			this( new IntVarList( solver ) )
		{
		}

		public IntVarListAllDifferent( Solver solver, IntVar[] varList ) :
			base( new IntVarList( solver, varList ) )
		{
		}

		public IntVarListAllDifferent( IntVarList list ) :
			base( list )
		{
		}

		public override string ToString( bool wd )
		{
			return "AllDifferent(" + VarList.ToString( wd ) + ")";
		}

		public IntVarList IntVarList
		{
			get
			{
				return VarList as IntVarList;
			}
		}

		public override void Post()
		{
			switch( Level )
			{
				case PropagateLevel.Normal:
				{
					m_Update	= new AllDifferentArrayX( this );

					foreach( Variable var in VarList )
					{
						var.OnVarIsBoundList.Add( this );
					}
				}
				break;
							
				case PropagateLevel.High:
				{
					m_Update	= new AllDifferentByCountX( this );

					foreach( Variable var in VarList )
					{
						var.OnVarDomainList.Add( this );
					}
				}
				break;
			}
		}

		public override bool IsViolated()
		{
			return IntVarList.Union().Cardinality < IntVarList.Count;
		}

		public override void Update( Variable variable )
		{
			m_Update.Update();
		}

		public override void Update()
		{
			m_Update.Update();
		}
		
		AllDifferentUpdate	m_Update;

		public abstract class AllDifferentUpdate
		{
			protected AllDifferentUpdate( IntVarListAllDifferent constraint )
			{
				m_Constraint	= constraint;
				m_VarList		= constraint.IntVarList;
			}
			
			public abstract void Update();
		
			protected IntVarListAllDifferent m_Constraint;
			protected IntVarList m_VarList;
		}

		public class AllDifferentIsBoundX : AllDifferentUpdate
		{
			public AllDifferentIsBoundX( IntVarListAllDifferent constraint ) :
				base( constraint )
			{
			}

			public override void Update()
			{
				for( int idx = 0; idx < m_VarList.Count; ++idx )
				{
					IntVar var = m_VarList[ idx ];

					if( var.IsBound() )
					{
						Update( var );
					}
				}
			}

			private void Update( IntVar intVar )
			{
				int val		= intVar.Value;

				for( int idx = 0; idx < m_VarList.Count; ++idx )
				{
					IntVar var = m_VarList[ idx ];

					if( !ReferenceEquals( var, intVar )
							&& var.Domain.Contains( val ) )
					{
						if( var.Domain.Min == val
								&& var.Domain.Max == val )
						{
							m_Constraint.Violate();
							return;
						}

						var.Difference( val );
					
						if( var.IsBound() )
						{
							Update( var );				
						}
					}
				}
			}
		}

		public class AllDifferentArrayX : AllDifferentUpdate
		{
			public AllDifferentArrayX( IntVarListAllDifferent constraint ) :
				base( constraint )
			{
			}

			public override void Update()
			{
				if( Update( m_Constraint.IntVarList ) )
					m_Constraint.Violate();
			}

			static public bool Update( IntVarList varList )
			{
				IntDomain[] domArray	= varList.ToDomainArray();

				for( int idx = 0; idx < domArray.Length; ++idx )
				{
					if( domArray[ idx ].Interval.IsBound() )
					{
						if( Update( domArray, idx ) )
							return true;
					}
				}
				
				varList.Update( domArray );
				
				return false;
			}

			static private bool Update( IntDomain[] domArray, int index )
			{
				int val		= domArray[ index ].Interval.Min;

				for( int idx = 0; idx < domArray.Length; ++idx )
				{
					IntDomain dom	= domArray[ idx ];

					if( idx != index
							&& dom.Contains( val ) )
					{
						dom	= dom.Difference( val );

						if( dom.IsEmpty() )
							return true;
					
						domArray[ idx ]	= dom;

						if( dom.Interval.IsBound() )
						{
							if( Update( domArray, idx ) )
								return true;
						}
					}
				}
				
				return false;
			}
		}

		public class AllDifferentByCountX : AllDifferentUpdate
		{
			struct Value
			{
				public int		m_Value;
				public IntVar	m_Variable;
			};
		
			public AllDifferentByCountX( IntVarListAllDifferent constraint ) :
				base( constraint )
			{
			}

			public override void Update()
			{
				if( AllDifferentArrayX.Update( m_Constraint.IntVarList ) )
				{
					m_Constraint.Violate();
					return;
				}
				
				UpdateOnDomain( m_Constraint.IntVarList );
				
				if( UpdateOnCount( m_Constraint.IntVarList ) )
				{
					m_Constraint.Violate();
					return;
				}
			}

			// Count the number of times each domain occurs in the variable list
			// If the number times is equal to the cardinality of the domain, then we know
			// that that domain can only be used by those variables that it's used by.
			static public void UpdateOnDomain( IntVarList varList )
			{
				Dictionary<IntDomain,List<IntVar>> map	= new Dictionary<IntDomain,List<IntVar>>();
				
				foreach( IntVar var in varList )
				{
					IntDomain dom	= var.Domain;
					
					List<IntVar> list;
					if( !map.TryGetValue( dom, out list ) )
					{
						list	= new List<IntVar>( varList.Count );

						map[ dom ]		= list;
					}

					list.Add( var );
				}							

				if( map.Count > 1 )
				{
					foreach( KeyValuePair<IntDomain,List<IntVar>> kv in map )
					{
						if( kv.Key.Cardinality == kv.Value.Count )
						{
							UpdateOnDomain( map, kv.Key );
						}
					}
				}
			}
			
			static private void UpdateOnDomain( Dictionary<IntDomain,List<IntVar>> map, IntDomain domain )
			{
				foreach( KeyValuePair<IntDomain,List<IntVar>> kv in map )
				{
					if( kv.Key != domain )
					{
						foreach( IntVar var in kv.Value )
						{
							var.Difference( domain );
						}					
					}
				}
			}

			static public bool UpdateOnCount( IntVarList varList )
			{
				int min		= varList.MinInterval.Min;
				int max		= varList.MaxInterval.Max;
				int count	= max - min + 1;
			
				if( count < 256 )
				{
					Value[] valList		= new Value[ count ];

					int valCount	= 0;

					foreach( IntVar var in varList )
					{
						foreach( int val in var )
						{
							int idx		= val - min;

							if( valList[ idx ].m_Value == 0 )
							{
								++valCount;
							
								valList[ idx ].m_Variable	= var;
							}
								
							++valList[ idx ].m_Value;
						}

					}

					if( valCount < varList.Count )
						return true;

					for( int idx = 0; idx < count; ++idx )
					{
						if( valList[ idx ].m_Value == 1 )
						{
							IntVar var	= valList[ idx ].m_Variable;

							if( !var.IsBound() )
							{
								int val		= min + idx;

								var.Intersect( val );
							}
						}
					}
				}
			
				return false;
			}
		}

	}
}

//--------------------------------------------------------------------------------
