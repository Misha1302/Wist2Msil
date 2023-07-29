using System.Security;

[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: SecurityRules(SecurityRuleSet.None, SkipVerificationInFullTrust = true)]