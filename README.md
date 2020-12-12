# Quote Manager

Small project that allows to add, update, remove or trade quotes.

## Background

This project was developed as a coding exercise where specific requirements were given by a technical recruiter. The main goal was to evaluate the level of knowledge on the below topics:
- Object Oriented Programming
- Data Structures
- Efficiency on the implementation
- Implementation practices
- Design patterns

For the coding exercise, the directions were provided by email and a timeline was specified for completion (3 hours). The project was sent to the recruiter upon completion for evaluation. 

**Note:** This project has been uploaded into GitHub as it was developed under the specified timeline and no further changes have been applied.

## General Requirements
- Project must use .NET Core.
- C# as programming language.
- Implement a class based on the IQuoteManager interface definition (review the `IQuoteManager.cs` file for the whole list of requirements).
- Other classes can be added in order to support the implementation, however, a specific requirement asked to **not modify the definition of IQuoteManager**.

## Stack of Technologies
- C#
- .NET Core 3.1
- Moq
- MsTest

## Special Considerations

### Repository

Due to the time constraint, an In-Memory repository approach was the best solution, however, the design of this implementation allows to easily replace this repository by a different type: database, file, etcetera.

### Unit Tests

The `main` method in `Program.cs` does not have any object instantiation, this was also due to the time constraint. In this case the implementation of the solution was done using Test Driven Development (TDD). All use cases are covered and can be verified by running the unit tests.
