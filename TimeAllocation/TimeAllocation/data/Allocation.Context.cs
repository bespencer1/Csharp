﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeAllocation.data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class AllocationEntities : DbContext
    {
        public AllocationEntities()
            : base("name=AllocationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Assignment_Allocation> Assignment_Allocation { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<vw_Assignment_Allocation> vw_Assignment_Allocation { get; set; }
        public DbSet<WeekEnding> WeekEnding { get; set; }
    
        public virtual int upds_Assignment_Allocation_Update(Nullable<int> allocation_ID, Nullable<int> assignment_ID, Nullable<System.DateTime> week_Ending, Nullable<double> hrs)
        {
            var allocation_IDParameter = allocation_ID.HasValue ?
                new ObjectParameter("Allocation_ID", allocation_ID) :
                new ObjectParameter("Allocation_ID", typeof(int));
    
            var assignment_IDParameter = assignment_ID.HasValue ?
                new ObjectParameter("Assignment_ID", assignment_ID) :
                new ObjectParameter("Assignment_ID", typeof(int));
    
            var week_EndingParameter = week_Ending.HasValue ?
                new ObjectParameter("Week_Ending", week_Ending) :
                new ObjectParameter("Week_Ending", typeof(System.DateTime));
    
            var hrsParameter = hrs.HasValue ?
                new ObjectParameter("Hrs", hrs) :
                new ObjectParameter("Hrs", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("upds_Assignment_Allocation_Update", allocation_IDParameter, assignment_IDParameter, week_EndingParameter, hrsParameter);
        }
    }
}
