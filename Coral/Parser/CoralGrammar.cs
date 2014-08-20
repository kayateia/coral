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
		var PoundRef = new NonTerminal( "PoundRef" );
		var BinExpr = new NonTerminal( "BinExpr", typeof( AstExpression ) );
		var ParExpr = new NonTerminal( "ParExpr" );
		var UnExpr = new NonTerminal( "UnExpr" );
		var UnOp = new NonTerminal( "UnOp", "operator" );
		var BinOp = new NonTerminal( "BinOp", "operator" );
		var PrePostOp = new NonTerminal( "PrePostOp", "operator" );
		var PreOpExpr = new NonTerminal( "PreOpExpr" );
		var PostOpExpr = new NonTerminal( "PostOpExpr" );
		var AssignmentStmt = new NonTerminal( "AssignmentStmt", typeof( AstAssignment ) );
		var Stmt = new NonTerminal( "Stmt" );
		var ExtStmt = new NonTerminal( "ExtStmt" );
		var ReturnStmt = new NonTerminal( "ReturnStmt" );
		var IfStmt = new NonTerminal( "IfStmt", typeof( AstIf ) );
		var ElifClause = new NonTerminal( "ElifClause" );
		var ElifClauses = new NonTerminal( "ElifClauses" );
		var ElseClause = new NonTerminal( "ElseClause" );
		var TryStmt = new NonTerminal( "TryStmt" );
		var ExceptClause = new NonTerminal( "ExceptClause" );
		var ExceptClauses = new NonTerminal( "ExceptClauses" );
		var FinallyClause = new NonTerminal( "FinallyClause" );
		var Block = new NonTerminal( "Block" );
		var StmtList = new NonTerminal( "StmtList", typeof( AstStatements ) );
		var MemberAccess = new NonTerminal( "MemberAccess" );
		var ArrayExpr = new NonTerminal( "ArrayExpr" );
		var ArrayElements = new NonTerminal( "ArrayElements" );
		var DictExpr = new NonTerminal( "DictExpr" );
		var DictElement = new NonTerminal( "DictElement" );
		var DictElements = new NonTerminal( "DictElements" );
		var ForStmt = new NonTerminal( "ForStmt");

		var ParamList = new NonTerminal( "ParamList" );
		var ArgList = new NonTerminal( "ArgList" );
		var FunctionDef = new NonTerminal( "FunctionDef" );
		var FunctionCall = new NonTerminal( "FunctionCall" );


		// 3. BNF rules
		//Eos is End-Of-Statement token produced by CodeOutlineFilter
		Expr.Rule = Term | UnExpr | BinExpr | PreOpExpr | PostOpExpr | PoundRef | MemberAccess | ArrayExpr | FunctionCall | DictExpr;
		Term.Rule = number | dollar | ParExpr | identifier | str;
		PoundRef.Rule = "#" + number;
		MemberAccess.Rule = Expr + "." + identifier;
		ParExpr.Rule = "(" + Expr + ")";
		UnExpr.Rule = UnOp + Term;
		UnOp.Rule = ToTerm( "+" ) | "-" | "!";
		BinExpr.Rule = Expr + BinOp + Expr;
		BinOp.Rule = ToTerm( "+" ) | "-" | "*" | "/" | "**" | "<" | ">" | "<=" | ">=" | "!=" | "==";
		PrePostOp.Rule = ToTerm( "--" ) | "++";
		PreOpExpr.Rule = PrePostOp + identifier;
		PreOpExpr.Rule |= PrePostOp + MemberAccess;
		PostOpExpr.Rule = identifier + PrePostOp;
		PostOpExpr.Rule |= MemberAccess + PrePostOp;
		AssignmentStmt.Rule = identifier + "=" + Expr;
		AssignmentStmt.Rule |= MemberAccess + "=" + Expr;
		Stmt.Rule = AssignmentStmt | Expr | ReturnStmt | Empty;
		ReturnStmt.Rule = "return" + Expr;
		IfStmt.Rule = "if" + Expr + colon + Stmt;
		IfStmt.Rule |= "if" + Expr + colon + Eos + Block;
		IfStmt.Rule |= "if" + Expr + colon + Eos + Block + PreferShiftHere() + ElifClauses;
		IfStmt.Rule |= "if" + Expr + colon + Eos + Block + PreferShiftHere() + ElifClauses + PreferShiftHere() + ElseClause;
		ElifClauses.Rule = MakeStarRule( ElifClauses, null, ElifClause );
		ElifClause.Rule = "elif" + Expr + colon + Eos + Block;
		ElseClause.Rule = "else" + colon + Eos + Block;
		TryStmt.Rule = "try" + colon + Eos + Block + ExceptClauses;
		TryStmt.Rule |= "try" + colon + Eos + Block + ExceptClauses + FinallyClause;
		ExceptClause.Rule = "except" + colon + Eos + Block;
		ExceptClause.Rule |= "except" + identifier + colon + Eos + Block;
		ExceptClause.Rule |= "except" + identifier + identifier + colon + Eos + Block;
		ExceptClauses.Rule = MakeStarRule( ExceptClauses, null, ExceptClause );
		FinallyClause.Rule = "finally" + colon + Eos + Block;
		ExtStmt.Rule = Stmt + Eos | FunctionDef | IfStmt | TryStmt | ForStmt;
		Block.Rule = Indent + StmtList + Dedent;
		StmtList.Rule = MakePlusRule( StmtList, ExtStmt );

		ParamList.Rule = MakeStarRule( ParamList, comma, identifier );
		ArgList.Rule = MakeStarRule( ArgList, comma, Expr );
		FunctionDef.Rule = "def" + identifier + "(" + ParamList + ")" + colon + Eos + Block;
		FunctionDef.NodeCaptionTemplate = "def #{1}(...)";
		FunctionCall.Rule = identifier + "(" + ArgList + ")";
		FunctionCall.Rule |= Expr + "(" + ArgList + ")";
		FunctionCall.NodeCaptionTemplate = "call #{0}(...)";

		// Implement JSON
		ArrayExpr.Rule = "[" + ArrayElements + "]";
		ArrayElements.Rule = MakeStarRule( ArrayElements, comma, Expr );
		DictExpr.Rule = "{" + DictElements + "}";
		DictElement.Rule = Term + ":" + Expr;
		DictElements.Rule = MakeStarRule( DictElements, comma, DictElement );

		ForStmt.Rule = "for" + identifier + "in" + Expr + colon + Eos + Block;
		ForStmt.Rule |= "for" + AssignmentStmt + comma + Expr + comma + Expr + colon + Eos + Block;

		this.Root = StmtList;       // Set grammar root

		// 4. Token filters - created in a separate method CreateTokenFilters
		//    we need to add continuation symbol to NonGrammarTerminals because it is not used anywhere in grammar
		NonGrammarTerminals.Add( ToTerm( @"\" ) );

		// 5. Operators precedence
		RegisterOperators( 1, "+", "-" );
		RegisterOperators( 2, "*", "/" );
		RegisterOperators( 3, Associativity.Right, "**" );
		RegisterOperators( 4, "<", ">", "<=", ">=", "!=", "==" );

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
		term.AddPrefix( "0", NumberOptions.Octal );
		term.AddSuffix( "L", TypeCode.Int64, NumberLiteral.TypeCodeBigInt );
		term.AddSuffix( "J", NumberLiteral.TypeCodeImaginary );
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
