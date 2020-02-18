using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NUnit2To3SyntaxConverter.Extensions.SyntaxFactoryUtils;

namespace NUnit2To3SyntaxConverter.ExpectedException
{
  public static class WellKnownExceptions
  {
    public static ExpressionSyntax ThrowsExceptionConstrainSyntax (string exceptionName)
    {
      var exceptionNameWithoutSystemNs = Regex.Replace (exceptionName, "^(System.)", "");
      return IsWellKnownException (exceptionNameWithoutSystemNs)
          ? (ExpressionSyntax) MemberAccess (IdentifierName ("Throws"), exceptionNameWithoutSystemNs)
          : SimpleInvocation (
              MemberAccess (
                  IdentifierName ("Throws"),
                  GenericName (Identifier ("InstanceOf"), TypeArgumentList (SeparatedList (new[] { (TypeSyntax) IdentifierName (exceptionName) })))),
              new ExpressionSyntax [0]);
    }

    private static bool IsWellKnownException (string exceptionName)
    {
      return exceptionName switch
      {
          nameof(Exception) => true,
          nameof(ArgumentException) => true,
          nameof(ArgumentNullException) => true,
          nameof(InvalidOperationException) => true,
          nameof(TargetInvocationException) => true,
          _ => false
      };
    }
  }
}