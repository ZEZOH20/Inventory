# Version 2 Implementation Plan: Inventory Management System Upgrade

## Overview

This document outlines the phased implementation plan for upgrading the Inventory Management System to Version 2. The upgrade introduces multi-user role management, approval workflows, advanced reporting, and enhanced usability while maintaining backward compatibility.

## Key Objectives

- Implement multi-role user management (Owner, Manager, Employee) with appropriate permissions
- Add approval workflows for orders with email notifications and audit trails
- Build advanced reporting dashboard with manual refresh and PDF exports using QuestPDF
- Ensure incremental development with testing at each phase
- Adapt architectural patterns from development-guide.md

## Phased Implementation

### Phase 1: Authentication & User Management (Foundation)

**Status**: Ready to Implement  
**Objectives**: Establish secure user authentication and role-based authorization as the foundation for all features.

**Detailed Scope**:

- Install ASP.NET Core Identity packages (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`)
- Create `ApplicationUser` model extending `IdentityUser` with additional fields:
  - Name (string)
  - Phone (string)
  - WarehouseId (int?, nullable for non-Managers)
- Update `SqlDbContext` to inherit from `IdentityDbContext<ApplicationUser>`
- Configure Identity services in `Program.cs`:
  - Add Identity with EF stores
  - Configure JWT authentication with bearer tokens
  - Set password policies (require digits, uppercase, etc.)
  - Seed roles: "Owner", "Manager", "Employee"
- Create `AuthController` with endpoints:
  - POST /api/auth/register (with role selection)
  - POST /api/auth/login (return JWT token)
  - POST /api/auth/reset-password
- Add JWT settings to `appsettings.json`:
  - Issuer, Audience, Key, TokenExpiration
- Apply `[Authorize]` and role-based attributes to existing controllers
- Seed initial Owner user via migration or startup seeding

**Dependencies**: None (foundation phase)

**Estimated Effort**: 1-2 weeks

**Risks & Mitigations**:

- Breaking existing API endpoints: Add auth gradually, test each controller
- JWT configuration errors: Use secure random keys, validate token parsing
- Database migration issues: Backup database, test migrations on dev environment

**Success Criteria**:

- Users can register with roles
- Login returns valid JWT tokens
- Role-based access works (e.g., Manager can't access Owner-only endpoints)
- Existing functionality remains intact

### Phase 2: Approval Workflows & Notifications

**Status**: Pending (after Phase 1)  
**Objectives**: Implement order approval processes with notifications and comprehensive audit trails.

**Detailed Scope**:

- Add approval fields to order entities (`Supply_Order`, `Release_Order`, `Transfer_Order`):
  - Status (enum: Pending, Approved, Rejected)
  - ApprovedBy (string, user ID)
  - ApprovedAt (DateTime?)
  - Notes (string, for rejection reasons)
- Update order creation services:
  - Employees: Create orders with Status=Pending
  - Managers: Approve/reject orders for their warehouses only
  - Owners: Bypass approval process
- Implement email notifications using SMTP:
  - Notify manager when order submitted for approval
  - Notify employee of approval/rejection with details
- Extend `AuditableEntity` for order actions (CreatedBy, UpdatedBy, etc.)
- Update DTOs and Fluent Validation for new fields
- Add approval endpoints to controllers:
  - PUT /api/orders/{id}/approve
  - PUT /api/orders/{id}/reject

**Dependencies**: Phase 1 (user roles and authentication for permissions)

**Estimated Effort**: 2-3 weeks

**Risks & Mitigations**:

- Email delivery failures: Implement retry logic, log failures, provide in-app notification fallback
- Permission logic complexity: Write comprehensive unit tests for role-based access
- Data integrity during status changes: Use transactions for approval updates

**Success Criteria**:

- Orders created by Employees are pending approval
- Managers receive email notifications for their warehouse orders
- Approval/rejection updates inventory and sends notifications
- Full audit trail maintained for all order actions

### Phase 3: Advanced Reporting & Dashboard

**Status**: Pending (after Phase 2)  
**Objectives**: Build comprehensive reporting APIs with data visualization and PDF export capabilities.

**Detailed Scope**:

- Create reporting controller with endpoints:
  - GET /api/reports/supply-orders (filter by date range, warehouse)
  - GET /api/reports/release-orders (filter by date range, warehouse)
  - GET /api/reports/transfer-orders (filter by date range, warehouse)
  - GET /api/reports/financial-summary (costs, revenues, profit/loss)
- Implement manual refresh mechanism (simple GET requests)
- Add PDF export using QuestPDF library:
  - Generate formatted reports with tables, charts, totals
  - Include headers, footers, company branding
- Role-based report access:
  - Owners: All warehouses and data
  - Managers: Only their assigned warehouses
  - Employees: Limited basic reports
- Update `Response.cs` with `ReportResponse<T>` for structured data
- Add date range filtering and warehouse-specific aggregations

**Dependencies**:

- Phase 1 (authentication for access control)
- Phase 2 (audited data and approval status for accurate reporting)

**Estimated Effort**: 3-4 weeks

**Risks & Mitigations**:

- Performance issues with large datasets: Implement pagination, database indexing, query optimization
- PDF generation complexity: Start with simple layouts, incrementally add features
- Data consistency: Ensure reports reflect current approved/audited data

**Success Criteria**:

- All user roles can access appropriate reports
- Manual refresh works correctly
- PDF exports contain properly formatted data
- Financial summaries calculate accurately

### Phase 4: Financial Integration (Future)

**Status**: Deferred  
**Objectives**: Add invoicing, payment tracking, and accounting software integration.

**Detailed Scope**:

- Implement basic invoicing for orders
- Add payment tracking and status
- Integrate with accounting software (e.g., QuickBooks API)
- Add cost analysis and margin calculations

**Dependencies**: All previous phases

**Estimated Effort**: 4-6 weeks

## Implementation Guidelines

### Code Quality Standards

- Follow SOLID principles and clean architecture
- Use dependency injection throughout
- Implement comprehensive error handling with try-catch blocks
- Write unit tests for all services and critical logic
- Use meaningful commit messages and follow conventional commits

### Testing Strategy

- Unit tests for services, repositories, and validation logic
- Integration tests for authentication and authorization
- Manual testing for workflows and UI interactions
- Build and run tests after each phase completion

### Database Migration Strategy

- Create migrations incrementally for each phase
- Backup database before migrations
- Test migrations on development environment first
- Document any data transformations required

### Security Considerations

- Use HTTPS for all communications
- Store sensitive data (JWT keys) securely
- Implement proper password hashing via Identity
- Validate all inputs and prevent injection attacks
- Log security events for audit purposes

## Timeline and Milestones

- **Week 1-2**: Phase 1 completion and testing
- **Week 3-5**: Phase 2 completion and testing
- **Week 6-9**: Phase 3 completion and testing
- **Week 10+**: Phase 4 (future) or additional features

## Next Steps

1. Review and approve this implementation plan
2. Begin Phase 1 implementation
3. Complete each phase with thorough testing before proceeding
4. Document any deviations or additional requirements discovered during implementation</content>
   <parameter name="filePath">d:\college\courses\ITI\Dotnet4Monthes\Ado\Final-Project\Inventory\backend\docs\version2_implementation_plan.md
