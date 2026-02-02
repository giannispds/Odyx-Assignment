# Odyx-Assignment

This solution implements the required provider matching and scoring logic as described in the assignment.  
The focus is on clean design, separation of concerns, and testability.

## Technology Stack
- .NET 8
- C#
- xUnit
- Moq

## Structure
- Models
- Domain entities representing providers, requestors, services, and certifications.

- Services
- Calculates the provider score as the average of all applicable scoring factors (certifications, assessment score, activity recency, project frequency, and monetary value).
- Applies filtering rules based on service type, capacity, cost profile, digital maturity, and location proximity, then ranks providers using the scoring service.

- Tests
- Unit tests covering scoring logic, matching rules, edge cases, and ranking behavior.

### Design Principles
- Dependency Injection via interfaces
- Separation of concerns between scoring and matching
- Simple, readable domain models

## Validation
- Model validation is implemented using **Data Annotations**
- FluentValidation was intentionally not used, as the assignment requirements are satisfied with simple declarative validation
- Validation rules are applied only where business constraints are clearly defined


## Testing
Unit tests cover:
- Provider scoring with all factors present
- Provider scoring with missing factors
- Boundary value (edge case) testing
- Correct average score calculations
- Provider matching logic (filtering, ranking, and limits)

Mocks are used only where necessary (e.g. scoring service in matching tests).