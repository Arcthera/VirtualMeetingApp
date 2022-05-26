using Newtonsoft.Json;
using VirtualMeetingApp;

namespace MeetingAppUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize()]
        public void Initialize() 
        {
            Person person = new Person("Mazvydas", "1234");
            Database.AddNewPerson(person);

            Meeting meeting = new Meeting("Test Meeting", person.Name, "This is a test meeting", MeetingCategory.TeamBuilding, MeetingType.Live, new DateTime(), new DateTime());
            Database.AddNewMeeting(meeting);
        }

        [TestMethod]
        public void Adding_Person_Updates_List()
        {
            Person newPerson = new Person("Edgaras", "4321");
            Database.AddNewPerson(newPerson);


            Assert.IsTrue(Database.People.Contains(newPerson));
        }

        [TestMethod]
        public void Adding_Meeting_Updates_List()
        {
            Meeting newMeeting = new Meeting("Another meeting", "Mazvydas", "Yet another meeting", MeetingCategory.Short, MeetingType.InPerson, new DateTime(), new DateTime());

            Database.AddNewMeeting(newMeeting);

            Assert.IsTrue(Database.Meetings.Contains(newMeeting));
        }

        [TestMethod]
        public void Removing_Meeting_Updates_List()
        {
            Meeting newMeeting = Database.Meetings.First();

            Database.DeleteMeeting(newMeeting);

            Assert.IsFalse(Database.Meetings.Contains(newMeeting));
        }

        [TestMethod]
        public void Existing_Person_Is_Found()
        {
            Assert.IsTrue(Database.PersonExists("Mazvydas"));
        }

        [TestMethod]
        public void Nonexisting_Person_Returns_False()
        {
            Assert.IsFalse(Database.PersonExists("AaAaA"));
        }

        [TestMethod]
        public void Adding_Person_To_Meeting_Updates_Atendees()
        {
            Meeting meeting = Database.Meetings.First();

            Person newPerson = new Person("Edgaras", "5426");

            meeting.AddPersonToMeeting(newPerson);

            Assert.IsTrue(meeting.Atendees.Contains(newPerson.Name));
        }

        [TestMethod]
        public void Removing_Person_From_Meeting_Updates_Atendees()
        {
            Meeting meeting = Database.Meetings.First();

            Person newPerson = new Person("Edgaras", "5426");

            meeting.AddPersonToMeeting(newPerson);

            if (meeting.Atendees.Contains(newPerson.Name))
            {
                meeting.RemovePersonFromMeeting(newPerson);

                Assert.IsFalse(meeting.Atendees.Contains(newPerson.Name));
            }
            else
            {
                Assert.Fail("Cannot test if removing a person from a meeting removes them from the atendee list because adding them doesn't work in the first place.");
            }
        }

        [TestMethod]
        public void Removing_Responsible_Person_Fails()
        {
            Meeting meeting = Database.Meetings.First();
            Person? responsiblePerson = Database.GetPerson(meeting.ResponsiblePerson);

            if (responsiblePerson == null) Assert.Fail("No responsable person for a meeting was found");

            meeting.RemovePersonFromMeeting(responsiblePerson);

            Assert.IsTrue(meeting.Atendees.Contains(responsiblePerson.Name));
        }
    }
}