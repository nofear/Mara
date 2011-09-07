using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver.BaseConstraint;

namespace MaraSolver.Float
{
	public abstract class FltVarExpr : ConstraintVarExpr
	{
		protected FltVarExpr( Solver solver, Variable[] varList ) :
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

		public abstract FltVar Var0
		{
			get;
		}

		#region FltVarExpr (+,-,*,/) FltVarExpr

		// v0 = v1 + v2
		public static FltVarExpr operator +( FltVarExpr lhs, FltVarExpr rhs )
		{
			FltVarExpr expr		= new FltVarExprAdd( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 - v2
		public static FltVarExpr operator -( FltVarExpr lhs, FltVarExpr rhs )
		{			
			FltVarExpr expr		= new FltVarExprSub( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 * v2
		public static FltVarExpr operator *( FltVarExpr lhs, FltVarExpr rhs )
		{			
			FltVarExpr expr		= new FltVarExprMul( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}

		// v0 = v1 / v2
		public static FltVarExpr operator /( FltVarExpr lhs, FltVarExpr rhs )
		{			
			FltVarExpr expr		= new FltVarExprDiv( lhs.Var0, rhs.Var0 );

			expr.AddExpr( lhs );
			expr.AddExpr( rhs );

			return expr;
		}
		
		#endregion

		#region FltVarExpr (+,-,*,/) FltVar

		// v0 = v1 + v2
		public static FltVarExpr operator +( FltVarExpr lhs, FltVar rhs )
		{			
			FltVarExpr expr		= new FltVarExprAdd( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 - v2
		public static FltVarExpr operator -( FltVarExpr lhs, FltVar rhs )
		{			
			FltVarExpr expr		= new FltVarExprSub( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 * v2
		public static FltVarExpr operator *( FltVarExpr lhs, FltVar rhs )
		{			
			FltVarExpr expr		= new FltVarExprMul( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 / v2
		public static FltVarExpr operator /( FltVarExpr lhs, FltVar rhs )
		{			
			FltVarExpr expr		= new FltVarExprDiv( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		#endregion

		#region FltVarExpr (+,-,*,/) double

		// v0 = v1 + v2
		public static FltVarExpr operator +( FltVarExpr lhs, double rhs )
		{			
			FltVarExpr expr		= new FltVarExprValAdd( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 - v2
		public static FltVarExpr operator -( FltVarExpr lhs, double rhs )
		{			
			FltVarExpr expr		= new FltVarExprValSub( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 * v2
		public static FltVarExpr operator *( FltVarExpr lhs, double rhs )
		{			
			FltVarExpr expr		= new FltVarExprValMul( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}

		// v0 = v1 / v2
		public static FltVarExpr operator /( FltVarExpr lhs, double rhs )
		{			
			FltVarExpr expr		= new FltVarExprValDiv( lhs.Var0, rhs );

			expr.AddExpr( lhs );

			return expr;
		}
		
		#endregion

		#region FltVarExpr <comp> val

		public static FltVarCmpVal operator ==( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValEqual( lhs.Var0, rhs );
		}

		public static FltVarCmpVal operator !=( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValNotEqual( lhs.Var0, rhs );
		}

		public static FltVarCmpVal operator <( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValLess( lhs.Var0, rhs );
		}

		public static FltVarCmpVal operator <=( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValLessEqual( lhs.Var0, rhs );
		}

		public static FltVarCmpVal operator >( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValGreater( lhs.Var0, rhs );
		}

		public static FltVarCmpVal operator >=( FltVarExpr lhs, double rhs )
		{			
			return new FltVarCmpValGreaterEqual( lhs.Var0, rhs );
		}

		#endregion
	}
}
