// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Bad code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Bad code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "Bad code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "I prefer it like this", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Bad code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Kept for consistency", Scope = "member", Target = "~M:Animalopoly.Code.AI.Easy(System.String,Animalopoly.Code.TileClasses.Animal,Animalopoly.Code.PlayerClass.Player)~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Kept for consistency", Scope = "member", Target = "~M:Animalopoly.Code.AI.Medium(System.String,Animalopoly.Code.TileClasses.Animal,Animalopoly.Code.PlayerClass.Player)~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Kept for consistency", Scope = "member", Target = "~M:Animalopoly.Code.AI.Expert(System.String,Animalopoly.Code.TileClasses.Animal,Animalopoly.Code.PlayerClass.Player)~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Bad code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Adding readonly to mutated variable", Scope = "member", Target = "~F:Animalopoly.Code.Graphing.Grapher.points")]
[assembly: SuppressMessage("Performance", "CA1854:Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method", Justification = "Unclear code", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
[assembly: SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Animalopoly")]
//[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Animalopoly is designed for Windows", Scope = "member", Target = "~M:Animalopoly.Code.Program.Main")]
