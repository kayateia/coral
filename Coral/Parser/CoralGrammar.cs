#region License
/*
	CliMOO - Multi-User Dungeon, Object Oriented for the web
	Copyright (C) 2010-2014 Kayateia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
	This file is based on Irony's MiniPython grammar. Its original notice:

	Copyright (c) Roman Ivantsov
	This source code is subject to terms and conditions of the MIT License
	for Irony. A copy of the license can be found in the License.txt file
	at the root of this distribution. 
	By using this source code in any fashion, you are agreeing to be bound by the terms of the 
	MIT License.
	You must not remove this notice from this software.
*/
#endregion
namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using Irony.Parsing;

// [Language( "Coral", "v1", "Coral scripting language for CliMOO" )]
class CoralGrammar : Grammar
{
	public CoralGrammar()
		: base( caseSensitive: false )
	{
		// 1. Terminals
		var number = CreateNumber( "number" );
		number.AstConfig.NodeType = typeof( AstNumber );
		var identifier = TerminalFactory.CreatePythonIdentifier( "identifier" );
		identifier.AstConfig.NodeType = typeof( AstIdentifier );
		var str = TerminalFactory.CreatePythonString( "string" );
		str.AstConfig.NodeType = typeof( AstString );

		var comment = new CommentTerminal( "comment", "//", "\n", "\r" );
		//comment must to be added to NonGrammarTerminals list; it is not used directly in grammar rules,
		// so we add it to this list to let Scanner know that it is also a valid terminal. 
		base.NonGrammarTerminals.Add( comment );
		comment = new CommentTerminal( "comment", "/*", "*/" );
		base.NonGrammarTerminals.Add( comment );
		var comma = ToTerm( "," );
		var colon = ToTerm( ":" );
		var dollar = ToTerm( "$" );

		// 2. Non-terminals
		var Expr = new NonTerminal( "Expr" );
		var Term = new NonTerminal( "Term" );
		var PoundRef = new NonTerminal( "PoundRef", typeof( AstIdentifier ) );
		var BinExpr = new NonTerminal( "BinExpr", typeof( AstExpression ) );
		var ParExpr = new NonTerminal( "ParExpr" );
		var UnExpr = new NonTerminal( "UnExpr", typeof( AstExpression ) );
		var UnOp = new NonTerminal( "UnOp", "operator" );
		var BinOp = new NonTerminal( "BinOp", "operator" );
		var AssignmentStmt = new NonTerminal( "AssignmentStmt", typeof( AstAssignment ) );
		var Stmt = new NonTerminal( "Stmt" );
		var ExtStmt = new NonTerminal( "ExtStmt" );
		var ReturnStmt = new NonTerminal( "ReturnStmt", typeof( AstReturn ) );
		var IfStmt = new NonTerminal( "IfStmt", typeof( AstIf ) );
		var ElifClause = new NonTerminal( "ElifClause" );
		var ElifClauses = new NonTerminal( "ElifClauses" );
		var ElseClause = new NonTerminal( "ElseClause" );
		var TryStmt = new NonTerminal( "TryStmt", typeof( AstTry ) );
		var ExceptClause = new NonTerminal( "ExceptClause" );
		//var ExceptClauses = new NonTerminal( "ExceptClauses" );
		var FinallyClause = new NonTerminal( "FinallyClause" );
		var ThrowStmt = new NonTerminal( "ThrowStmt", typeof( AstThrow ) );
		var Block = new NonTerminal( "Block" );
		var StmtList = new NonTerminal( "StmtList", typeof( AstStatements ) );
		var MemberAccess = new NonTerminal( "MemberAccess", typeof( AstMemberAccess ) );
		var ArrayExpr = new NonTerminal( "ArrayExpr", typeof( AstArray ) );
		var ArrayElements = new NonTerminal( "ArrayElements" );
		var DictExpr = new NonTerminal( "DictExpr", typeof( AstDictionary ) );
		var DictElement = new NonTerminal( "DictElement" );
		var DictElements = new NonTerminal( "DictElements" );
		var ForInStmt = new NonTerminal( "ForInStmt", typeof( AstFor ) );
		var ForStmt = new NonTerminal( "ForStmt", typeof( AstWhile ) );
		var BreakStmt = new NonTerminal( "BreakStmt", typeof( AstBreak ) );
		var PassStmt = new NonTerminal( "PassStmt", typeof( AstPass ) );
		var WhileStmt = new NonTerminal( "WhileStmt", typeof( AstWhile ) );
		var ContinueStmt = new NonTerminal( "ContinueStmt", typeof( AstContinue ) );
		var ArrayAccess = new NonTerminal( "ArrayAccess", typeof( AstArrayAccess ) );
		var ArraySliceFull = new NonTerminal( "ArraySliceFull", typeof( AstArraySlice ) );
		var ArraySliceFromStart = new NonTerminal( "ArraySliceFromStart", typeof( AstArraySlice ) );
		var ArraySliceFromEnd = new NonTerminal( "ArraySliceFromEnd", typeof( AstArraySlice ) );

		var ParamList = new NonTerminal( "ParamList" );
		var ArgList = new NonTerminal( "ArgList" );
		var FunctionDef = new NonTerminal( "FunctionDef", typeof( AstFunc ) );
		var FunctionCall = new NonTerminal( "FunctionCall", typeof( AstCall ) );


		// 3. BNF rules
		//Eos is End-Of-Statement token produced by CodeOutlineFilter
		Expr.Rule
			= Term
			| UnExpr
			| BinExpr
			;

		Term.Rule
			= number
			| dollar
			| ParExpr
			| identifier
			| MemberAccess
			| PoundRef
			| ArrayExpr
			| FunctionCall
		//	| FunctionDef
			| DictExpr
			| ArrayAccess
			| ArraySliceFull
			| ArraySliceFromStart
			| ArraySliceFromEnd
			| str
			;

		PoundRef.Rule = "#" + number;

		MemberAccess.Rule = Term + "." + identifier;

		ParExpr.Rule = "(" + Expr + ")";

		UnExpr.Rule = UnOp + Term;
		UnOp.Rule
			= ToTerm( "+" )
			| "-"
			| "!"
			;

		BinExpr.Rule = Expr + BinOp + Expr;
		BinOp.Rule
			= ToTerm( "+" )
			| "-"
			| "*"
			| "/"
			| "**"
			| "<"
			| ">"
			| "<="
			| ">="
			| "!="
			| "=="
			| "||"
			| "&&"
			| "+="
			| "%"
			;

		AssignmentStmt.Rule
			= identifier + "=" + Expr
			| MemberAccess + "=" + Expr
			| ArrayAccess + "=" + Expr;

		Stmt.Rule
			= AssignmentStmt
			| Expr
			| ReturnStmt
			| BreakStmt
			| ContinueStmt
			| ThrowStmt
			| Empty
			;

		ReturnStmt.Rule
			= "return" + Expr
			| "return";

		IfStmt.Rule
			= "if" + Expr + colon + Stmt
			| "if" + Expr + colon + Eos + Block
			| "if" + Expr + colon + Eos + Block + ElifClauses
			| "if" + Expr + colon + Eos + Block + ElifClauses + ElseClause;
		ElifClauses.Rule = MakeStarRule( ElifClauses, null, ElifClause );
		ElifClause.Rule
			= "elif" + Expr + colon + Eos + Block
			;
		ElseClause.Rule
			= "else" + colon + Eos + Block
			;

		TryStmt.Rule
			= "try" + colon + Eos + Block + ExceptClause
			| "try" + colon + Eos + Block + ExceptClause + FinallyClause;

		ExceptClause.Rule
			= "except" + colon + Eos + Block
			| "except" + identifier + colon + Eos + Block
			;

		FinallyClause.Rule = "finally" + colon + Eos + Block;

		ThrowStmt.Rule
			= "throw"
			| "throw" + Expr;

		ExtStmt.Rule
			= Stmt + Eos
			| FunctionDef
			| IfStmt
			| TryStmt
			| ForInStmt
			| ForStmt
			| WhileStmt
			;
		StmtList.Rule = MakePlusRule( StmtList, ExtStmt );

		Block.Rule = Indent + StmtList + Dedent;

		ParamList.Rule = MakeStarRule( ParamList, comma, identifier );
		ArgList.Rule = MakeStarRule( ArgList, comma, Expr );

		FunctionDef.Rule 
			= "def" + identifier + "(" + ParamList + ")" + colon + Eos + Block
			| "def" + ToTerm( "(" ) + ParamList + ")" + colon + Stmt
			;

		FunctionDef.NodeCaptionTemplate = "def #{1}(...)";
		FunctionCall.Rule
			= identifier + "(" + ArgList + ")"
			| Expr + "(" + ArgList + ")";
		FunctionCall.NodeCaptionTemplate = "call #{0}(...)";

		// Implement JSON
		ArrayExpr.Rule = "[" + ArrayElements + "]";
		ArrayElements.Rule = MakeStarRule( ArrayElements, comma, Expr );

		DictExpr.Rule = "{" + DictElements + "}";
		DictElement.Rule = Term + ":" + Expr;
		DictElements.Rule = MakeStarRule( DictElements, comma, DictElement );

		ForInStmt.Rule
			= "for" + identifier + "in" + Expr + colon + Eos + Block
			;
		ForStmt.Rule
			= "for" + AssignmentStmt + comma + Expr + comma + Expr + colon + Eos + Block
			;

		BreakStmt.Rule = "break";
		ContinueStmt.Rule = "continue";
		PassStmt.Rule = "pass";

		WhileStmt.Rule
			= "while" + Expr + colon + Eos + Block
			;

		// This weird song and dance is required because for some reason, Irony is not including
		// colon terminals in the parse tree. (Maybe a setting having to do with a Python based
		// grammar, who knows.) Anyway, e.g. [:5] is not distinguishable from [5:] or [5]. So
		// I've built these separate rules to record the metadata about what it was.
		ArrayAccess.Rule
			= Expr + "[" + Expr + "]"
			;

		ArraySliceFull.Rule
			= Expr + "[" + Expr + colon + Expr + "]"
			;

		ArraySliceFromStart.Rule
			= Expr + "[" + colon + Expr + "]"
			;

		ArraySliceFromEnd.Rule
			= Expr + "[" + Expr + colon + "]"
			;

		this.Root = StmtList;       // Set grammar root

		// 4. Token filters - created in a separate method CreateTokenFilters
		//    we need to add continuation symbol to NonGrammarTerminals because it is not used anywhere in grammar
		NonGrammarTerminals.Add( ToTerm( @"\" ) );

		// 5. Operators precedence
		RegisterOperators( -3, "+=" );
		RegisterOperators( 1, "||" );
		RegisterOperators( 2, "&&" );
		RegisterOperators( 3, "==", "!=" );
		RegisterOperators( 4, "<", ">", "<=", ">=" );
		RegisterOperators( 5, "+", "-" );
		RegisterOperators( 6, "*", "/", "%" );
		RegisterOperators( 7, Associativity.Right, "**" );
		RegisterOperators( 12, "++", "--" );

		// 6. Miscellaneous: punctuation, braces, transient nodes
		MarkPunctuation( "(", ")", ":" );
		RegisterBracePair( "(", ")" );
		MarkTransient( Term, Expr, Stmt, ExtStmt, UnOp, BinOp, ExtStmt, ParExpr, Block );

		// 7. Error recovery rule
		ExtStmt.ErrorRule = SyntaxError + Eos;
		FunctionDef.ErrorRule = SyntaxError + Dedent;
		IfStmt.ErrorRule = SyntaxError + Dedent;

		// 8. Syntax error reporting
		AddToNoReportGroup( "(" );
		AddToNoReportGroup( Eos );
		AddOperatorReportGroup( "operator" );
		this.LanguageFlags = LanguageFlags.NewLineBeforeEOF /*| LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt*/;
	}

	public static NumberLiteral CreateNumber( string name )
	{
		NumberLiteral term = new NumberLiteral( name, NumberOptions.NoDotAfterInt );
		//default int types are Integer (32bit) -> LongInteger (BigInt); Try Int64 before BigInt: Better performance?
		term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64 };
		// term.DefaultFloatType = TypeCode.Double; -- it is default
		//float type is implementation specific, thus try decimal first (higher precision)
		//term.DefaultFloatTypes = new TypeCode[] { TypeCode.Decimal, TypeCode.Double };
		term.AddPrefix( "0x", NumberOptions.Hex );
		// term.AddPrefix( "0", NumberOptions.Octal );
		// term.AddSuffix( "L", TypeCode.Int64, NumberLiteral.TypeCodeBigInt );
		// term.AddSuffix( "J", NumberLiteral.TypeCodeImaginary );
		return term;
	}

	public override void CreateTokenFilters( LanguageData language, TokenFilterList filters )
	{
		var outlineFilter = new CodeOutlineFilter( language.GrammarData,
			OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces, ToTerm( @"\" ) ); // "\" is continuation symbol
		filters.Add( outlineFilter );
	}

}

}
