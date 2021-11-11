using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;

namespace CGServicesHV.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class Messages : List<Message>
    {
        public List<Message> Errors()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Error).ToList();
        }

        public List<Message> Warnings()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Warning).ToList();
        }

        public List<Message> Infos()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Info).ToList();
        }

        public void Add(string name, string message, Message.SeverityType severity)
        {
            this.Add(new Message(name, message, severity));
        }

        public void Add(List<Message> messages)
        {
            this.AddRange(messages);
        }
    }

    [DataContract(Namespace = "")]
    public class Message
    {
        public enum SeverityType
        {
            Error,
            Warning,
            Info
        };

        public Message()
        {
        }

        public Message(string name)
        {
            Name = name;
        }

        public Message(string name, string value)
        {
            Name = name;
            Value = value;
            Severity = SeverityType.Info;
        }

        public Message(string name, string value, SeverityType severity)
        {
            Name = name;
            Value = value;
            Severity = severity;
        }

        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20)]
        public string Value { get; set; }
        [DataMember(Order = 30)]
        public SeverityType Severity { get; set; }
    }
}
