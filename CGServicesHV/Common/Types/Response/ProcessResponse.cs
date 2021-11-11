using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;

namespace CGServicesHV.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]

    public class ProcessResponses : List<ProcessResponse>
    {
    }

    [DataContract(Namespace = "")]
    public class ProcessResponse
    {
        public ProcessResponse()
        {
            StartDate = DateTime.Now;
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["PrimaryEnvironment"];
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            SqlDmlResponses = new SqlDmlResponses();
            ConnectionStrings = new ConnectionStrings();
            ProcessResponses = new ProcessResponses();
            Messages = new Messages();
            SerializedObjects = new SerializedObjects();
            Errors = new Errors();
            ProcessSteps = new ProcessSteps();
            RowsAffected = 0;
            SearchKey = "";
            IsParallel = false;
        }

        public ProcessResponse(string name, bool getServiceContext = false, string plantNum = null)
        {
            StartDate = DateTime.Now;
            ProcessName = name;
            Plant = (plantNum != null) ? PlantManager.GetPlant(plantNum) : null;
            ServiceContext = (getServiceContext) ? new ServiceContext() : null;
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["PrimaryEnvironment"];
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            SqlDmlResponses = new SqlDmlResponses();
            ConnectionStrings = new ConnectionStrings();
            ProcessResponses = new ProcessResponses();
            Messages = new Messages();
            SerializedObjects = new SerializedObjects();
            Errors = new Errors();
            ProcessSteps = new ProcessSteps();
            RowsAffected = 0;
            SearchKey = "";
            IsParallel = false;
        }

        public ProcessResponse(string name, Plant plant, bool getServiceContext = false)
        {
            StartDate = DateTime.Now;
            ProcessName = name;
            Plant = plant;
            ServiceContext = (getServiceContext) ? new ServiceContext() : null;
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["PrimaryEnvironment"];
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            SqlDmlResponses = new SqlDmlResponses();
            ConnectionStrings = new ConnectionStrings();
            ProcessResponses = new ProcessResponses();
            Messages = new Messages();
            SerializedObjects = new SerializedObjects();
            Errors = new Errors();
            ProcessSteps = new ProcessSteps();
            RowsAffected = 0;
            SearchKey = "";
            IsParallel = false;
        }

        [DataMember(Order = 10)]
        public string ProcessName { get; set; }
        [DataMember(Order = 12)]
        public string SearchKey { get; set; }

        [DataMember(Order = 20)]
        public DateTime StartDate { get; set; }
        [DataMember(Order = 22)]
        public DateTime EndDate { get; set; }
        [DataMember(Order = 24)]
        public decimal ElapsedSeconds { get; set; }
        [DataMember(Order = 26)]
        public Errors Errors { get; set; }
        [DataMember(Order = 28)]
        public Messages Messages { get; set; }
        [DataMember(Order = 30)]
        public ConnectionStrings ConnectionStrings { get; set; }

        [DataMember(Order = 50)]
        public int RowsAffected { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public SqlBulkCopyResponses SqlBulkCopyResponses { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public SqlDmlResponses SqlDmlResponses { get; set; }

        [XmlIgnore]
        public SerializedObjects SerializedObjects { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public Plant Plant { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string Environment { get; set; }
        [DataMember(Order = 122, EmitDefaultValue = false)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 124, EmitDefaultValue = false)]
        public ServiceContext ServiceContext { get; set; }

        [DataMember(Order = 130, EmitDefaultValue = false)]
        public bool IsParallel { get; set; }

        [DataMember(Order = 150)]
        public ProcessSteps ProcessSteps { get; set; }

        [DataMember(Order = 200)]
        public ProcessResponses ProcessResponses { get; set; }

        public void Close(bool logZero = false)
        {
            foreach (var childProcessResponse in ProcessResponses)
            {
                if (SqlBulkCopyResponses == null) SqlBulkCopyResponses = new SqlBulkCopyResponses();
                foreach (var sqlBulkCopyResponse in childProcessResponse.SqlBulkCopyResponses)
                {
                    SqlBulkCopyResponses.Add(sqlBulkCopyResponse);
                    if (sqlBulkCopyResponse.HasErrors())
                    {
                        if (childProcessResponse.Errors == null) childProcessResponse.Errors = new Errors();
                        childProcessResponse.Errors.Add(new Error(sqlBulkCopyResponse.Message));
                    }
                }
                if (SqlDmlResponses == null) SqlDmlResponses = new SqlDmlResponses();
                foreach (var sqlDmlResponse in childProcessResponse.SqlDmlResponses)
                {
                    SqlDmlResponses.Add(sqlDmlResponse);
                }
                if (childProcessResponse.HasErrors())
                {
                    foreach (var error in childProcessResponse.Errors)
                    {
                        if (Errors == null) Errors = new Errors();
                        Errors.Add(error);
                    }
                }
            }
            EndDate = DateTime.Now;
            ElapsedSeconds = (decimal)(EndDate - StartDate).TotalSeconds;
            Logging.InsertProcessLog(this, logZero);
        }

        public string Serialize()
        {
            try
            {
                var root = new XElement("root", XElement.Load(new StringReader(XmlSerializationUtility.SerializeObject(Encoding.UTF8, this))));
                foreach (var serializedObject in SerializedObjects)
                {
                    var element = new XElement(serializedObject.Name, XElement.Load(new StringReader(serializedObject.Value)));
                    root.Add(element);
                }
                var value = root.ToString();
                return value;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public void AddStep(ProcessStep step)
        {
            step.Id = this.ProcessSteps.Count + 1;
            ProcessSteps.Add(step);
        }

        public void Done()
        {
            EndDate = DateTime.Now;
            ElapsedSeconds = (decimal)(EndDate - StartDate).TotalSeconds;
        }

        public bool HasErrors()
        {
            return ResponseHasErrors(this);
        }

        public bool ResponseHasErrors(ProcessResponse response)
        {
            if (response.Errors.Any() || response.ProcessSteps.Any(step => { return step != null && step.Errors.Any(); }))
            {
                return true;
            }

            foreach (var childResponse in response.ProcessResponses)
            {
                if (ResponseHasErrors(childResponse))
                {
                    return true;
                }
            }

            foreach (var step in response.ProcessSteps)
            {
                if (response.SqlBulkCopyResponses.Any(sqlBulkCopyResponse => sqlBulkCopyResponse.Errors.Any()))
                {
                    return true;
                }
                if (response.SqlDmlResponses.Any(sqlDmlResponse => sqlDmlResponse.Errors.Any()))
                {
                    return true;
                }
                foreach (var childResponse in step.ProcessResponses)
                {
                    if (ResponseHasErrors(childResponse))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ResponseHasErrors2(ProcessResponse response)
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
