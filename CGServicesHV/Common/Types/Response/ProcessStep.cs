using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class ProcessSteps : List<ProcessStep>
    {
    }

    [DataContract(Namespace = "")]
    public class ProcessStep
    {
        public ProcessStep()
        {
            StartDate = DateTime.Now;
            Errors = new Errors();
            Messages = new Messages();
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            SqlDmlResponses = new SqlDmlResponses();
            RowsAffected = 0;
        }

        public ProcessStep(string name)
        {
            Name = name;
            StartDate = DateTime.Now;
            StartDateChar = StartDate.ToString("MM/dd/yyyy hh:mm:ss tt");
            Errors = new Errors();
            Messages = new Messages();
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            SqlDmlResponses = new SqlDmlResponses();
            ProcessResponses = new ProcessResponses();
            RowsAffected = 0;
        }

        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public DateTime StartDate { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string StartDateChar { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public DateTime EndDate { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string EndDateChar { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public decimal ElapsedSeconds { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = true)]
        public int RowsAffected { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public Messages Messages { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public Errors Errors { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public SqlBulkCopyResponses SqlBulkCopyResponses { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public SqlDmlResponses SqlDmlResponses { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ProcessResponses ProcessResponses { get; set; }

        public void Done()
        {
            EndDate = DateTime.Now;
            EndDateChar = EndDate.ToString("MM/dd/yyyy hh:mm:ss tt");
            ElapsedSeconds = (decimal)(EndDate - StartDate).TotalSeconds;
        }

        public bool HasErrors()
        {
            return ProcessStepHasErrors(this);
            //return Errors.Any();
        }

        public bool ProcessStepHasErrors(ProcessStep processStep)
        {
            if (Errors.Any())
            {
                return true;
            }
            foreach (var childResponse in processStep.ProcessResponses)
            {
                if (childResponse.Errors.Any() || childResponse.ProcessSteps.Any(step => step.Errors.Any()))
                {
                    return true;
                }
                if (ResponseHasErrors(childResponse))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResponseHasErrors(ProcessResponse response)
        {
            if (response.Errors.Any())
            {
                return true;
            }
            foreach (var childResponse in response.ProcessResponses)
            {
                if (childResponse.Errors.Any() || childResponse.ProcessSteps.Any(step => step.Errors.Any()))
                {
                    return true;
                }
                if (ResponseHasErrors(childResponse))
                {
                    return true;
                }
            }
            foreach (var step in response.ProcessSteps)
            {
                if (step.Errors.Any())
                {
                    return true;
                }
                if (step.ProcessResponses.Any(ResponseHasErrors))
                {
                    return true;
                }
            }
            if (response.SqlBulkCopyResponses.Any(sqlBulkCopyResponse => sqlBulkCopyResponse.Errors.Any()))
            {
                return true;
            }
            if (response.SqlDmlResponses.Any(sqlDmlResponse => sqlDmlResponse.Errors.Any()))
            {
                return true;
            }
            return false;
        }
    }
}
