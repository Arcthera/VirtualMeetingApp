using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMeetingApp
{
    public static class Database
    {
        public static List<Person> People { get; }
        public static List<Meeting> Meetings { get; }

        static Database()
        {
            if (File.Exists("people.json")) People = LoadPeopleJson();
            else People = new List<Person>();

            if (File.Exists("meetings.json")) Meetings = LoadMeetingsJson();
            else Meetings = new List<Meeting>();
        }

        public static Person? GetPerson(string name)
        {
            foreach (Person person in People)
            {
                if (person.Name == name)
                {
                    return person;
                }
            }

            return null;
        }

        public static bool PersonExists(string name)
        {
            if(People.Any(p => p.Name == name))
            {
                return true;
            }

            return false;
        }

        public static void AddNewMeeting(Meeting meeting)
        {
            Meetings.Add(meeting);
            SaveMeetingsJson();
        }

        public static void AddNewPerson(Person person)
        {
            if (!PersonExists(person.Name))
            {
                People.Add(person);
                SaveMeetingsJson();
            }
        }

        public static void DeleteMeeting(Meeting meeting)
        {
            Meetings.Remove(meeting);
            SaveMeetingsJson();
        }

        public static List<Meeting> GetResponsibleForMeetings(Person person)
        {
            return Meetings.Where(meeting => meeting.ResponsiblePerson == person.Name).ToList();
        }

        public static void SaveMeetingsJson()
        {
            string json = JsonConvert.SerializeObject(Meetings);

            File.WriteAllText("meetings.json", json);
        }

        public static void SavePeopleJson()
        {
            string json = JsonConvert.SerializeObject(People);

            File.WriteAllText("people.json", json);
        }

        private static List<Person> LoadPeopleJson()
        {
            using (StreamReader r = new StreamReader("people.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Person>>(json);
            }       
        }

        private static List<Meeting> LoadMeetingsJson()
        {
            if (!File.Exists("meetings.json"))
            {
                File.Create("meetings.json").Close();
            }

            using (StreamReader r = new StreamReader("meetings.json"))
            {
                string json = r.ReadToEnd();

                if (String.IsNullOrEmpty(json))
                {
                    return new List<Meeting>();
                }

                return JsonConvert.DeserializeObject<List<Meeting>>(json);
            }
        }
    }
}
