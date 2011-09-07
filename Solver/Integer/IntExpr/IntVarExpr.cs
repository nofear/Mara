using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver.BaseConstraint;

namespace MaraSolver.Integer
{
	public abstract class IntVarExpr : ConstraintVarExpr
	{
		protected IntVarExpr( Solver solver, Variable[] varList ) :
			base( solver, varList )
		{
		}

		public override void Post()
		{
			Post( Variable.How.OnInterval );
		}

		public override bool Equals( object obj )
		{
			return base.Equals( obj );
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public abstract IntVar Var0
		{
			get;
		}

		#region IntVarExpr (+,-,*,/) IntVarExpr

		// v0 = v1 + v2
		public static IntVarExpr operator +( IntVarExpr lhs, IntVarExpr rhs )
		{
			IntVarExpr expr		= new IntVarExprVarAdd( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 - v2
		public static IntVarExpr operator -( IntVarExpr lhs, IntVarExpr rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarSub( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 * v2
		public static IntVarExpr operator *( IntVarExpr lhs, IntVarExpr rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarMul( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 / v2
		public static IntVarExpr operator /( IntVarExpr lhs, IntVarExpr rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarDiv( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}
		
		#endregion

		#region IntVarExpr (+,-,*,/) IntVar

		// v0 = v1 + v2
		public static IntVarExpr operator +( IntVarExpr lhs, IntVar rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarAdd( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 - v2
		public static IntVarExpr operator -( IntVarExpr lhs, IntVar rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarSub( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 * v2
		public static IntVarExpr operator *( IntVarExpr lhs, IntVar rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarMul( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 / v2
		public static IntVarExpr operator /( IntVarExpr lhs, IntVar rhs )
		{			
			IntVarExpr expr		= new IntVarExprVarDiv( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		#endregion

		#region IntVarExpr (+,-,*,/) int

		// v0 = v1 + v2
		public static IntVarExpr operator +( IntVarExpr lhs, int rhs )
		{			
			IntVarExpr expr		= new IntVarExprValAdd( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 - v2
		public static IntVarExpr operator -( IntVarExpr lhs, int rhs )
		{			
			IntVarExpr expr		= new IntVarExprValSub( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 * v2
		public static IntVarExpr operator *( IntVarExpr lhs, int rhs )
		{			
			IntVarExpr expr		= new IntVarExprValMul( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 / v2
		public static IntVarExpr operator /( IntVarExpr lhs, int rhs )
		{			
			IntVarExpr expr		= new IntVarExprValDiv( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}
		
		#endregion

		#region IntVarExpr <comp> val

		public static IntVarCmpVal operator ==( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValEqual( lhs.Var0, rhs );
		}

		public static IntVarCmpVal operator !=( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValNotEqual( lhs.Var0, rhs );
		}

		public static IntVarCmpVal operator <( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValLess( lhs.Var0, rhs );
		}

		public static IntVarCmpVal operator <=( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValLessEqual( lhs.Var0, rhs );
		}

		public static IntVarCmpVal operator >( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValGreater( lhs.Var0, rhs );
		}

		public static IntVarCmpVal operator >=( IntVarExpr lhs, int rhs )
		{			
			return new IntVarCmpValGreaterEqual( lhs.Var0, rhs );
		}

		#endregion
	}
}
