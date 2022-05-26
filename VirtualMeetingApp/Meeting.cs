using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMeetingApp
{
    public enum MeetingCategory
    {
        CodeMonkey,
        Hub,
        Short,
        TeamBuilding
    }

    public enum MeetingType
    {
        Live,
        InPerson
    }

    public class Meeting
    {
        public string Name { get; }
        public string ResponsiblePerson { get; }
        public string Description { get; }
        public MeetingCategory Category { get; }
        public MeetingType Type { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public List<String> Atendees { get; }

        public Meeting (string name, string responsiblePerson, string description, MeetingCategory category, MeetingType type, DateTime startDate, DateTime endDate)
        {
            this.Name = name;
            this.ResponsiblePerson = responsiblePerson;
            this.Description = description;
            this.Category = category;
            this.Type = type;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Atendees = new List<String>();
        }

        public void AddPersonToMeeting(Person person)
        {
            if (!PersonAlreadyExistsInMeeting(person))
            {
                Atendees.Add(person.Name);
                Database.SaveMeetingsJson();
            }
        }

        public void RemovePersonFromMeeting(Person person)
        {
            if (!IsResponsablePerson(person))
            {
                Atendees.Remove(person.Name);
                Database.SaveMeetingsJson();
            }
        }

        public bool PersonAlreadyExistsInMeeting(Person person)
        {
            if (Atendees.Contains(person.Name)) return true;

            return false;
        }

        public bool IsResponsablePerson(Person person)
        {
            if (ResponsiblePerson.Equals(person.Name)) return true;

            return false;
        }
    }
}
