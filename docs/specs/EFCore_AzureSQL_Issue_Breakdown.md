# Epic Breakdown for EF Core Azure SQL Repository Implementation

## Summary
- **Total Epics**: 4
- **Total Sub-Issues**: 13
- **Estimated Timeline**: 3 weeks (15 working days)
- **Total Story Points**: ~120 hours

## Implementation Strategy

**Sequential Epics:**
- Epic #106 → Epic #111 → Epic #115 (must complete in order)
- Epic #119 can run parallel with Epic #115

**Week-by-Week Breakdown:**
- **Week 1**: Epic #106 (DbContext & Configuration) + Start Epic #115 (Migrations)
- **Week 2**: Epic #111 (Repository Implementation)
- **Week 3**: Epic #119 (Connection Management) + Integration Testing

---

## Epic Details

### Epic 1: EF Core Database Context and Configuration
**GitHub Issue**: [#106](https://github.com/ricardocovo/ghc-sample-ps/issues/106)  
**Title**: `[Epic] EF Core Database Context and Configuration`  
**Timeline**: Week 1 (5 days)  
**Dependencies**: None

**Purpose**: Establish the foundation for database access by creating ApplicationDbContext with proper configuration, entity mappings, and automatic audit field population.

#### Sub-Issues

1. **#107 - Install EF Core packages and tooling** (Small: 30min - 1hr)
   - Add EF Core 10.0 NuGet packages to Core and Test projects
   - Install dotnet-ef CLI tool globally
   - Verify solution builds successfully
   
2. **#108 - Create ApplicationDbContext with audit functionality** (Medium: 3-4hrs)
   - Create DbContext class with Players DbSet
   - Override SaveChangesAsync to auto-populate audit fields
   - Configure retry policies and command timeout
   - Write unit tests for audit field population
   
3. **#109 - Implement Player entity configuration with Fluent API** (Medium: 2-3hrs)
   - Create PlayerConfiguration class
   - Map all properties with proper SQL types
   - Define indexes for UserId, Name, DateOfBirth
   - Configure constraints and default values
   
4. **#110 - Configure DbContext registration in Program.cs** (Small: 1-2hrs)
   - Register DbContext with dependency injection
   - Configure SQL Server provider with retry logic
   - Set up health check endpoint
   - Enable development-specific logging

**Success Criteria:**
- ApplicationDbContext manages database connections properly
- Audit fields automatically populated on save
- Entity configurations enforce business rules at database level
- All unit tests passing with 90%+ coverage

---

### Epic 2: EF Core Repository Implementation
**GitHub Issue**: [#111](https://github.com/ricardocovo/ghc-sample-ps/issues/111)  
**Title**: `[Epic] EF Core Repository Implementation`  
**Timeline**: Week 2 (5 days)  
**Dependencies**: #106 (DbContext must exist first)

**Purpose**: Replace in-memory mock repository with production-ready EF Core implementation supporting all CRUD operations with proper error handling and logging.

#### Sub-Issues

1. **#112 - Implement EfPlayerRepository with CRUD operations** (Medium: 4-6hrs)
   - Create EfPlayerRepository implementing IPlayerRepository
   - Implement all methods using ApplicationDbContext
   - Use AsNoTracking for read-only queries
   - Add comprehensive logging for all operations
   
2. **#113 - Add comprehensive repository unit tests** (Medium: 4-6hrs)
   - Write tests using EF InMemory provider
   - Test all CRUD operations
   - Test audit field population
   - Test query optimization (AsNoTracking)
   - Achieve 95%+ code coverage
   
3. **#114 - Implement database exception handling and logging** (Small: 2-3hrs)
   - Catch DbUpdateException, DbUpdateConcurrencyException, SqlException
   - Translate to domain-specific exceptions
   - Log all errors with operation context
   - Provide user-friendly error messages
   
4. **#122 - End-to-end integration testing with database** (Large: 6-8hrs)
   - Test all CRUD operations through UI
   - Verify data persistence across restarts
   - Measure performance benchmarks
   - Test concurrent access scenarios
   - Document test results

**Success Criteria:**
- All IPlayerRepository methods working with database
- Player data persists permanently
- Performance targets met (< 100ms per operation)
- Error handling provides good user experience
- All tests passing with 95%+ coverage

---

### Epic 3: Database Migrations and Schema Management
**GitHub Issue**: [#115](https://github.com/ricardocovo/ghc-sample-ps/issues/115)  
**Title**: `[Epic] Database Migrations and Schema Management`  
**Timeline**: Week 1 Day 5 + Week 3 Days 1-2 (3 days total)  
**Dependencies**: #106 (DbContext and configurations must be complete)

**Purpose**: Create version-controlled database migrations with seed data for development and automated migration application.

#### Sub-Issues

1. **#116 - Generate initial migration with Players table** (Small: 1-2hrs)
   - Generate InitialCreate migration using dotnet ef
   - Review migration code for correctness
   - Verify all columns, constraints, and indexes
   - Test migration application and rollback
   
2. **#117 - Add seed data to migration for development** (Small: 1-2hrs)
   - Add HasData configuration for 10 sample players
   - Generate migration with seed data
   - Verify seed data only applies in Development
   - Test migration with seed data
   
3. **#118 - Configure automatic migration application on startup** (Small: 1hr)
   - Add Database.Migrate() logic to Program.cs
   - Only run in Development environment
   - Log migration application
   - Handle migration errors gracefully

**Success Criteria:**
- InitialCreate migration creates Players table correctly
- Seed data populates 10 players in development
- Migrations apply automatically on startup in development
- Migration rollback works correctly

---

### Epic 4: Secure Connection String Management
**GitHub Issue**: [#119](https://github.com/ricardocovo/ghc-sample-ps/issues/119)  
**Title**: `[Epic] Secure Connection String Management`  
**Timeline**: Week 3 Days 1-2 (2 days, can overlap with Epic 3)  
**Dependencies**: #106 (DbContext must be configured)

**Purpose**: Implement secure connection string storage using User Secrets for development and Key Vault references for production.

#### Sub-Issues

1. **#120 - Configure User Secrets for development connection string** (Small: 30min - 1hr)
   - Initialize User Secrets in Web project
   - Set LocalDB connection string
   - Verify application reads from User Secrets
   - Test with different connection strings
   
2. **#121 - Document connection string setup for developers** (Medium: 2-3hrs)
   - Create Database_Connection_Setup.md documentation
   - Document LocalDB and Azure SQL setup
   - Include troubleshooting guide
   - Update README files with setup links

**Success Criteria:**
- No connection strings in source control
- User Secrets working for local development
- Documentation guides developers through setup
- Production configuration ready for Key Vault

---

## Implementation Timeline

### Week 1: Foundation Setup (Days 1-5)

**Days 1-2: DbContext and Entity Configuration**
- #107: Install EF Core packages (30min)
- #108: Create ApplicationDbContext (3-4hrs)
- #109: Implement Player entity configuration (2-3hrs)
- Total: ~7-8 hours

**Days 3-4: Configuration and Initial Migration**
- #110: Configure DbContext registration (1-2hrs)
- Unit tests for DbContext and configurations (2-3hrs)
- #116: Generate initial migration (1-2hrs)
- Total: ~5-7 hours

**Day 5: Migration Finalization**
- #117: Add seed data to migration (1-2hrs)
- #118: Configure automatic migration application (1hr)
- Test migration end-to-end (2hrs)
- Total: ~4-5 hours

**Week 1 Total**: ~16-20 hours

---

### Week 2: Repository Implementation (Days 1-5)

**Days 1-2: Repository Implementation**
- #112: Implement EfPlayerRepository (4-6hrs)
- Manual testing of CRUD operations (2hrs)
- Update Program.cs to use EF repository (30min)
- Total: ~6-8 hours

**Days 3-4: Testing and Error Handling**
- #113: Add comprehensive repository unit tests (4-6hrs)
- #114: Implement database exception handling (2-3hrs)
- Total: ~6-9 hours

**Day 5: Integration Testing**
- #122: End-to-end integration testing (6-8hrs)
- Bug fixes and refinements (2-3hrs)
- Total: ~8-11 hours

**Week 2 Total**: ~20-28 hours

---

### Week 3: Connection Management and Finalization (Days 1-5)

**Days 1-2: Connection String Management**
- #120: Configure User Secrets (30min - 1hr)
- #121: Document connection string setup (2-3hrs)
- Test with Azure SQL if available (2hrs)
- Total: ~4-6 hours

**Days 3-4: Final Testing and Documentation**
- Update existing service tests if needed (2-3hrs)
- Performance testing and optimization (2-3hrs)
- Update all README files (2hrs)
- Final code review preparation (2hrs)
- Total: ~8-10 hours

**Day 5: Review and Completion**
- Final code review (2hrs)
- Address review feedback (2-3hrs)
- Final testing of complete system (2hrs)
- Documentation finalization (1hr)
- Total: ~7-8 hours

**Week 3 Total**: ~19-24 hours

---

## Overall Timeline Summary

| Week | Focus Area | Hours | Key Deliverables |
|------|-----------|-------|------------------|
| Week 1 | Foundation | 16-20 | DbContext, Configurations, Initial Migration |
| Week 2 | Repository | 20-28 | EF Repository, Unit Tests, Integration Tests |
| Week 3 | Completion | 19-24 | Connection Management, Documentation, Review |
| **Total** | **3 weeks** | **55-72 hours** | **Production-ready EF Core data layer** |

---

## Dependencies and Critical Path

**Critical Path (Sequential):**
1. #107 → #108 → #109 → #110 (Epic #106: DbContext setup)
2. #116 → #117 → #118 (Epic #115: Migrations)
3. #112 → #113 → #114 → #122 (Epic #111: Repository)

**Parallel Work Opportunities:**
- #120 and #121 (Epic #119) can start once #110 is complete
- Documentation can be written in parallel with testing
- Code reviews can happen incrementally

**Blocking Dependencies:**
- Epic #111 cannot start until Epic #106 is complete
- Epic #115 cannot start until #108 and #109 are complete
- #122 cannot start until #112, #113, and #114 are complete

---

## Risk Mitigation

**High Risk Items:**
1. **Migration Issues** (Epic #115)
   - Mitigation: Test migrations extensively on clean database
   - Have rollback procedures ready
   - Review migration SQL before applying

2. **Performance Problems** (Epic #111)
   - Mitigation: Implement indexes from the start
   - Use AsNoTracking for read queries
   - Monitor query performance early
   - Have optimization plan ready

3. **Connection Pool Exhaustion** (Epic #106)
   - Mitigation: Use Scoped lifetime for DbContext
   - Monitor connection metrics
   - Configure appropriate pool size

**Medium Risk Items:**
1. **Concurrency Conflicts** (Epic #111)
   - Mitigation: Implement optimistic concurrency
   - Handle DbUpdateConcurrencyException gracefully
   - Provide clear error messages

2. **Developer Setup Issues** (Epic #119)
   - Mitigation: Clear documentation with troubleshooting
   - Support multiple connection string formats
   - Test setup process on clean machine

---

## Success Metrics

**Code Quality:**
- DbContext and configurations: 100% test coverage
- Repository implementation: 95% test coverage
- Overall data layer: 90% minimum coverage
- Zero critical or high severity bugs

**Performance:**
- Single player query: < 50ms (P95)
- Player list (100 records): < 200ms (P95)
- CRUD operations: < 100ms (P95)

**Functionality:**
- All CRUD operations working
- Data persists across restarts
- Concurrent access safe
- Migrations apply successfully
- Error handling provides good UX

**Documentation:**
- Setup guide complete
- Troubleshooting guide complete
- README files updated
- All code has XML documentation

---

## Next Steps After Completion

**Phase 2: Production Deployment (1 week)**
- Provision Azure SQL Database
- Configure Key Vault
- Set up managed identity
- Apply migrations to production
- Deploy to Azure Container Apps

**Phase 3: Performance Optimization (1-2 weeks)**
- Query optimization based on usage patterns
- Implement caching strategies
- Index optimization
- Connection pool tuning

**Phase 4: Advanced Features (2 weeks)**
- Soft delete functionality
- Audit log tables
- Optimistic concurrency UI handling
- Bulk operations

---

## Questions and Decisions

**Open Decisions:**
- [ ] LocalDB vs Azure SQL for development? (Recommendation: LocalDB)
- [ ] Implement soft delete in Phase 1? (Recommendation: No, defer to Phase 4)
- [ ] Use row version for optimistic concurrency? (Recommendation: Yes, low overhead)
- [ ] Separate audit log table? (Recommendation: No, audit fields sufficient for Phase 1)
- [ ] Persist ApplicationUser to database? (Recommendation: No, keep transient from Entra ID)

**Infrastructure Decisions:**
- [ ] Azure SQL pricing tier for production? (Recommendation: Start with Basic, monitor and scale)
- [ ] Enable geo-replication? (Recommendation: Not initially, add when needed)
- [ ] SQL Auth or Managed Identity? (Recommendation: Managed Identity for production)

---

## Communication Plan

**Stakeholder Updates:**
- Daily standup: Progress on current issues
- End of Week 1: Demo DbContext and migrations
- End of Week 2: Demo working repository with persistence
- End of Week 3: Final demo and handoff

**Documentation Deliverables:**
- Week 1: Technical design documentation
- Week 2: Repository implementation guide
- Week 3: Setup and troubleshooting documentation

**Code Review Schedule:**
- #108, #109: Review together (end of Week 1 Day 2)
- #112, #114: Review together (end of Week 2 Day 2)
- Final review: All code (Week 3 Day 5)

---

## Repository Structure After Implementation

```
src/
├── GhcSamplePs.Core/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Configurations/
│   │       └── PlayerConfiguration.cs
│   ├── Migrations/
│   │   ├── [timestamp]_InitialCreate.cs
│   │   ├── [timestamp]_InitialCreate.Designer.cs
│   │   ├── [timestamp]_SeedPlayerData.cs
│   │   └── ApplicationDbContextModelSnapshot.cs
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   └── IPlayerRepository.cs (existing)
│   │   └── Implementations/
│   │       ├── MockPlayerRepository.cs (keep for reference)
│   │       └── EfPlayerRepository.cs (new)
│   └── GhcSamplePs.Core.csproj (updated with EF packages)
│
├── GhcSamplePs.Web/
│   ├── Program.cs (updated with DbContext registration)
│   ├── appsettings.json (no connection string)
│   └── appsettings.Development.json (logging config)
│
tests/
└── GhcSamplePs.Core.Tests/
    ├── Data/
    │   └── ApplicationDbContextTests.cs
    ├── Repositories/
    │   └── EfPlayerRepositoryTests.cs
    ├── TestHelpers/
    │   └── DbContextFactory.cs
    └── GhcSamplePs.Core.Tests.csproj (updated with EF InMemory)

docs/
├── Database_Connection_Setup.md (new)
└── specs/
    ├── EFCore_AzureSQL_Repository_Implementation_Specification.md
    └── EFCore_AzureSQL_Issue_Breakdown.md (this file)
```

---

## Command Reference

**Migration Commands:**
```powershell
# Add migration
dotnet ef migrations add MigrationName --startup-project src\GhcSamplePs.Web --project src\GhcSamplePs.Core

# Apply migrations
dotnet ef database update --startup-project src\GhcSamplePs.Web

# Rollback migrations
dotnet ef database update PreviousMigration --startup-project src\GhcSamplePs.Web

# Generate SQL script
dotnet ef migrations script --idempotent --startup-project src\GhcSamplePs.Web --output migration.sql

# Remove last migration
dotnet ef migrations remove --startup-project src\GhcSamplePs.Web
```

**User Secrets Commands:**
```powershell
# Initialize secrets
dotnet user-secrets init --project src\GhcSamplePs.Web

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..." --project src\GhcSamplePs.Web

# List secrets
dotnet user-secrets list --project src\GhcSamplePs.Web

# Remove secret
dotnet user-secrets remove "ConnectionStrings:DefaultConnection" --project src\GhcSamplePs.Web
```

**Testing Commands:**
```powershell
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests\GhcSamplePs.Core.Tests\GhcSamplePs.Core.Tests.csproj
```

---

## Approval and Sign-off

**Technical Review:**
- [ ] Architecture reviewed and approved
- [ ] Entity configurations reviewed
- [ ] Error handling strategy approved
- [ ] Security approach approved

**Implementation Sign-off:**
- [ ] Week 1 completed - DbContext and migrations working
- [ ] Week 2 completed - Repository implementation and tests passing
- [ ] Week 3 completed - Documentation and final testing complete
- [ ] Production deployment plan approved

**Final Acceptance:**
- [ ] All unit tests passing (95%+ coverage)
- [ ] Integration tests passing
- [ ] Performance benchmarks met
- [ ] Documentation complete and reviewed
- [ ] Code review approved
- [ ] Security review approved
- [ ] Ready for production deployment

---

**Document Version**: 1.0  
**Created**: November 28, 2025  
**Last Updated**: November 28, 2025  
**Created By**: Development Planner Agent

**Related Documents:**
- [EF Core Azure SQL Repository Implementation Specification](./EFCore_AzureSQL_Repository_Implementation_Specification.md)
- [GitHub Project Board](https://github.com/ricardocovo/ghc-sample-ps/projects)
- [Repository Issues](https://github.com/ricardocovo/ghc-sample-ps/issues)
